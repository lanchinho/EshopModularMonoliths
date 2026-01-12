namespace Ordering;

public static class OrderingModule
{
	public static IServiceCollection AddOrderingModule(this IServiceCollection services,
		IConfiguration configuration)
	{
		return services;
	}

	public static IApplicationBuilder UseOrderingModule(this IApplicationBuilder app)
	{
		return app;
	}
}
