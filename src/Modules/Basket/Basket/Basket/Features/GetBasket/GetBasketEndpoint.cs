namespace Basket.Basket.Features.GetBasket
{
    public record GetBasketResponse(ShoppingCartDto ShoppingCart);

    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
                {
                    var result = await sender.Send(new GetBasketQuery(userName));
                    var response = result.Adapt<GetBasketResponse>();
                    return Results.Ok(response);
                })
                .Produces<GetBasketResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Get Basket")
                .WithDescription("Get user's basket")
                .RequireAuthorization();
        }
    }
}