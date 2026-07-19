using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services
{
    public interface IReceiptService
    {
        Task<List<ReceiptResponseDto>> GetAllAsync();
        Task<ReceiptResponseDto?> GetByIdAsync(int id);
        Task<List<ReceiptResponseDto>> GetByUserIdAsync(int userId);
        Task<ReceiptResponseDto> CreateAsync(CreateReceiptDto dto);
        Task<bool> DeleteAsync(int id);
    }
}