namespace Ordering.Orders.Features.GetOders;

public record GetOrdersResponse(PaginatedResult<OrderDto> Orders);

public class GetOrdersEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetOrdersQuery(request));
                var response = result.Adapt<GetOrdersResponse>();
                return response.Orders.Count == 0 ? Results.NoContent() : Results.Ok(response);
            })
            .WithName("GetOrders")
            .Produces<GetOrdersResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get Orders")
            .WithDescription("Gets all placed orders")
            .RequireAuthorization();
    }
}