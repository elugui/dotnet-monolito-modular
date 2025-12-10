using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.CreateEstrutura
{
    public class CreateEstruturaHandler : IRequestHandler<CreateEstruturaCommand, Guid>
    {
        // TODO: Inject DbContext via constructor

        public async Task<Guid> Handle(CreateEstruturaCommand request, CancellationToken cancellationToken)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new ArgumentException("Nome é obrigatório");
            if (request.EstruturaTipoCodigo == 0)
                throw new ArgumentException("EstruturaTipoCodigo é obrigatório");
            if (request.TerminoVigencia <= request.InicioVigencia)
                throw new ArgumentException("TerminoVigencia deve ser maior que InicioVigencia");
            if (!Enum.IsDefined(typeof(EstruturaStatus), request.Status))
                throw new ArgumentException("Status inválido");

            // TODO: Persistir entidade no banco
            var estrutura = new Estrutura
            {
                Codigo = Guid.NewGuid(),
                Nome = request.Nome,
                EstruturaTipoCodigo = request.EstruturaTipoCodigo,
                CodigoExterno = request.CodigoExterno,
                InicioVigencia = request.InicioVigencia,
                TerminoVigencia = request.TerminoVigencia,
                Versao = request.Versao,
                Status = (EstruturaStatus)request.Status
            };

            // Exemplo: dbContext.Estruturas.Add(estrutura); await dbContext.SaveChangesAsync();
            await Task.CompletedTask;
            return estrutura.Codigo;
        }
    }
}
