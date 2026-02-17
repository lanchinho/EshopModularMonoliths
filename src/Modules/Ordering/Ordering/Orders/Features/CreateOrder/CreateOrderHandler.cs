using FluentValidation;
using FluentValidation.Validators;

namespace Ordering.Orders.Features.CreateOrder;

public record CreateOrderCommand(OrderDto Order) : ICommand<CreateOrderResult>;

public record CreateOrderResult(Guid OrderId);

public class CreateORderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateORderCommandValidator()
    {
        RuleFor(x => x.Order.OrderName)
            .NotEmpty()
            .WithMessage("OrderName is required");
    }
}

internal class CreateOrderHandler(OrderingDbContext context)
    : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = CreateNewOrder(command.Order);
        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);
        return new(order.Id);
    }


    private static Order CreateNewOrder(OrderDto orderDto)
    {
        var shippingAddress = Address.Of(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName,
            orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine,
            orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);

        var billingAddress = Address.Of(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName,
            orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country,
            orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);

        var newOrder = Order.Create(
            id: Guid.NewGuid(),
            customerId: orderDto.CustomerId,
            orderName: $"{orderDto.OrderName}_{new Random().Next()}",
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            payment: Payment.Of(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration,
                orderDto.Payment.Cvv, orderDto.Payment.PaymentMethod)
        );

        orderDto.Items.ForEach(item =>
        {
            newOrder.Add(
                item.ProductId,
                item.Quantity,
                item.Price);
        });

        return newOrder;
    }
}