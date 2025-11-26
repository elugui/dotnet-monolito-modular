using MonolitoModular.Shared.Contracts;
using MonolitoModular.Slices.Users;
using MonolitoModular.Slices.Products;
using MonolitoModular.Slices.Users.Grpc;
using MonolitoModular.Slices.Products.Grpc;
using MonolitoModular.Slices.Cadastrados.Estruturas.Grpc;
using Scalar.AspNetCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// configure Kestrel to listen on specific ports for HTTP/1.1 and HTTP/2 (gRPC)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5033, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1);
    options.ListenAnyIP(5000, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// No topo do Program.cs, na área de builder.Services
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // Informações Básicas
        document.Info.Title = "API BIPPER";
        document.Info.Version = "v1.0";
        
        // Descrição Detalhada (Suporta Markdown!)
        document.Info.Description = 
            "Esta API permite gerir endpoints da aplicação BIPPER.\n\n" +
            "### Funcionalidades:\n" +
            "- Em desenvolvimento\n\n" +
            "> **Nota:** Projeto em desenvolvimento.";

        // Informações de Contacto
        document.Info.Contact = new OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Email = "dev@bipper.com",
            Url = new Uri("https://bipper.com/developers")
        };

        // Termos de Serviço e Licença
        document.Info.TermsOfService = new Uri("https://bipper.com/termos");
        document.Info.License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        };

        return Task.CompletedTask;
    });
});

// Register slice modules
var sliceModules = new ISliceModule[]
{
    new UsersModule(),
    new ProductsModule()
};

foreach (var module in sliceModules)
{
    module.RegisterServices(builder.Services, builder.Configuration);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTheme(ScalarTheme.Moon);
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map gRPC services for inter-slice communication
app.MapGrpcService<UsersGrpcService>();
app.MapGrpcService<ProductsGrpcService>();
app.MapGrpcService<EstruturasGrpcService>();

app.Run();
