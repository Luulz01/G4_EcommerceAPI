using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;

namespace EcommerceAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Receipt>()
                .Property(r => r.Total)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<ReceiptItem>()
                .Property(ri => ri.UnitPrice)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<ReceiptItem>()
                .Property(ri => ri.Subtotal)
                .HasColumnType("decimal(10,2)");
        }
    }
}