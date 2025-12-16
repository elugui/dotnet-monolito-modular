using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;
using MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure;
using Microsoft.EntityFrameworkCore; 

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.ListEstruturas
{
    public class ListEstruturasHandler : IRequestHandler<ListEstruturasQuery, List<Estrutura>>
    {
        private readonly EstruturasDbContext _contexto;

        public ListEstruturasHandler(EstruturasDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Estrutura>> Handle(ListEstruturasQuery request, CancellationToken cancellationToken)
        {
            var estruturas = await _contexto.Estruturas.ToListAsync(cancellationToken);
            return estruturas;
        }
    }
}
