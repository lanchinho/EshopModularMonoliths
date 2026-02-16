using MassTransit;
using Shared.Messaging.Events;

namespace Catalog.EventHandlers;

public class ProductPriceChangedEventHandler(IBus bus, ILogger<ProductPriceChangedEventHandler> logger)
    : INotificationHandler<ProductPriceChangedEvent>
{
    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
        var integrationEvent = new ProductPriceChangedIntegrationEvent
        {
            ProductId = notification.Product.Id,
            Name = notification.Product.Name,
            Price = notification.Product.Price,
            Description = notification.Product.Description,
            Category = notification.Product.Category,
            ImageFile = notification.Product.ImageFile
        };
        
        await bus.Publish(integrationEvent, cancellationToken);
    }
}