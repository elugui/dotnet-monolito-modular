using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;
using MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.CreateEstrutura;

public class CreateEstruturaCommandHandler : IRequestHandler<CreateEstruturaCommand, Guid>
{
    private readonly EstruturasDbContext _context;

    public CreateEstruturaCommandHandler(EstruturasDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateEstruturaCommand request, CancellationToken cancellationToken)
    {
        var estrutura = new Estrutura
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Descricao = request.Descricao
        };

        _context.Estruturas.Add(estrutura);
        await _context.SaveChangesAsync(cancellationToken);

        return estrutura.Id;
    }
}