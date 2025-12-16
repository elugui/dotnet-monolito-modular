using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure;

public class EstruturasDbContext : DbContext
{
    public EstruturasDbContext(DbContextOptions<EstruturasDbContext> options) : base(options)
    {
    }

    public DbSet<Estrutura> Estruturas => Set<Estrutura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("estruturas");

        modelBuilder.Entity<Estrutura>(entity =>
        {
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CodigoExterno).HasMaxLength(100);
            entity.Property(e => e.EstruturaTipoCodigo).HasPrecision(18, 2);
            entity.Property(e => e.InicioVigencia).IsRequired();
            entity.Property(e => e.TerminoVigencia).IsRequired();
            entity.Property(e => e.Versao).IsRequired();
            entity.Property(e => e.Status).HasConversion<int>().IsRequired();
        });
    }
}