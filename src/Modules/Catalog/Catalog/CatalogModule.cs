using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Catalog;

public static class CatalogModule
{
	public static IServiceCollection AddCatalogModule(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddMediatR(config =>
		{
			config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
		});

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