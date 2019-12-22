using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace webapp.Pages
{
    public class IndexModel : ComponentBase
    {
        public string DataFromAPI { get; private set; }

        public async void GetData()
        {
            //We will make a GET request to a really cool website...

            string baseUrl = "http://orbackend:5000/hello";        //The 'using' will help to prevent memory leaks.        //Create a new instance of HttpClient
            using (HttpClient client = new HttpClient())

            //Setting up the response...         

            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();
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
