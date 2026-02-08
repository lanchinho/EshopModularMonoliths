using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Data;
using Shared.Data.Interceptors;

namespace Basket;

public static class BasketModule
{
	public static IServiceCollection AddBasketModule(this IServiceCollection services,
		IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("Database");

		services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
		services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

		services.AddDbContext<BasketDbContext>((sp, options) =>
		{
			options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
			options.UseNpgsql(connectionString);
		});

		return services;
	}

	public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
	{
		app.UseMigration<BasketDbContext>();

		return app;
	}
}
