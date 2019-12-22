using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webapp.Data.Entities;

namespace webapp.Data.Services
{
    public class OvenService
    {
        private readonly ZFContext _context;

        public OvenService(ZFContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Oven>> GetOvensAsync()
        {
            return await _context.Ovens.ToListAsync();
        }

        public async Task<Oven> GetOvenAsync(int id)
        {
            return await _context.Ovens
              .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Oven> CreateOvenAsync(Oven Oven)
        {
            _context.Ovens.Add(Oven);
            await _context.SaveChangesAsync();
            return await GetOvenAsync(Oven.Id);
        }

        public async Task DeleteOven(Oven Oven)
        {
            _context.Ovens.Remove(Oven);
            await _context.SaveChangesAsync();
        }
    }
}
