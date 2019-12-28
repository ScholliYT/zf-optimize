using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Data.Entities;

namespace webapp.Pages.FormComponents
{
    public partial class FormIndex : ComponentBase
    {
        [Inject] public ZFContext _zfContext { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IToastService ToastService { get; set; }

        public List<Form> FormList { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadForms();
        }

        private async Task LoadForms()
        {
            FormList = await _zfContext.Forms.ToListAsync();
            StateHasChanged();
        }

        public async Task AddRandomForm()
        {
            try
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
            catch (Exception)
            {
                ToastService.ShowError($"Fehler beim erstellen einer neuen Form.");
            }
        }

        public void AddForm()
        {
            NavigationManager.NavigateTo("/form");
        }

        public void EditForm(int id)
        {
            NavigationManager.NavigateTo($"/form/{id}");
        }

        public async Task DeleteForm(Form form)
        {
            try
            {
                _zfContext.Forms.Remove(form);
                await _zfContext.SaveChangesAsync();
                await LoadForms();
            }
            catch (Exception)
            {
                ToastService.ShowError($"Fehler beim Löschen von Form {form.Id}.");
            }
        }
    }
}
