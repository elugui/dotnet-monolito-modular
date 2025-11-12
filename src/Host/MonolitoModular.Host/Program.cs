using MonolitoModular.Shared.Contracts;
using MonolitoModular.Slices.Users;
using MonolitoModular.Slices.Products;
using MonolitoModular.Slices.Users.Grpc;
using MonolitoModular.Slices.Products.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map gRPC services for inter-slice communication
app.MapGrpcService<UsersGrpcService>();
app.MapGrpcService<ProductsGrpcService>();

app.Run();
