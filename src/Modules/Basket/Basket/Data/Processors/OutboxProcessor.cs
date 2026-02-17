using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basket.Data.Processors;

public class OutboxProcessor(
    IServiceProvider serviceProvider,
    IBus bus,
    ILogger<OutboxProcessor> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbcontext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();

                var outboxMsg = await dbcontext.OutboxMessages
                    .Where(m => m.ProcessedOn == null)
                    .ToListAsync(stoppingToken);

                foreach (var msg in outboxMsg)
                {
                    var eventType = Type.GetType(msg.Type);
                    if (eventType == null)
                    {
                        logger.LogWarning("Could not resolve type: {Type}", msg.Type);
                        continue;
                    }

                    var eventMessage = JsonSerializer.Deserialize(msg.Content, eventType);
                    if (eventMessage == null)
                    {
                        logger.LogWarning("Could not resolve message: {Content}", msg.Content);
                        continue;
                    }

                    await bus.Publish(eventMessage, stoppingToken);
                    msg.ProcessedOn = DateTime.UtcNow;
                    logger.LogInformation("Successfully processed outbox message with ID: {Id}", msg.Id);
                }
                
                await dbcontext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error processing the outbox messages");
                throw;
            }
            
            //Adjust the delay...
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}