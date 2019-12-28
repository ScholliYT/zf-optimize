using System;
using ChartJs.Blazor.ChartJS.BarChart;
using ChartJs.Blazor.ChartJS.BarChart.Axes;
using ChartJs.Blazor.ChartJS.Common.Axes;
using ChartJs.Blazor.ChartJS.Common.Axes.Ticks;
using ChartJs.Blazor.ChartJS.Common.Properties;
using ChartJs.Blazor.ChartJS.Common.Wrappers;
using ChartJs.Blazor.ChartJS.PieChart;
using ChartJs.Blazor.Charts;
using ChartJs.Blazor.Util;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using webapp.Data;

namespace webapp.Pages
{
    public class IndexModel : ComponentBase
    {
        [Inject] public ZFContext _zfContext { get; set; }

        private protected PieConfig _config;
        private protected ChartJsPieChart _pieChartJs;

        private protected BarConfig _barChartConfig;
        private protected ChartJsBarChart _barChart;
        private protected BarDataset<Int32Wrapper> _barDataSet;

        protected override async Task OnInitializedAsync()
        {
            _config = new PieConfig
            {
                Options = new PieOptions
                {
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = "Sample chart from Blazor"
                    },
                    Responsive = true,
                    Animation = new ArcAnimation
                    {
                        AnimateRotate = true,
                        AnimateScale = true
                    }
                }
            };

            _config.Data.Labels.AddRange(new[] { "A", "B", "C", "D" });

            var pieSet = new PieDataset
            {
                BackgroundColor = new[]
                {
                    ColorUtil.RandomColorString(), ColorUtil.RandomColorString(),
                    ColorUtil.RandomColorString(), ColorUtil.RandomColorString()
                },
                BorderWidth = 0,
                HoverBackgroundColor = ColorUtil.RandomColorString(),
                HoverBorderColor = ColorUtil.RandomColorString(),
                HoverBorderWidth = 1,
                BorderColor = "#ffffff"
            };

            pieSet.Data.AddRange(new double[] { 4, 5, 6, 7 });
            _config.Data.Datasets.Add(pieSet);

            await InitBarChart();
        }

        private async Task InitBarChart()
        {
            _barChartConfig = new BarConfig
            {
                Options = new BarOptions
                {
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = "Jahresübersicht"
                    },
                    Scales = new BarScales
                    {
                        XAxes = new List<CartesianAxis> {
                        new BarCategoryAxis {
                            BarPercentage = 0.5,
                            BarThickness = BarThickness.Flex
                        }
                    },
                        YAxes = new List<CartesianAxis> {
                        new BarLinearCartesianAxis {
                            Ticks = new LinearCartesianTicks {
                                BeginAtZero = true
                            }
                        }
                    }
                    }
                }
            };

            var orders = await Task.Run(_zfContext.OrderProducts.Where(o => o.Order.Date.Year == DateTime.Now.Year)
                .Include(o => o.Order).AsEnumerable().GroupBy(o => o.Order)
                .Select(g => new { g.Key.Date.Month, ProductsCount = g.Sum(op => op.Amount) }).ToList);

            _barChartConfig.Data.Labels.AddRange(orders.Select(o => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(o.Month)).ToList());

            _barDataSet = new BarDataset<Int32Wrapper>
            {
                Label = "Absatzmenge",
                BackgroundColor = new[] { ColorUtil.RandomColorString(), ColorUtil.RandomColorString(), ColorUtil.RandomColorString(), ColorUtil.RandomColorString() },
                BorderWidth = 0,
                HoverBackgroundColor = ColorUtil.RandomColorString(),
                HoverBorderColor = ColorUtil.RandomColorString(),
                HoverBorderWidth = 1,
                BorderColor = "#ffffff"
            };

            _barDataSet.AddRange(orders.Select(o => o.ProductsCount).ToList().Wrap());
            _barChartConfig.Data.Datasets.Add(_barDataSet);
        }
        private protected async Task<int> GesamtMenge()
        {
            return await _zfContext.OrderProducts.Where(op => op.Order.Date.Year == DateTime.Now.Year)
                .SumAsync(x => x.Amount);
        }

        private protected async Task<decimal> Umsatz()
        {
            return await _zfContext.OrderProducts.Where(op => op.Order.Date.Year == DateTime.Now.Year)
                .Include(x => x.Product).Select(x => x.Amount * x.Product.Price).SumAsync();
        }
    }
}