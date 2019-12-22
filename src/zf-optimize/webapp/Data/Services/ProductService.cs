using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Data.Entities;

namespace webapp.Data.Services
{
    public class ProductService
    {
            private readonly ZFContext _context;

            public ProductService(ZFContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Product>> GetItemsAsync()
            {
                return await _context.Products.ToListAsync();
            }

            public async Task<Product> GetFormAsync(int id)
            {
                return await _context.Products
                    .FirstOrDefaultAsync(i => i.Id == id);
            }


            public async Task<Product> CreateItemAsync(Product item)
            {
                _context.Products.Add(item);
                await _context.SaveChangesAsync();
                return await GetFormAsync(item.Id);
            }

            public async Task DeleteItem(Product item)
            {
                _context.Products.Remove(item);
                await _context.SaveChangesAsync();
            }

            public async Task SaveChangesAsync()
            {
                await _context.SaveChangesAsync();
            }
        }

}
