using Microsoft.EntityFrameworkCore;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure;

public class EstruturasDbContext : DbContext
{
    public EstruturasDbContext(DbContextOptions<EstruturasDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("estruturas");

    }
}