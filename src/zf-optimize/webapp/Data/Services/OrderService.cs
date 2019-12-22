using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webapp.Data.Entities;

namespace webapp.Data.Services
{
    public class OrderService
    {
        private readonly ZFContext _context;

        public OrderService(ZFContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetOrderAsync(int id)
        {
            return await _context.Orders
              .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Order> CreateOrderAsync(Order Order)
        {
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();
            return await GetOrderAsync(Order.Id);
        }

        public async Task DeleteOrder(Order Order)
        {
            _context.Orders.Remove(Order);
            await _context.SaveChangesAsync();
        }
    }
}
