var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
	config.ReadFrom.Configuration(context.Configuration));

builder.Services
	.AddCarterWithAssemblies(typeof(CatalogModule).Assembly, typeof(BasketModule).Assembly)
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
