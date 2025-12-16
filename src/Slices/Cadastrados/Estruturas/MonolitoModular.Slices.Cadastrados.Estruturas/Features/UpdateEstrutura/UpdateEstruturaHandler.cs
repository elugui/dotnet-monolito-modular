using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;
using MonolitoModular.Slices.Cadastrados.Estruturas.Infrastructure;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.UpdateEstrutura
{
    public class UpdateEstruturaHandler : IRequestHandler<UpdateEstruturaCommand, bool>
    {
        private readonly EstruturasDbContext _contexto;

        public UpdateEstruturaHandler(EstruturasDbContext contexto)
        {
            _contexto = contexto;
        }

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

            // Buscar entidade existente
            var estrutura = await _contexto.Estruturas.FindAsync(new object[] { request.Codigo }, cancellationToken);
            if (estrutura == null)
                throw new InvalidOperationException("Estrutura não encontrada.");

            // Aplicar alterações
            estrutura.Nome = request.Nome;
            estrutura.EstruturaTipoCodigo = request.EstruturaTipoCodigo;
            estrutura.CodigoExterno = request.CodigoExterno;
            estrutura.InicioVigencia = request.InicioVigencia;
            estrutura.TerminoVigencia = request.TerminoVigencia;
            estrutura.Versao = request.Versao;
            estrutura.Status = request.Status;

            // Persistir alterações
            _contexto.Estruturas.Update(estrutura);
            await _contexto.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
