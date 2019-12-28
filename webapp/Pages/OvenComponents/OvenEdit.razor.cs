using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using webapp.Data;
using webapp.Data.Entities;

namespace webapp.Pages.OvenComponents
{
    public partial class OvenEdit : ComponentBase
    {
        [Inject] public ZFContext _zfContext { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IToastService ToastService { get; set; }

        [Parameter] public int? Id { get; set; }

        protected Oven Oven { get; private set; }
        protected bool LoadFailed { get; private set; }
        protected bool NotFound { get; private set; }
        protected bool CreationMode { get; private set; }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                CreationMode = Id == default;

                if (CreationMode)
                {
                    Oven = new Oven();
                }
                else
                {
                    Oven = await _zfContext.Ovens.FirstOrDefaultAsync(o => o.Id == Id);
                    NotFound = Oven == null;
                }
            }
            catch (Exception)
            {
                LoadFailed = true;
            }
        }

        protected async Task Save()
        {
            if (CreationMode)
            {
                _zfContext.Ovens.Add(Oven);
            }

            await _zfContext.SaveChangesAsync();
            NavigationManager.NavigateTo("/ovens");
            ToastService.ShowSuccess($"Ofen {Oven.Id} erstellt.");
        }
    }
}
