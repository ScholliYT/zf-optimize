using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Data.Entities;

namespace webapp.Pages.OvenComponents
{
    public partial class OvenIndex : ComponentBase
    {
        [Inject] public ZFContext _zfContext { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] protected IToastService ToastService { get; set; }


        public List<Oven> Ovens = new List<Oven>();


        protected override async Task OnInitializedAsync()
        {
            await LoadOvens();
        }

        private async Task LoadOvens()
        {
            Ovens.AddRange(await _zfContext.Ovens.ToListAsync());
            StateHasChanged();
        }

        private protected void AddOven()
        {
            NavigationManager.NavigateTo("/oven");
        }

        public void EditOven(int id)
        {
            NavigationManager.NavigateTo($"/oven/{id}");
        }

        public async Task DeleteOven(Oven oven)
        {
            try
            {
                _zfContext.Ovens.Remove(oven);
                await _zfContext.SaveChangesAsync();
                await LoadOvens();
            }
            catch (Exception)
            {
                ToastService.ShowError($"Fehler beim Löschen von Ofen {oven.Id}.");
            }
        }
    }
}