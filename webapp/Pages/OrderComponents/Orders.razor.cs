using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [Parameter] public int? Year { get; set; }

        private protected List<Order> OrderList { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await LoadOrders();
            StateHasChanged();
        }

        private async Task LoadOrders()
        {
            OrderList = await zfContext.Orders.Where(o => o.Date.Year.Equals(Year)).OrderBy(o => o.Date).ToListAsync();
        }

        private protected async Task AddMonth()
        {
            var month = OrderList.Count + 1;
            await zfContext.Orders.AddAsync(new Order
            {
                Date = new DateTime(Year.GetValueOrDefault(), month, 1) 
            });
            await LoadOrders();
            StateHasChanged();
        }

        private protected void ManageProducts(int orderId)
        {
            navigationManager.NavigateTo($"/order/manage/{orderId}");
        }

        private protected async Task DeleteOrder(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
