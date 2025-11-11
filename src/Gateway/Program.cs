using ModularMonolith.Modules.Customers.API.Extensions;
using ModularMonolith.Modules.Products.API.Extensions;
using ModularMonolith.Shared.Application.Behaviors;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddApplicationPart(typeof(ModularMonolith.Modules.Customers.API.Controllers.CustomersController).Assembly)
    .AddApplicationPart(typeof(ModularMonolith.Modules.Products.API.Controllers.ProductsController).Assembly);

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Modular Monolith API", Version = "v1" });
});

// Register MediatR Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Add Modules
builder.Services.AddCustomersModule(builder.Configuration);
builder.Services.AddProductsModule(builder.Configuration);

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Modular Monolith API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

app.Run();
