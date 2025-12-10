using System;
using MediatR;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.UpdateEstrutura
{
    public class UpdateEstruturaCommand : IRequest<bool>
    {
        public Guid Codigo { get; set; }
        public string Nome { get; set; }
        public decimal EstruturaTipoCodigo { get; set; }
        public string CodigoExterno { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime TerminoVigencia { get; set; }
        public int Versao { get; set; }
        public EstruturaStatus Status { get; set; }
    }
}
