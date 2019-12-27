using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Data;
using webapp.Data.Entities;

namespace webapp.Pages.ProductComponents
{
    public partial class ProductEdit : ComponentBase
    {
        [Inject] public ZFContext _zfContext { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Parameter] public int? Id { get; set; }

        protected Product Product { get; private set; }
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
                    Product = new Product();
                }
                else
                {
                    Product = await _zfContext.Products.FirstOrDefaultAsync(p => p.Id == Id);
                    NotFound = Product == null;
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
                _zfContext.Products.Add(Product);
            }

            await _zfContext.SaveChangesAsync();
            NavigationManager.NavigateTo("/products");
        }
    }
}
