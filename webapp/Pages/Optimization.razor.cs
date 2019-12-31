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
                var ovens = await _zfContext.Ovens.ToListAsync();

                // alle orderproducts der ausgewählten Orders
                var orderProducts = await _zfContext.OrderProducts
                    .Where(op => op.Amount > 0)
                    .Where(op => SelectedOrders.Select(o => o.Id).Contains(op.OrderId))
                    .Include(op => new {op.Order, op.Product})
                    .ToListAsync();
                var products_used = orderProducts.Select(op => op.Product).Distinct().ToList();

                // genutze Produktforms
                var productForms = await _zfContext.ProductForms
                    .Where(pf => products_used.Select(p => p.Id).Contains(pf.ProductId))
                    .Include(pf => new {pf.Form, pf.Product})
                    .Distinct()
                    .ToListAsync();
                var forms_used = productForms.Select(pf => pf.Form).Distinct().ToList();

                var baseUrl = "http://orbackend:5000/optimize";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                await using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var backend_ovens = ovens.Select(BackendOven.MakeBackendOven).ToList();

                    // Anzahl der Stück je Form berechnen
                    var formamounts_floats = new Dictionary<Form, double>();
                    foreach (var op in orderProducts)
                    {
                        foreach (var pf in productForms.Where(pf => pf.Product == op.Product))
                        {
                            if(!formamounts_floats.ContainsKey(pf.Form))
                            {
                                formamounts_floats.Add(pf.Form, 0);
                            }
                            formamounts_floats[pf.Form] += op.Amount * pf.Amount;
                        }
                    }

                    // Wieder in ints konvertieren
                    var formamounts = formamounts_floats.ToDictionary(faf => faf.Key, faf => (int) faf.Value);

                    var backend_forms = forms_used.Select(form => BackendForm.MakeBackendForm(form, formamounts[form])).ToList();

                    var data = new
                    {
                        ovens = backend_ovens,
                        forms = backend_forms
                    };

                    // @"{""ovens"":[{""id"":0,""size"":1,""changeduration_sec"":5}],""forms"":[{""id"":0,""required_amount"":15,""castingcell_demand"":1}]}";
                    var json = JsonSerializer.Serialize(data);

                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using var streamReader = new StreamReader(httpResponse.GetResponseStream());
                {
                    var data = streamReader.ReadToEnd();
                    Console.WriteLine(data);
                    JSONFromAPI = data;

                    Assignments = JsonSerializer.Deserialize<List<BackendAssignment>>(data);
                    // TODO: Wieder um Size Scale faktor runterskallieren
                    StateHasChanged();
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
    }
}
