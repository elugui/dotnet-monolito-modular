using System;
using MediatR;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.DeleteEstrutura
{
    public class DeleteEstruturaCommand : IRequest<bool>
    {
        public Guid Codigo { get; set; }
    }
}
