using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.UpdateEstrutura
{
    public class UpdateEstruturaHandler : IRequestHandler<UpdateEstruturaCommand, bool>
    {
        public async Task<bool> Handle(UpdateEstruturaCommand request, CancellationToken cancellationToken)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new ArgumentException("Nome é obrigatório.");
            if (request.EstruturaTipoCodigo == 0)
                throw new ArgumentException("EstruturaTipoCodigo é obrigatório.");
            if (request.TerminoVigencia <= request.InicioVigencia)
                throw new ArgumentException("TerminoVigencia deve ser maior que InicioVigencia.");
            if (!Enum.IsDefined(typeof(EstruturaStatus), request.Status))
                throw new ArgumentException("Status inválido.");

            // TODO: Buscar entidade, aplicar alterações, persistir
            // Exemplo:
            // var estrutura = await _dbContext.Estruturas.FindAsync(request.Codigo);
            // if (estrutura == null) throw new NotFoundException(...);
            // estrutura.Nome = request.Nome; ...
            // await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
