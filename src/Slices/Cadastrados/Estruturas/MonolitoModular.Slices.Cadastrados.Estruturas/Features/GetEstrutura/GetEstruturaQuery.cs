using System;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.GetEstrutura
{
    public class GetEstruturaQuery : IRequest<Estrutura>
    {
        public Guid Codigo { get; set; }
    }
}
