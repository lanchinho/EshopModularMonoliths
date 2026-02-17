using MassTransit;
using Ordering.Orders.Features.CreateOrder;
using Shared.Messaging.Events;

namespace Ordering.Orders.EventHandlers;

public class BasketCheckoutIntegrationEventHandler(
    ISender sender,
    ILogger<BasketCheckoutIntegrationEventHandler> logger) : IConsumer<BasketCheckoutIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
    {
        logger.LogInformation("Integration Event handled {IntegrationEvent}", context.Message.GetType().Name);

        var createOrderCmd = MapToCreateOrderCommand(context.Message);
        await sender.Send(createOrderCmd);
    }

    private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutIntegrationEvent message)
    {
        var addressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress, message.AddressLine,
            message.Country, message.State, message.ZipCode);

        var paymentDto = new PaymentDto(message.CardName, message.CardNumber, message.Expiration, message.Cvv,
            message.PaymentMethod);

        var orderId = Guid.NewGuid();

        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId: message.CustomerId,
            OrderName: message.UserName,
            ShippingAddress: addressDto,
            BillingAddress: addressDto,
            Payment: paymentDto,
            Items:
            [
                //TODO: remover hardcoded ...
                new(orderId, new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"), 2, 1200.00m),
                new(orderId, new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27"), 1, 1250.00m)
            ]);

        return new CreateOrderCommand(orderDto);
    }
}