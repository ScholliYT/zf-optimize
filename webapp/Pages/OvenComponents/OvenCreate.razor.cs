using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace webapp.Pages.OvenComponents
{
    public partial class OvenCreate : ComponentBase
    {
        [Inject] public IJSRuntime JSRuntime { get; set; }

        public void GoBack()
        {
            JSRuntime.InvokeAsync<string>("window.history.go", -1);
        }
    }
}
