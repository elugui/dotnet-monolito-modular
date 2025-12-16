using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.DeleteEstrutura
{
    public class DeleteEstruturaHandler : IRequestHandler<DeleteEstruturaCommand, bool>
    {
        private readonly EstruturasDbContext _contexto;

        public DeleteEstruturaHandler(EstruturasDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<bool> Handle(DeleteEstruturaCommand request, CancellationToken cancellationToken)
        {
            if (request.Codigo == Guid.Empty)
                throw new ArgumentException("Codigo é obrigatório.");

            // Buscar entidade existente
            var estrutura = await _contexto.Estruturas.FindAsync(new object[] { request.Codigo }, cancellationToken);
            if (estrutura == null)
                throw new InvalidOperationException("Estrutura não encontrada.");

            // Remover e persistir
            _contexto.Estruturas.Remove(estrutura);
            await _contexto.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
