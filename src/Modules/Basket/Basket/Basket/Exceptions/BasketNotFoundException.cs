namespace Basket.Basket.Exceptions;

internal class BasketNotFoundException(string userName)
	: NotFoundException("Basket", userName)
{
}
