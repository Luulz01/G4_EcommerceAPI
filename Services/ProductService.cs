using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(ToDto).ToList();
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product == null ? null : ToDto(product);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductRequestDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock
            };
            await _repository.AddAsync(product);
            return ToDto(product);
        }

        public async Task<bool> UpdateAsync(int id, ProductRequestDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;

            await _repository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return false;

            await _repository.DeleteAsync(product);
            return true;
        }

        private static ProductResponseDto ToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock
        };
    }
}