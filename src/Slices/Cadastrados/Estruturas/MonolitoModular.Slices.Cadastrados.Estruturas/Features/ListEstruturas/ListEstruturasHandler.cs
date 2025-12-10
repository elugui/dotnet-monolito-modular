using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.ListEstruturas
{
    public class ListEstruturasHandler : IRequestHandler<ListEstruturasQuery, List<Estrutura>>
    {
        public async Task<List<Estrutura>> Handle(ListEstruturasQuery request, CancellationToken cancellationToken)
        {
            // TODO: Buscar lista de entidades
            // Exemplo:
            // var estruturas = await _dbContext.Estruturas.ToListAsync();
            // return estruturas;

            return new List<Estrutura>();
        }
    }
}
