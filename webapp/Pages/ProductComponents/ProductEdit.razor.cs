using Blazored.Toast.Services;
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
        [Inject] protected IToastService ToastService { get; set; }

        [Parameter] public int? Id { get; set; }

        protected Product Product { get; private set; }
        protected List<ProductForm> ProductForms { get; private set; }
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
                    ProductForms = new List<ProductForm>();
                }
                else
                {
                    Product = await _zfContext.Products.FirstOrDefaultAsync(p => p.Id == Id);
                    NotFound = Product == null;

                    if (!NotFound)
                    {
                        await LoadProductForms();
                    }
                }
            }
            catch (Exception)
            {
                LoadFailed = true;
            }
        }

        private async Task LoadProductForms()
        {
            ProductForms = await _zfContext.ProductForms.Include(pf => pf.Product).Include(pf => pf.Form).ToListAsync();
            StateHasChanged();
        }

        protected async Task Save()
        {
            if (CreationMode)
            {
                _zfContext.Products.Add(Product);
            }

            _zfContext.ProductForms.AddRange(ProductForms);
            await _zfContext.SaveChangesAsync();
            NavigationManager.NavigateTo("/products");
            ToastService.ShowSuccess($"Produkt {Product.Id} {(CreationMode?"erstellt":"geändert")}.");
        }

        protected void AddProductForm()
        {
            ProductForms.Add(new ProductForm());
            StateHasChanged();
        }

        public void EditProductForm(ProductForm productForm)
        {
        }

        public async Task DeleteProductForm(ProductForm productForm)
        {
            ProductForms.Remove(productForm);
            await LoadProductForms();
        }
    }
}
