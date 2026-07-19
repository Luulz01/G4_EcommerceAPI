namespace EcommerceAPI.DTOs
{
    public class ReceiptItemRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateReceiptDto
    {
        public int UserId { get; set; }
        public List<ReceiptItemRequestDto> Items { get; set; } = new();
    }

    public class ReceiptItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class ReceiptResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReceiptItemResponseDto> Items { get; set; } = new();
    }
}