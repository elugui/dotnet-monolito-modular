using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;
using MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.GetEstrutura;

public record GetEstruturaQuery(Guid Id) : IRequest<Estrutura?>;

public class GetEstruturaQueryHandler : IRequestHandler<GetEstruturaQuery, Estrutura?>
{
    private readonly EstruturasDbContext _context;

    public GetEstruturaQueryHandler(EstruturasDbContext context)
    {
        _context = context;
    }

    public async Task<Estrutura?> Handle(GetEstruturaQuery request, CancellationToken cancellationToken)
    {
        return await _context.Estruturas.FindAsync(new object[] { request.Id }, cancellationToken);
    }
}
