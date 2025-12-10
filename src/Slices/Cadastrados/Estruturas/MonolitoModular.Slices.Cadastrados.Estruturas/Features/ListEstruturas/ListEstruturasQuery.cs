using System.Collections.Generic;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.ListEstruturas
{
    public class ListEstruturasQuery : IRequest<List<Estrutura>>
    {
        // Filtros opcionais podem ser adicionados aqui
    }
}
