using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Data.Entities;
using Blazored.Modal;
using Blazored.Modal.Services;

namespace webapp.Pages.ProductComponents
{
    public partial class ProductFormEdit : ComponentBase
    {
        [CascadingParameter]
        ModalParameters Parameters { get; set; }

        public ProductForm ProductForm { get; set; }

        protected override void OnInitialized()
        {
            ProductForm = Parameters.Get<ProductForm>("ProductForm");
        }


        void Confirm()
        {
            ModalService.Close(ModalResult.Ok(ProductForm));
        }

        void Cancel()
        {
            ModalService.Close(ModalResult.Cancel());
        }
    }
}
