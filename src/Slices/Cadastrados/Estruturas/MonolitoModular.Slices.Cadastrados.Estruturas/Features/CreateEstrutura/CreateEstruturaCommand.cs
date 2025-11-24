using MediatR;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.CreateEstrutura;

public record CreateEstruturaCommand(string Nome, string Descricao) : IRequest<Guid>;