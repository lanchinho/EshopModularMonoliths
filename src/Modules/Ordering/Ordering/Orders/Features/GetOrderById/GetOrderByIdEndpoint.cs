namespace Ordering.Orders.Features.GetOrderById;

public record GetOrderByIdResponse(OrderDto Order);

public class GetOrderByIdEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetOrderByIdQuery(id));
                var response = result.Adapt<GetOrderByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetOrderById")
            .Produces<GetOrderByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get Order By Id")
            .WithDescription("Get order details by id")
            .RequireAuthorization();
    }
}