using Microsoft.EntityFrameworkCore;
using ModularMonolith.Modules.Products.Domain.Entities;
using ModularMonolith.Modules.Products.Domain.ValueObjects;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Products.Infrastructure.Persistence;

public class ProductsDbContext : BaseDbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("products");

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Description)
                .HasMaxLength(1000);

            entity.Property(p => p.StockQuantity)
                .IsRequired();

            entity.Property(p => p.CreatedAt)
                .IsRequired();

            entity.Property(p => p.UpdatedAt);

            entity.Property(p => p.IsActive)
                .IsRequired();

            // Configure Money as owned entity (value object)
            entity.OwnsOne(p => p.Price, price =>
            {
                price.Property(m => m.Amount)
                    .HasColumnName("Price")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                price.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .IsRequired()
                    .HasMaxLength(3);
            });

            // Ignore domain events collection
            entity.Ignore(p => p.DomainEvents);
        });
    }
}
