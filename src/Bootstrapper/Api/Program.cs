using Scalar.AspNetCore;
using Shared.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddCarterWithAssemblies(typeof(CatalogModule).Assembly);

builder.Services
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
	.UseOrderingModule();

app.UseExceptionHandler();

app.Run();
