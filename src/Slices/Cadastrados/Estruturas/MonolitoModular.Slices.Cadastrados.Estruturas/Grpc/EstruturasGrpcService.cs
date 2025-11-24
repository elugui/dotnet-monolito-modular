using Grpc.Core;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.GetEstrutura;
//using MonolitoModular.Slices.Cadastrados.Estruturas.Grpc;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Grpc;

public class EstruturasGrpcService : EstruturasService.EstruturasServiceBase
{
    private readonly IMediator _mediator;

    public EstruturasGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetEstruturaResponse> GetEstrutura(GetEstruturaRequest request, ServerCallContext context)
    {
        var estrutura = await _mediator.Send(new GetEstruturaQuery(Guid.Parse(request.Id)));
        if (estrutura == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Estrutura não encontrada"));

        return new GetEstruturaResponse
        {
            Estrutura = new EstruturaDto
            {
                Id = estrutura.Id.ToString(),
                Nome = estrutura.Nome,
                Descricao = estrutura.Descricao ?? ""
            }
        };
    }
}
