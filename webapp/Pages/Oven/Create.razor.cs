using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using webapp.Data;

namespace webapp.Pages.Oven
{
    public class CreateOvenModel : ComponentBase, IBackButton
    {
        [Inject] public IJSRuntime JSRuntime { get; set; }

        public void GoBack()
        {
            JSRuntime.InvokeAsync<string>("window.history.go", -1);
        }
    }
}
