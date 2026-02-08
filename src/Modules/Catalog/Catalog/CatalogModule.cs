using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Behaviors;

namespace Catalog;

public static class CatalogModule
{
	public static IServiceCollection AddCatalogModule(this IServiceCollection services,
		IConfiguration configuration)
	{
		var assembly = Assembly.GetExecutingAssembly();
		services.AddMediatR(config =>
		{
			config.RegisterServicesFromAssembly(assembly);
			config.AddOpenBehaviors([typeof(ValidationBehavior<,>), typeof(LoggingBehavior<,>)]);
		})
		.AddValidatorsFromAssembly(assembly);

		var connectionString = configuration.GetConnectionString("Database");

		services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
		services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

		services.AddDbContext<CatalogDbContext>((sp, options) =>
		{
			options.AddInterceptors(sp.GetService<ISaveChangesInterceptor>());
			options.UseNpgsql(connectionString);
		});

		services.AddScoped<IDataSeeder, CatalogDataSeeder>();

		return services;
	}

	public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
	{
		app.UseMigration<CatalogDbContext>();
		return app;
	}
}