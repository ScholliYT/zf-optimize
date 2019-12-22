using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using webapp.Data.Entities;
using webapp.Data.Services;

namespace webapp.Pages
{
    public class FormsModel : ComponentBase
    {
        [Inject] protected FormService FormService { get; set; }
        public List<Form> Forms { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadForms();
        }

        private async Task LoadForms()
        {
            Forms = new List<Form>(await FormService.GetFormsAsync());

            StateHasChanged();
        }
    }
}
