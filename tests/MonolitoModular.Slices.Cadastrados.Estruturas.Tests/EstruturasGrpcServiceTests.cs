using System.Threading.Tasks;
using Xunit;
using Grpc.Net.Client;
using MonolitoModular.Slices.Cadastrados.Estruturas.Grpc;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Tests
{
    public class EstruturasGrpcServiceTests
    {
        [Fact]
        public async Task GetEstrutura_DeveRetornarEstruturaOuVazio()
        {
            // Arrange
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new EstruturasService.EstruturasServiceClient(channel);

            // Act
            var request = new GetEstruturaRequest { Codigo = "1" }; // Ajuste para um código válido existente
            var response = await client.GetEstruturaAsync(request);

            // Assert
            Assert.NotNull(response);
            // Se não existir, Estrutura será null
            // Se existir, Estrutura.Nome não deve ser vazio
        }

        [Fact]
        public async Task ListEstruturas_DeveRetornarLista()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new EstruturasService.EstruturasServiceClient(channel);

            var response = await client.ListEstruturasAsync(new ListEstruturasRequest());

            Assert.NotNull(response);
            Assert.NotNull(response.Estruturas);
            // Pode validar quantidade ou campos conforme necessário
        }
    }
}
