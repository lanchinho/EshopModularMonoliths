namespace Basket.Basket.Features.CreateBasket;

public record CreateBasketRequest(ShoppingCartDto ShoppingCart);

public record CreateBasketResponse(Guid Id);

public class CreateBasketEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("/basket", async (ShoppingCartDto shoppingCartDto, ISender sender) =>
		{
			var command = shoppingCartDto.Adapt<CreateBasketCommand>();
			var result = await sender.Send(command);		
			return Results.Created($"/basket/{result.Id}", result.Adapt<CreateBasketResponse>());
		})
		.Produces<CreateBasketResponse>(StatusCodes.Status201Created)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status500InternalServerError)
	    .WithSummary("Create Basket")
		.WithDescription("Create a basket for a user with its products");
	}
}
