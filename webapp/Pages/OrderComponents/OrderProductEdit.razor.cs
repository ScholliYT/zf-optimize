using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Data.Entities;

namespace webapp.Pages.OrderComponents
{
    public partial class OrderProductEdit : ComponentBase
    {
        [CascadingParameter]
        ModalParameters Parameters { get; set; }

        public OrderProduct OrderProduct { get; set; }

        protected override void OnInitialized()
        {
            OrderProduct = Parameters.Get<OrderProduct>("OrderProduct");
        }

        void Confirm()
        {
            ModalService.Close(ModalResult.Ok(OrderProduct));
        }

        void Cancel()
        {
            ModalService.Close(ModalResult.Cancel());
        }
    }
}
