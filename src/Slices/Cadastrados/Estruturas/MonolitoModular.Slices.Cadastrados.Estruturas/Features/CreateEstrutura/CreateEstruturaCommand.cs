using MediatR;
using System;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.CreateEstrutura
{
    public class CreateEstruturaCommand : IRequest<Guid>
    {
        public string Nome { get; set; } = string.Empty;
        public decimal EstruturaTipoCodigo { get; set; }
        public string CodigoExterno { get; set; } = string.Empty;
        public DateTime InicioVigencia { get; set; }
        public DateTime TerminoVigencia { get; set; }
        public int Versao { get; set; }
        public int Status { get; set; }
    }
}
