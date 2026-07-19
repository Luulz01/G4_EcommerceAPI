using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _repository;
        private readonly AppDbContext _context; // acceso directo, necesario para la transacción

        public ReceiptService(IReceiptRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<ReceiptResponseDto>> GetAllAsync()
        {
            var receipts = await _repository.GetAllAsync();
            return receipts.Select(ToDto).ToList();
        }

        public async Task<ReceiptResponseDto?> GetByIdAsync(int id)
        {
            var receipt = await _repository.GetByIdAsync(id);
            return receipt == null ? null : ToDto(receipt);
        }

        public async Task<List<ReceiptResponseDto>> GetByUserIdAsync(int userId)
        {
            var receipts = await _repository.GetByUserIdAsync(userId);
            return receipts.Select(ToDto).ToList();
        }

        public async Task<ReceiptResponseDto> CreateAsync(CreateReceiptDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("El recibo debe tener al menos un producto");

            // Transacción: si algo falla, no queda stock descontado a medias
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var receipt = new Receipt
                {
                    UserId = dto.UserId,
                    CreatedAt = DateTime.UtcNow,
                    Items = new List<ReceiptItem>()
                };

                decimal total = 0;

                foreach (var itemDto in dto.Items)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null)
                        throw new KeyNotFoundException($"Producto {itemDto.ProductId} no encontrado");

                    if (itemDto.Quantity <= 0)
                        throw new InvalidOperationException($"La cantidad para el producto {product.Name} debe ser mayor a 0");

                    if (product.Stock < itemDto.Quantity)
                        throw new InvalidOperationException(
                            $"Stock insuficiente para '{product.Name}'. Disponible: {product.Stock}, solicitado: {itemDto.Quantity}");

                    var subtotal = product.Price * itemDto.Quantity;
                    total += subtotal;

                    // Descuento automático de stock
                    product.Stock -= itemDto.Quantity;

                    receipt.Items.Add(new ReceiptItem
                    {
                        ProductId = product.Id,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.Price,
                        Subtotal = subtotal
                    });
                }

                receipt.Total = total; // calculado en backend, nunca recibido del cliente

                _context.Receipts.Add(receipt);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var created = await _repository.GetByIdAsync(receipt.Id);
                return ToDto(created!);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var receipt = await _repository.GetByIdAsync(id);
            if (receipt == null) return false;

            await _repository.DeleteAsync(receipt);
            return true;
        }

        private static ReceiptResponseDto ToDto(Receipt r) => new()
        {
            Id = r.Id,
            UserId = r.UserId,
            Total = r.Total,
            CreatedAt = r.CreatedAt,
            Items = r.Items.Select(i => new ReceiptItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}