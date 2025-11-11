using ModularMonolith.Modules.Products.Domain.Events;
using ModularMonolith.Modules.Products.Domain.ValueObjects;
using ModularMonolith.Shared.Domain.Entities;

namespace ModularMonolith.Modules.Products.Domain.Entities;

/// <summary>
/// Product aggregate root
/// </summary>
public sealed class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private Product() : base() 
    { 
        Name = string.Empty;
        Description = string.Empty;
        Price = null!;
    }

    private Product(Guid id, string name, string description, Money price, int stockQuantity) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Product Create(string name, string description, decimal price, int stockQuantity, string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name cannot be empty", nameof(name));
        }

        if (stockQuantity < 0)
        {
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));
        }

        var priceVo = Money.Create(price, currency);
        var product = new Product(Guid.NewGuid(), name, description, priceVo, stockQuantity);

        product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.Price.Amount));

        return product;
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));
        }

        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductStockUpdatedEvent(Id, StockQuantity));
    }

    public void UpdatePrice(decimal newPrice, string currency = "USD")
    {
        Price = Money.Create(newPrice, currency);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductPriceUpdatedEvent(Id, Price.Amount, Price.Currency));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
