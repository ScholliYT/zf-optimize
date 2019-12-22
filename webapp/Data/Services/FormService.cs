using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webapp.Data.Entities;

namespace webapp.Data.Services
{
    public class FormService
    {
        private readonly ZFContext _context;

        public FormService(ZFContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Form>> GetFormsAsync()
        {
            return await _context.Forms.ToListAsync();
        }

        public async Task<Form> GetFormAsync(int id)
        {
            return await _context.Forms
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Form> CreateFormAsync(Form form)
        {
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            return await GetFormAsync(form.Id);
        }

        public async Task DeleteForm(Form form)
        {
            _context.Forms.Remove(form);
            await _context.SaveChangesAsync();
        }
    }
}