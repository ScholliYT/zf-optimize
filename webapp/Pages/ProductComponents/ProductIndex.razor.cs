using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Data.Entities;

namespace webapp.Pages.ProductComponents
{
    public partial class ProductIndex : ComponentBase
    {
        [Inject] private protected ZFContext _zfContext { get; set; }
        [Inject] private protected NavigationManager NavigationManager { get; set; }

        private protected List<Product> Products;

        protected override async Task OnInitializedAsync()
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            Products = await _zfContext.Products.ToListAsync();
            StateHasChanged();
        }

        private protected void AddProduct()
        {
            NavigationManager.NavigateTo("/product");
        }


        public void EditProduct(int id)
        {
            NavigationManager.NavigateTo($"/product/{id}");
        }

        public async Task DeleteProduct(Product product)
        {
            _zfContext.Products.Remove(product);
            await _zfContext.SaveChangesAsync();
            await LoadProducts();
        }
    }
}
