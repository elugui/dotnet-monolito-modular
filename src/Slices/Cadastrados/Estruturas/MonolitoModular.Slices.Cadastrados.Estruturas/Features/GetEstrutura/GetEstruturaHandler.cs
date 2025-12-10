using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.GetEstrutura
{
    public class GetEstruturaHandler : IRequestHandler<GetEstruturaQuery, Estrutura>
    {
        public async Task<Estrutura> Handle(GetEstruturaQuery request, CancellationToken cancellationToken)
        {
            if (request.Codigo == Guid.Empty)
                throw new ArgumentException("Codigo é obrigatório.");

            // TODO: Buscar entidade por código
            // Exemplo:
            // var estrutura = await _dbContext.Estruturas.FindAsync(request.Codigo);
            // if (estrutura == null) throw new NotFoundException(...);
            // return estrutura;

            return null;
        }
    }
}
