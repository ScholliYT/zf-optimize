using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using webapp.Data.Services;

namespace webapp.Pages.Oven
{
    public class OvenIndexModel : ComponentBase
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public OvenService OvenService { get; set; }

        public List<Data.Entities.Oven> Ovens = new List<Data.Entities.Oven>();


        protected override async Task OnInitializedAsync()
        {
            await LoadOvens();
        }

        private async Task LoadOvens()
        {
            Ovens.AddRange(await OvenService.GetOvensAsync());

            StateHasChanged();
        }

        private protected void AddOven()
        {
            NavigationManager.NavigateTo("/ovens/create");
        }
    }
}