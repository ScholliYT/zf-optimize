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

namespace webapp.Pages.OrderComponents
{
    public partial class OrderEdit : ComponentBase
    {
        [Inject] private ZFContext zfContext { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected IToastService ToastService { get; set; }
        [Inject] protected IModalService ModalService { get; set; }

        [Parameter] public int? OrderId { get; set; }

        public Order Order { get; set; }
        private protected List<OrderProduct> OrderProducts { get; set; }

        protected bool NotFound { get; private set; }
        protected bool CreationMode { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                CreationMode = OrderId == default;

                if (CreationMode)
                {
                    Order = new Order()
                    {
                        Date = DateTime.Now.Date
                    };
                    OrderProducts = new List<OrderProduct>();
                }
                else
                {
                    Order = await zfContext.Orders.FirstOrDefaultAsync(o => o.Id == OrderId);
                    NotFound = Order == null;

                    if (!NotFound)
                    {
                        await LoadOrderProducts();
                    }
                }
            }
            catch (Exception)
            {
                ToastService.ShowError($"Fehler beim Laden.");
            }
        }

        private async Task LoadOrderProducts()
        {
            OrderProducts = await zfContext.OrderProducts.Where(op => op.OrderId == Order.Id).Include(op => op.Product).Include(op => op.Order).ToListAsync();
            StateHasChanged();
        }

        protected async Task Save()
        {
            if (CreationMode)
            {
                zfContext.Orders.Add(Order);
            }
            await zfContext.SaveChangesAsync();
            NavigationManager.NavigateTo($"/orders/{Order.Date.Year}");
            ToastService.ShowSuccess($"Bestellung {Order.Id} {(CreationMode ? "erstellt" : "geändert")}.");
        }

        protected void AddOrderProduct()
        {
            if (Order.Id == default)
            {
                ToastService.ShowWarning($"Bitte speichern Sie zuerst die Bestellung.");
                return;
            }

            var orderproduct = new OrderProduct()
            {
                OrderId = Order.Id
            };
            var parameters = new ModalParameters();

            parameters.Add("OrderProduct", orderproduct);

            ModalService.OnClose += AddOrderProductModalClosed;
            ModalService.Show<OrderProductEdit>("Produktbestellung erstellen", parameters);
        }

        protected async void AddOrderProductModalClosed(ModalResult result)
        {
            try
            {
                ModalService.OnClose -= AddOrderProductModalClosed;
                if (!result.Cancelled)
                {
                    OrderProduct orderproduct = (OrderProduct)result.Data;
                    zfContext.OrderProducts.Add(orderproduct);
                    await zfContext.SaveChangesAsync();
                    await LoadOrderProducts();
                }
            }
            catch (Exception)
            {
                ToastService.ShowError($"Fehler beim Hinzufügen der Produktbestellung.");
            }
        }
        protected void EditOrderProduct(OrderProduct orderProduct)
        {
            var parameters = new ModalParameters();

            parameters.Add("OrderProduct", orderProduct);

            ModalService.OnClose += EditOrderProductModalClosed;
            ModalService.Show<OrderProductEdit>("Produktbestellung bearbeiten", parameters);
        }

        protected async void EditOrderProductModalClosed(ModalResult result)
        {
            try
            {
                ModalService.OnClose -= EditOrderProductModalClosed;
                if (!result.Cancelled)
                {
                    OrderProduct orderproduct = (OrderProduct)result.Data;
                    await zfContext.SaveChangesAsync();
                    await LoadOrderProducts();
                }
            }
            catch (Exception)
            {
                ToastService.ShowError($"Fehler beim Bearbeiten der Produktbestellung.");
            }
        }

        protected async void DeleteOrderProduct(OrderProduct orderProduct)
        {
            try
            {
                zfContext.OrderProducts.Remove(orderProduct);
                await zfContext.SaveChangesAsync();
                await LoadOrderProducts();
            }
            catch (Exception)
            {
                ToastService.ShowError($"Fehler beim Löschen von der Produktbestellung.");
            }
        }

    }
}
