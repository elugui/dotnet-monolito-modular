using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;
using MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure; 

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.GetEstrutura
{
    public class GetEstruturaHandler : IRequestHandler<GetEstruturaQuery, Estrutura>
    {
        private readonly EstruturasDbContext _contexto;

        public GetEstruturaHandler(EstruturasDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<Estrutura> Handle(GetEstruturaQuery request, CancellationToken cancellationToken)
        {
            if (request.Codigo == Guid.Empty)
                throw new ArgumentException("Codigo é obrigatório.");

            var estrutura = await _contexto.Estruturas.FindAsync(new object[] { request.Codigo }, cancellationToken);
            if (estrutura == null)
                throw new InvalidOperationException("Estrutura não encontrada.");

            return estrutura;
        }
    }
}
