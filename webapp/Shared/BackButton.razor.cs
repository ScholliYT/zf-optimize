using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace webapp.Shared
{
    public partial class BackButton
    {
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private protected void GoBack()
        {
            JSRuntime.InvokeAsync<string>("window.history.go", -1);
        }
    }
}
