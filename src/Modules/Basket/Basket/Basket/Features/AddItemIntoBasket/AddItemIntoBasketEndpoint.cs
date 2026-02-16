namespace Basket.Basket.Features.AddItemIntoBasket
{
    public record AddItemIntoBasketRequest(ShoppingCartItemDto ShoppingCartItem);

    public record AddItemIntoBasketResponse(Guid Id);

    public class AddItemIntoBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket/{userName}/items",
                    async ([FromRoute] string userName, [FromBody] AddItemIntoBasketRequest request, ISender sender) =>
                    {
                        var command = new AddItemIntoBasketCommand(userName, request.ShoppingCartItem);
                        var result = await sender.Send(command);
                        var response = result.Adapt<AddItemIntoBasketResponse>();
                        return Results.Ok(response);
                    })
                .Produces<AddItemIntoBasketResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithSummary("Add item into Basket")
                .WithDescription("Add an item to the user's basket.")
                .RequireAuthorization();
        }
    }
}