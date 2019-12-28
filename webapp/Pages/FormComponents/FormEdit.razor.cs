using System;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Data.Entities;

namespace webapp.Pages.FormComponents
{
    public partial class FormEdit : ComponentBase
    {
        [Inject] public ZFContext _zfContext { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IToastService ToastService { get; set; }
        [Parameter] public int? Id { get; set; }


        protected Form Form { get; private set; }
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
                    Form = new Form();
                }
                else
                {
                    Form = await _zfContext.Forms.FirstOrDefaultAsync(f => f.Id == Id);
                    NotFound = Form == null;
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
                _zfContext.Forms.Add(Form);
            }

            await _zfContext.SaveChangesAsync();

            NavigationManager.NavigateTo("/forms");
            ToastService.ShowSuccess($"Form {Form.Id} {(CreationMode ? "erstellt" : "geändert")}.");
        }
    }
}
