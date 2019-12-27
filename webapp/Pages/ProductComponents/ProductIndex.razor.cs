using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using webapp.Data;
using webapp.Data.Entities;

namespace webapp.Pages.ProductComponents
{
    public partial class ProductIndex : ComponentBase
    {
        [Inject] private protected ZFContext _zfContext { get; set; }
        [Inject] private protected NavigationManager NavigationManager { get; set; }

        private protected virtual List<Product> Products => _zfContext.Products.ToList();

        private protected void AddProduct()
        {
            NavigationManager.NavigateTo("/product");
        }
    }
}
