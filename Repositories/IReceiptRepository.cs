using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories
{
    public interface IReceiptRepository
    {
        Task<List<Receipt>> GetAllAsync();
        Task<Receipt?> GetByIdAsync(int id);
        Task<List<Receipt>> GetByUserIdAsync(int userId);
        Task DeleteAsync(Receipt receipt);
    }
}