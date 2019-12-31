using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using webapp.Data;

namespace webapp.Pages.OrderComponents
{
    public partial class Years : ComponentBase
    {
        [Inject] private ZFContext zfContext { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }


        private protected List<int> YearList { get; set; }

        private protected int Year { get; set; }


        protected override async Task OnInitializedAsync()
        {
            YearList = await zfContext.Orders.Select(o => o.Date.Year).Distinct().ToListAsync();
            StateHasChanged();
        }

        private protected void RedirectToOrder()
        {
            if (Year != default)
                navigationManager.NavigateTo($"/orders/{Year}");
        }
    }
}
