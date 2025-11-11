using Microsoft.EntityFrameworkCore;
using ModularMonolith.Modules.Customers.Domain.Entities;
using ModularMonolith.Modules.Customers.Domain.ValueObjects;
using ModularMonolith.Shared.Infrastructure.Persistence;

namespace ModularMonolith.Modules.Customers.Infrastructure.Persistence;

public class CustomersDbContext : BaseDbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("customers");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(c => c.CreatedAt)
                .IsRequired();

            entity.Property(c => c.UpdatedAt);

            entity.Property(c => c.IsActive)
                .IsRequired();

            // Configure Email as owned entity (value object)
            entity.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(256);

                email.HasIndex(e => e.Value)
                    .IsUnique();
            });

            // Ignore domain events collection
            entity.Ignore(c => c.DomainEvents);
        });
    }
}
