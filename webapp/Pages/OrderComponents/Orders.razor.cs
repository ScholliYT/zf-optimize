using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Data.Entities;

namespace webapp.Pages.OrderComponents
{
    public partial class Orders : ComponentBase
    {
        [Inject] private ZFContext zfContext { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] protected IToastService ToastService { get; set; }


        [Parameter] public int Year { get; set; }

        private protected List<Order> OrderList { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            OrderList = await zfContext.Orders.Where(o => o.Date.Year.Equals(Year)).OrderBy(o => o.Date).ToListAsync();
            StateHasChanged();
        }

        private protected async Task AddMonth()
        {
            navigationManager.NavigateTo("/order/manage");
        }

        private protected void ManageProducts(int orderId)
        {
            navigationManager.NavigateTo($"/order/manage/{orderId}");
        }

        private protected async Task DeleteOrder(Order order)
        {
            try
            {
                zfContext.Orders.Remove(order);
                await zfContext.SaveChangesAsync();
                await LoadOrders();
            }
            catch (Exception)
            {
                ToastService.ShowError($"Fehler beim Löschen von der Bestellung.");
            }
        }
    }
}
