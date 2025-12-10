using System.Threading.Tasks;
using Grpc.Core;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.GetEstrutura;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.ListEstruturas;
using MonolitoModular.Slices.Cadastrados.Estruturas.Grpc;
using MediatR;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Grpc
{
    public class EstruturasGrpcService : EstruturasService.EstruturasServiceBase
    {
        private readonly IMediator _mediator;
        public EstruturasGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<GetEstruturaResponse> GetEstrutura(GetEstruturaRequest request, ServerCallContext context)
        {
            var estrutura = await _mediator.Send(new GetEstruturaQuery { Codigo = Guid.Parse(request.Codigo) });
            if (estrutura == null)
                return new GetEstruturaResponse();
            return new GetEstruturaResponse
            {
                Estrutura = MapToProto(estrutura)
            };
        }

        public override async Task<ListEstruturasResponse> ListEstruturas(ListEstruturasRequest request, ServerCallContext context)
        {
            var estruturas = await _mediator.Send(new ListEstruturasQuery());
            var response = new ListEstruturasResponse();
            foreach (var estrutura in estruturas)
            {
                response.Estruturas.Add(MapToProto(estrutura));
            }
            return response;
        }

        private Estrutura MapToProto(Domain.Estrutura entidade)
        {
            return new Estrutura
            {
                Codigo = entidade.Codigo.ToString(),
                Nome = entidade.Nome,
                EstruturaTipoCodigo = entidade.EstruturaTipoCodigo.ToString(),
                CodigoExterno = entidade.CodigoExterno,
                InicioVigencia = entidade.InicioVigencia.ToString("o"),
                TerminoVigencia = entidade.TerminoVigencia.ToString("o"),
                Versao = entidade.Versao,
                Status = (int)entidade.Status
            };
        }
    }
}


/* Para executar, rode o host: 
dotnet run --project src/Host/MonolitoModular.Host/MonolitoModular.Host.csproj) 
e depois os testes (dotnet test [MonolitoModular.Slices.Cadastrados.Estruturas.Tests](http://_vscodecontentref_/0)/.
*/