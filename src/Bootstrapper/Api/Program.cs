using Api.SchemeTransformers;
using Keycloak.AuthServices.Authentication;
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
    .AddMassTransitWithAssemblies(builder.Configuration, catalogAssembly, basketAssembly)
    .AddCatalogModule(builder.Configuration)
    .AddBasketModule(builder.Configuration)
    .AddOrderingModule(builder.Configuration)
    .AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); })
    .AddExceptionHandler<CustomexceptionHandler>()
    .AddProblemDetails()
    .AddAuthorization()
    .AddKeycloakWebApiAuthentication(builder.Configuration);

var app = builder.Build();

app.MapCarter();

app.MapOpenApi()
    .CacheOutput();

app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar/v1"))
    .ExcludeFromDescription();

app
    .UseAuthentication()
    .UseAuthorization()
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule()
    .UseSerilogRequestLogging()
    .UseExceptionHandler();

app.Run();