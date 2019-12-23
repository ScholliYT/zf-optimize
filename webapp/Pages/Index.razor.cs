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

            await InitBarChartAsync();
        }

        private async Task InitBarChartAsync()
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

            var orders = await _zfContext.OrderProducts.GroupBy(o => o.Order)
                .Select(g => new { g.Key.Date.Month, ProductsCount = g.Sum(op => op.Amount)})
                .ToListAsync();

            _barChartConfig.Data.Labels.AddRange(orders.Select(o => o.Month.ToString()).ToList());

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
    }
}