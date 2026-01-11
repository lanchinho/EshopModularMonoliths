using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basket;

public static class BasketModule
{
	public static IServiceCollection AddBasketModule(this IServiceCollection services,
		IConfiguration configuration)
	{
		return services;
	}
}
