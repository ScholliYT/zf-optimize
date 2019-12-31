using Blazored.Modal;
using Blazored.Modal.Services;
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
        [Inject] protected IModalService ModalService { get; set; }

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
            ProductForms = await _zfContext.ProductForms.Where(pf => pf.ProductId == Product.Id).Include(pf => pf.Product).Include(pf => pf.Form).ToListAsync();
            StateHasChanged();
        }
       
        protected async Task Save()
        {
            if (CreationMode)
            {
                _zfContext.Products.Add(Product);
            }
            await _zfContext.SaveChangesAsync();
            NavigationManager.NavigateTo("/products");
            ToastService.ShowSuccess($"Produkt {Product.Id} {(CreationMode ? "erstellt" : "geändert")}.");
        }

        protected void AddProductForm()
        {
            if (Product.Id == default)
            {
                ToastService.ShowWarning($"Bitte speichern Sie zuerst das Produkt.");
                return;
            }

            var productForm = new ProductForm()
            {
                ProductId = this.Product.Id
            };
            var parameters = new ModalParameters();

            parameters.Add("ProductForm", productForm);

            ModalService.OnClose += AddProductFormModalClosed;
            ModalService.Show<ProductFormEdit>("Produkt-Form Zuweisung erstellen", parameters);
        }

        protected async void AddProductFormModalClosed(ModalResult result)
        {
            try
            {
                ModalService.OnClose -= AddProductFormModalClosed;
                if (!result.Cancelled)
                {
                    ProductForm productForm = (ProductForm)result.Data;
                    _zfContext.ProductForms.Add(productForm);
                    await _zfContext.SaveChangesAsync();
                    await LoadProductForms();
                }
            }
            catch (Exception)
            {

                ToastService.ShowError($"Fehler beim Hinzufügen der ProduktForm.");
            }
        }

        public void EditProductForm(ProductForm productForm)
        {
            var parameters = new ModalParameters();

            parameters.Add("ProductForm", productForm);

            ModalService.OnClose += EditProductFormModalClosed;
            ModalService.Show<ProductFormEdit>("Produkt-Form Zuweisung bearbeiten", parameters);
        }

        protected async void EditProductFormModalClosed(ModalResult result)
        {
            try
            {
                ModalService.OnClose -= EditProductFormModalClosed;
                if (!result.Cancelled)
                {
                    ProductForm productForm = (ProductForm)result.Data;
                    await _zfContext.SaveChangesAsync();
                    await LoadProductForms();
                }
            }
            catch (Exception)
            {
                ToastService.ShowError($"Fehler beim Bearbeiten der ProduktForm.");
            }
        }

        public async Task DeleteProductForm(ProductForm productForm)
        {
            _zfContext.ProductForms.Remove(productForm);
            await _zfContext.SaveChangesAsync();
            await LoadProductForms();
            StateHasChanged();
        }
    }
}
