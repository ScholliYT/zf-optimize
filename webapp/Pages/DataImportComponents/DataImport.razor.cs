using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using webapp.Data;
using webapp.Data.Entities;

namespace webapp.Pages.DataImportComponents
{
    public partial class DataImport : ComponentBase
    {
        [Inject] private protected ZFContext zfContext { get; set; }
        private protected bool TaskFinished { get; set; }

        private protected async Task HandleFileUpload(IFileListEntry[] files)
        {
            List<Form>? importedForms;
            List<Product>? importedProducts;
            var file = files.FirstOrDefault();
            if (file != null)
            {
                await using var ms = new MemoryStream();
                await file.Data.CopyToAsync(ms);

                if (file.Type == @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    var excel = new ExcelPackage(ms);
                    var worksheets = excel.Workbook.Worksheets.AsEnumerable().ToList();
                    zfContext.RemoveRange(zfContext.ProductForms);
                    zfContext.RemoveRange(zfContext.OrderProducts);
                    zfContext.RemoveRange(zfContext.Orders);
                    zfContext.RemoveRange(zfContext.Products);
                    zfContext.RemoveRange(zfContext.Forms);
                    await zfContext.SaveChangesAsync();
                    await FindForms();
                    await FindProducts();
                    await zfContext.SaveChangesAsync();
                    await AssociateProductForms();
                    await FindOrders();
                    await zfContext.SaveChangesAsync();
                    TaskFinished = true;
                    StateHasChanged();


                    async Task FindForms()
                    {
                        var forms = worksheets.FirstOrDefault(w => w.Name == "Formen und Gießzellenbedarf");
                        var formsAdresses = forms?.Cells
                            .Where(c => int.Parse(c.Address.Substring(1, c.Address.Length - 1)) > 5 &&
                                        c.Value.ToString().StartsWith('F')).Select(c => c.Address).ToList();
                        if (formsAdresses != null)
                            await zfContext.Forms.AddRangeAsync(from fa in formsAdresses
                                select int.Parse(fa.Substring(1, fa.Length - 1))
                                into row
                                let anzBisher = forms?.GetValue<int>(row, 2)
                                let anzMax = forms?.GetValue<int>(row, 3)
                                let gieszBedarf = forms?.GetValue<float>(row, 4)
                                let name = forms?.GetValue<string>(row, 1)
                                orderby row
                                select new Form
                                {
                                    Actions = anzBisher.GetValueOrDefault(),
                                    ActionsMax = anzMax.GetValueOrDefault(),
                                    CastingCells = gieszBedarf.GetValueOrDefault(),
                                    Name = name
                                });
                    }

                    async Task FindProducts()
                    {
                        var products = worksheets.FirstOrDefault(w => w.Name.Contains("Bestellungen 2019"));
                        var productsAdresses =
                            products?.Cells.Where(c => c.Value != null && c.Value.ToString().StartsWith("Produkt Nr. "))
                                .Select(c => c.Address).ToList();
                        if (productsAdresses != null)
                            await zfContext.Products.AddRangeAsync(from pa in productsAdresses
                                select int.Parse(pa.Substring(1, pa.Length - 1))
                                into row
                                let name = products?.GetValue<string>(row, 3)
                                orderby row
                                select new Product
                                    {Name = name});
                    }

                    async Task AssociateProductForms()
                    {
                        var productforms =
                            worksheets.FirstOrDefault(w => w.Name.Contains("Zuweisung Produkte und Formen"));
                        int tmp;
                        float tmp2;
                        var productAdresses = productforms.Cells.Where(c =>
                                int.TryParse(c.Value.ToString(), out tmp) &&
                                c.Address.StartsWith("A"))
                            .Select(c => c.Address.Substring(1, c.Address.Length - 1)).ToList();

                        var amounts = (from pf in productAdresses
                            select int.Parse(pf)
                            into row
                            let productNr = productforms.GetValue<int>(row, 1)
                            orderby row
                            select new
                            {
                                id = productNr,
                                amounts = productforms.Cells.Where(c =>
                                        c.Address.Substring(1) == row.ToString() && !c.Address.StartsWith('A'))
                                    .Select(c => c.Value.ToString()).ToArray()
                            }).ToList();

                        foreach (var amount in amounts)
                        {
                            var product =
                                await zfContext.Products.SingleAsync(p => p.Name.Contains(amount.id.ToString()));
                            for (var i = 1; i <= zfContext.Forms.Count(); i++)
                            {
                                var form = await zfContext.Forms.SingleAsync(f => f.Name == $"F{i}");
                                await zfContext.ProductForms.AddAsync(new ProductForm
                                {
                                    Form = form,
                                    FormId = form.Id,
                                    Amount = float.Parse(amount.amounts[i - 1]),
                                    Product = product,
                                    ProductId = product.Id
                                });
                            }
                        }
                    }

                    async Task FindOrders()
                    {
                        var ordersheets = worksheets.Where(w => w.Name.Contains("Bestellungen")).ToList();

                        foreach (var ordersheet in ordersheets)
                        {
                            var year = int.Parse(ordersheet.Name.Split(' ')[2]);

                            var productrows = ordersheet.Cells.Where(c => c.Text.StartsWith("Produkt Nr. ")).ToList();

                            foreach (var row in productrows.Select(x => int.Parse(x.Address.Substring(1))))
                                for (var i = 1; i <= 12; i++)
                                {
                                    var order = await zfContext.Orders.AddAsync(new Order
                                    {
                                        Date = new DateTime(year, i, 1)
                                    });
                                    await zfContext.OrderProducts.AddAsync(new OrderProduct
                                    {
                                        Order = order.Entity,
                                        Product = await zfContext.Products.SingleAsync(p =>
                                            p.Name == ordersheet.GetValue<string>(row, 3)),
                                        Amount = ordersheet.GetValue<int>(row, i + 3)
                                    });
                                }
                        }
                    }
                }
            }
        }
    }
}