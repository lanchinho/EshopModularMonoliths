namespace Basket.Basket.Features.RemoveItemFromBasket;

public record RemoveItemFromBasketCommand(string UserName, Guid ProductId)
	: ICommand<RemoveItemFromBasketResult>;

public record RemoveItemFromBasketResult(Guid Id);

public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
{
	public RemoveItemFromBasketCommandValidator()
	{
		RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
		RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
	}
}

internal class RemoveItemFromBasketHandler(BasketDbContext context)
	: ICommandHandler<RemoveItemFromBasketCommand, RemoveItemFromBasketResult>
{
	public async Task<RemoveItemFromBasketResult> Handle(RemoveItemFromBasketCommand command, CancellationToken cancellationToken)
	{
		var basket = await context.ShoppingCarts
			.Include(x => x.Items)
			.FirstOrDefaultAsync(x => x.UserName == command.UserName, cancellationToken)
			?? throw new BasketNotFoundException(command.UserName);

		basket.RemoveItem(command.ProductId);		
		await context.SaveChangesAsync(cancellationToken);

		return new RemoveItemFromBasketResult(basket.Id);
	}
}
