using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using webapp.Data;
using webapp.Data.Entities;
using webapp.Data.Services;

namespace webapp.Pages
{
    public class FormsModel : ComponentBase
    {
        [Inject] public ZFContext _zfContext { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }

        public List<Form> Forms { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadForms();
        }

        private async Task LoadForms()
        {
            Forms = await _zfContext.Forms.ToListAsync();
            StateHasChanged();
        }

        public async Task AddRandomForm()
        {
            var random = new Random();
            int actionsMax = random.Next(100, 100000);
            int actions = (int)(actionsMax * random.NextDouble());
            Form form = new Form()
            {
                ActionsMax = actionsMax,
                Actions = actions,
                CastingCells = (float)Math.Round(random.NextDouble() * 5f, 2)
            };

            _zfContext.Forms.Add(form);
            await _zfContext.SaveChangesAsync();
            await LoadForms();
        }

        public void EditForm(int id)
        {
            NavigationManager.NavigateTo($"/form/{id}");
        }

        public async Task DeleteForm(Form form)
        {
            _zfContext.Forms.Remove(form);
            await _zfContext.SaveChangesAsync();
            await LoadForms();
        }
    }
}
