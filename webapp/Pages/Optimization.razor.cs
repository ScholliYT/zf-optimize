using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using webapp.Data;
using webapp.Data.Entities;
using webapp.Data.Entities.Backend;

namespace webapp.Pages
{
    public partial class Optimization : ComponentBase
    {
        [Inject] private protected ZFContext _zfContext { get; set; }
        [Inject] private protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IToastService ToastService { get; set; }

        protected string JSONFromAPI { get; private set; }

        protected List<Order> Orders;
        protected HashSet<Order> SelectedOrders;

        protected List<BackendAssignment> Assignments;

        protected override async Task OnInitializedAsync()
        {
            SelectedOrders = new HashSet<Order>();
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            Orders = await _zfContext.Orders.ToListAsync();
            StateHasChanged();
        }

        public void UpdateOrderCheckedState(Order order, bool state)
        {
            if (state && !SelectedOrders.Contains(order))
            {
                SelectedOrders.Add(order);
            }
            else if (!state && SelectedOrders.Contains(order))
            {
                SelectedOrders.Remove(order);
            }
            StateHasChanged();
            // leave other cases
        }

        public async void StartOptimization()
        {
            try
            {
                if (SelectedOrders.Count == 0)
                {
                    ToastService.ShowWarning($"Bitte wählen Sie min. eine Bestellung aus.");
                    return;
                }

                var ovens = await _zfContext.Ovens.ToListAsync();
                if (ovens.Count == 0)
                {
                    ToastService.ShowWarning($"Bitte konfigurieren Sie min. einen Ofen.");
                    return;
                }


                #region Gather data

                // alle orderproducts der ausgewählten Orders
                var orderProducts = await _zfContext.OrderProducts
                    .Where(op => op.Amount > 0)
                    .Where(op => SelectedOrders.Select(o => o.Id).Contains(op.OrderId))
                    .Include(op => op.Product)
                    .Include(op => op.Order)
                    .ToListAsync();
                var products_used = orderProducts.Select(op => op.Product).Distinct().ToList();

                // genutze Produktforms
                var productForms = await _zfContext.ProductForms
                    .Where(pf => pf.Amount > 0)
                    .Where(pf => products_used.Select(p => p.Id).Contains(pf.ProductId))
                    .Include(pf => pf.Product)
                    .Include(pf => pf.Form)
                    .Distinct()
                    .ToListAsync();
                var forms_used = productForms.Select(pf => pf.Form).Distinct().ToList();

                // Anzahl der Stück je Form berechnen
                var formamounts_floats = new Dictionary<Form, double>();
                foreach (var op in orderProducts)
                {
                    foreach (var pf in productForms.Where(pf => pf.Product == op.Product))
                    {
                        if (!formamounts_floats.ContainsKey(pf.Form))
                        {
                            formamounts_floats.Add(pf.Form, 0);
                        }
                        formamounts_floats[pf.Form] += op.Amount * pf.Amount; // Stück * Formen pro Stück
                    }
                }

                // Wieder in ints konvertieren
                var formamounts = new Dictionary<Form, int>();
                foreach (var faf in formamounts_floats)
                {
                    formamounts.Add(faf.Key, (int)faf.Value);
                }
                #endregion Gather Data

                string baseUrl = "http://orbackend:5000/optimize";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";


                var backend_ovens = ovens.Select(oven => BackendOven.MakeBackendOven(oven)).ToList();
                var backend_forms = forms_used.Select(form => BackendForm.MakeBackendForm(form, formamounts[form])).ToList();

                var ovensIDMapping = MapIDsToStart_Oven(backend_ovens);
                var formsIDMaping = MapIDsToStart_Form(backend_forms);

                // POST
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var data = new
                    {
                        ovens = backend_ovens,
                        forms = backend_forms
                    };

                    // @"{""ovens"":[{""id"":0,""size"":1,""changeduration_sec"":5}],""forms"":[{""id"":0,""required_amount"":15,""castingcell_demand"":1}]}";
                    string json = JsonSerializer.Serialize(data);

                    streamWriter.Write(json);
                }

                // Get Response
                httpWebRequest.ReadWriteTimeout = 360000; // 360 000 ms = 6 min
                httpWebRequest.Timeout = 360000; // 360 000 ms = 6 min
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var data = streamReader.ReadToEnd();
                    if (data != null)
                    {
                        Console.WriteLine(data);
                        JSONFromAPI = data;

                        Assignments = JsonSerializer.Deserialize<List<BackendAssignment>>(data);
                        // TODO: Form Reperaturen herausziehen

                        // IDs wieder zurück mappen
                        foreach (BackendAssignment assignment in Assignments)
                        {
                            assignment.FormAssignments = assignment.FormAssignments.Select(fa => formsIDMaping[fa]).ToList();
                        }

                        StateHasChanged();
                    }
                }
            }
            catch (WebException ex)
            {
                ToastService.ShowError($"Fehler bei der Verbindung zur Backend API. Wohlmöglich ist die API nicht erreichbar.");
            }
            catch (Exception e)
            {
                ToastService.ShowError($"Fehler bei der Verbindung zur Backend API.");
            }
        }

        /// <summary>
        /// Maps IDs of the Form (123, 200, 303...) to (0,1,2...) in place
        /// </summary>
        /// <param name="forms"></param>
        /// <returns>Mapping (old, new)</returns>
        private static Dictionary<int, int> MapIDsToStart_Form(List<BackendForm> forms)
        {
            Dictionary<int, int> idmapping = new Dictionary<int, int>();
            for (int i = 0; i < forms.Count; i++)
            {
                idmapping.Add(i, forms[i].Id);
                forms[i].Id = i;
            }
            return idmapping;
        }

        /// <summary>
        /// Maps IDs of the Oven (123, 200, 303...) to (0,1,2...) in place
        /// </summary>
        /// <param name="ovens"></param>
        /// <returns>Mapping (old, new)</returns>
        private static Dictionary<int, int> MapIDsToStart_Oven(List<BackendOven> ovens)
        {
            Dictionary<int, int> idmapping = new Dictionary<int, int>();
            for (int i = 0; i < ovens.Count; i++)
            {
                idmapping.Add(i, ovens[i].Id);
                ovens[i].Id = i;
            }
            return idmapping;
        }
    }
}
