using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.CreateEstrutura;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Tests
{
    public class CreateEstruturaHandlerTests
    {
        [Fact]
        public async Task Should_Throw_When_Nome_Is_Empty()
        {
            var handler = new CreateEstruturaHandler();
            var command = new CreateEstruturaCommand
            {
                Nome = "",
                EstruturaTipoCodigo = 1,
                InicioVigencia = DateTime.Now,
                TerminoVigencia = DateTime.Now.AddDays(1),
                Status = EstruturaStatus.Ativo
            };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_When_EstruturaTipoCodigo_Is_Zero()
        {
            var handler = new CreateEstruturaHandler();
            var command = new CreateEstruturaCommand
            {
                Nome = "Teste",
                EstruturaTipoCodigo = 0,
                InicioVigencia = DateTime.Now,
                TerminoVigencia = DateTime.Now.AddDays(1),
                Status = EstruturaStatus.Ativo
            };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_When_TerminoVigencia_Less_Than_InicioVigencia()
        {
            var handler = new CreateEstruturaHandler();
            var command = new CreateEstruturaCommand
            {
                Nome = "Teste",
                EstruturaTipoCodigo = 1,
                InicioVigencia = DateTime.Now,
                TerminoVigencia = DateTime.Now.AddDays(-1),
                Status = EstruturaStatus.Ativo
            };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_When_Status_Is_Invalid()
        {
            var handler = new CreateEstruturaHandler();
            var command = new CreateEstruturaCommand
            {
                Nome = "Teste",
                EstruturaTipoCodigo = 1,
                InicioVigencia = DateTime.Now,
                TerminoVigencia = DateTime.Now.AddDays(1),
                Status = (EstruturaStatus)99
            };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
