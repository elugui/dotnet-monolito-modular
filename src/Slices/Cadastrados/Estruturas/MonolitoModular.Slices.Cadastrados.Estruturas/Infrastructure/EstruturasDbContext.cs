using Microsoft.EntityFrameworkCore;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure;

public class EstruturasDbContext : DbContext
{
    public EstruturasDbContext(DbContextOptions<EstruturasDbContext> options) : base(options) { }

    public DbSet<Estrutura> Estruturas => Set<Estrutura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("estruturas");

        modelBuilder.Entity<Estrutura>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(500);
        });
    }
}