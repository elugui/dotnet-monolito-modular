using ModularMonolith.Modules.Customers.Domain.Events;
using ModularMonolith.Modules.Customers.Domain.ValueObjects;
using ModularMonolith.Shared.Domain.Entities;

namespace ModularMonolith.Modules.Customers.Domain.Entities;

/// <summary>
/// Customer aggregate root
/// </summary>
public sealed class Customer : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private Customer() : base() 
    { 
        Name = string.Empty;
        Email = null!;
    }

    private Customer(Guid id, string name, Email email, string? phoneNumber) : base(id)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Customer Create(string name, string email, string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Customer name cannot be empty", nameof(name));
        }

        var emailVo = Email.Create(email);
        var customer = new Customer(Guid.NewGuid(), name, emailVo, phoneNumber);

        customer.AddDomainEvent(new CustomerCreatedEvent(customer.Id, customer.Name, customer.Email.Value));

        return customer;
    }

    public void Update(string name, string email, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Customer name cannot be empty", nameof(name));
        }

        Name = name;
        Email = Email.Create(email);
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CustomerUpdatedEvent(Id, Name, Email.Value));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CustomerDeactivatedEvent(Id));
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
