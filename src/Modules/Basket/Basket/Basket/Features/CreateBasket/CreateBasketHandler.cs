namespace Basket.Basket.Features.CreateBasket;

public record CreateBasketCommand(ShoppingCartDto ShoppingCart)
    : ICommand<CreateBasketResult>;

public record CreateBasketResult(Guid Id);

public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
{
    public CreateBasketCommandValidator()
    {
        RuleFor(x => x.ShoppingCart.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

internal class CreateBasketHandler(IBasketRepository repository) :
    ICommandHandler<CreateBasketCommand, CreateBasketResult>
{
    public async Task<CreateBasketResult> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = CreateNewBasket(command.ShoppingCart);
        await repository.CreateBasket(shoppingCart, cancellationToken);
        return new(shoppingCart.Id);
    }

    private static ShoppingCart CreateNewBasket(ShoppingCartDto shoppingCartDto)
    {
        var newBaket = ShoppingCart.Create(Guid.NewGuid(), shoppingCartDto.UserName);
        shoppingCartDto.Items.ForEach(item =>
        {
            newBaket.AddItem(item.ProductId, item.Quantity, item.Color, item.Price, item.ProductName);
        });

        return newBaket;
    }
}