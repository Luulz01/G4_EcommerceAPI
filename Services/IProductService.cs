using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(ProductRequestDto dto);
        Task<bool> UpdateAsync(int id, ProductRequestDto dto);
        Task<bool> DeleteAsync(int id);
    }
}