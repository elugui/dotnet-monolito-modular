using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.DeleteEstrutura
{
    public class DeleteEstruturaHandler : IRequestHandler<DeleteEstruturaCommand, bool>
    {
        public async Task<bool> Handle(DeleteEstruturaCommand request, CancellationToken cancellationToken)
        {
            if (request.Codigo == Guid.Empty)
                throw new ArgumentException("Codigo é obrigatório.");

            // TODO: Buscar entidade, remover e persistir
            // Exemplo:
            // var estrutura = await _dbContext.Estruturas.FindAsync(request.Codigo);
            // if (estrutura == null) throw new NotFoundException(...);
            // _dbContext.Estruturas.Remove(estrutura);
            // await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
