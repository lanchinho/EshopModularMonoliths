namespace Basket.Basket.Features.DeleteBasket;

public record DeleteBasketCommand(string UserName)
	: ICommand<DeleteBasketResult>;

public record DeleteBasketResult(bool IsSucess);

internal class DeleteBasketHandler(BasketDbContext context)
	: ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
	public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
	{
		var basketToBeDeleted = await context.ShoppingCarts
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.UserName == command.UserName, cancellationToken)
			?? throw new BasketNotFoundException(command.UserName);

		context.ShoppingCarts.Remove(basketToBeDeleted);
		var result = await context.SaveChangesAsync(cancellationToken) > 0;

		return new DeleteBasketResult(result);
	}
}
