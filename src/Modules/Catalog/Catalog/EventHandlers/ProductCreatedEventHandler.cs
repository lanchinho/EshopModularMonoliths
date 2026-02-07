namespace Catalog.EventHandlers;

public class ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
	: INotificationHandler<ProductCreatedEvent>
{
	public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("Domain event handled: {DomainEvnent}", notification.GetType().Name);
		return Task.CompletedTask;
	}
}
