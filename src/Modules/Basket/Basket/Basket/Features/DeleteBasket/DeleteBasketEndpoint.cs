namespace Basket.Basket.Features.DeleteBasket
{
    public record DeleteBasketResponse(bool IsSuccess);

    internal class DeleteBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{userName}", async (string userName, ISender sender) =>
                {
                    var result = await sender.Send(new DeleteBasketCommand(userName));
                    var response = result.Adapt<DeleteBasketResponse>();
                    return Results.Ok(response);
                })
                .Produces<DeleteBasketResponse>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithSummary("Delete Basket")
                .WithDescription("Delete a basket with its products by user name.")
                .RequireAuthorization();
        }
    }
}