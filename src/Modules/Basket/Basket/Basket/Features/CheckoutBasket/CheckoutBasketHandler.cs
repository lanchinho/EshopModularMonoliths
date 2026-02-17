namespace Basket.Basket.Features.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckout)
    : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckout).NotNull().WithMessage("BasketCheckout can´t be null");
        RuleFor(x => x.BasketCheckout.UserName).NotEmpty()
            .WithMessage("UserName is required");
    }
}

public class CheckoutBasketHandler(BasketDbContext context)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var basket = await context.ShoppingCarts
                .AsNoTracking()
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.UserName == command.BasketCheckout.UserName, cancellationToken);

            if (basket is null) throw new BasketNotFoundException(command.BasketCheckout.UserName);

            var eventMsg = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
            eventMsg.TotalPrice = basket.TotalPrice;

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = typeof(BasketCheckoutIntegrationEvent).AssemblyQualifiedName ?? string.Empty,
                Content = JsonSerializer.Serialize(eventMsg),
                OcurredOn = DateTime.UtcNow
            };

            context.OutboxMessages.Add(outboxMessage);
            context.ShoppingCarts.Remove(basket);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new CheckoutBasketResult(true);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            return new CheckoutBasketResult(false);
        }
    }
}