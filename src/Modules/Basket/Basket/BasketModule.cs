namespace Basket;

public static class BasketModule
{
	public static IServiceCollection AddBasketModule(this IServiceCollection services,
		IConfiguration configuration)
	{
		return services;
	}
	
	public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
	{
		return app;
	}
}
