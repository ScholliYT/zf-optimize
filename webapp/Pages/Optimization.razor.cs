using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using webapp.Data.Entities;
using webapp.Data.Entities.Backend;

namespace webapp.Pages
{
    public partial class Optimization : ComponentBase
    {
        [Inject] private protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IToastService ToastService { get; set; }

        public string DataFromAPI { get; private set; }

        public async void StartOptimization()
        {
            string baseUrl = "http://orbackend:5000/optimize";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                var data = new
                {
                    ovens = new BackendOven[]
                    {
                        new BackendOven()
                        {
                            Id=0,
                            CastingCellAmount=1,
                            ChangeDuration=TimeSpan.FromSeconds(5)
                        }
                    },
                    forms = new BackendForm[]
                    {
                        new BackendForm()
                        {
                            Id=0,
                            CastingCells=1,
                            Actions=10,
                            ActionsMax=100,
                            Backend_RequiredAmount = 15
                        }
                    }
                };

                // @"{""ovens"":[{""id"":0,""size"":1,""changeduration_sec"":5}],""forms"":[{""id"":0,""required_amount"":15,""castingcell_demand"":1}]}";
                string json = JsonSerializer.Serialize(data);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var data = streamReader.ReadToEnd();
                if (data != null)
                {
                    Console.WriteLine(data);
                    DataFromAPI = data;
                    StateHasChanged();
                }
            }
        }
    }
}
