using Shared.Messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly;
var orderingAssembly = typeof(OrderingModule).Assembly;

builder.Services
    .AddCarterWithAssemblies(catalogAssembly, basketAssembly)
    .AddMediatRWithAssemblies(catalogAssembly, basketAssembly, orderingAssembly)
    .AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
    })
    .AddMassTransitWithAssemblies(catalogAssembly, basketAssembly)
    .AddCatalogModule(builder.Configuration)
    .AddBasketModule(builder.Configuration)
    .AddOrderingModule(builder.Configuration)
    .AddOpenApi()
    .AddExceptionHandler<CustomexceptionHandler>()
    .AddProblemDetails();

var app = builder.Build();

app.MapCarter();

app.MapOpenApi()
    .CacheOutput();

app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar/v1"))
    .ExcludeFromDescription();

app
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule()
    .UseSerilogRequestLogging()
    .UseExceptionHandler();

app.Run();