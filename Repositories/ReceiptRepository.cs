using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly AppDbContext _context;

        public ReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Receipt>> GetAllAsync()
            => await _context.Receipts
                .Include(r => r.Items).ThenInclude(i => i.Product)
                .ToListAsync();

        public async Task<Receipt?> GetByIdAsync(int id)
            => await _context.Receipts
                .Include(r => r.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<List<Receipt>> GetByUserIdAsync(int userId)
            => await _context.Receipts
                .Include(r => r.Items).ThenInclude(i => i.Product)
                .Where(r => r.UserId == userId)
                .ToListAsync();

        public async Task DeleteAsync(Receipt receipt)
        {
            _context.Receipts.Remove(receipt);
            await _context.SaveChangesAsync();
        }
    }
}