namespace Catalog.Products.Features.GetProducts;

public record GetProductsResponse(IEnumerable<ProductDto> Products);

public class GetProductsEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/products", async (ISender sender) =>
		{
			var result = await sender.Send(new GetProductsQuery());
			var response = result.Adapt<GetProductsResponse>();
			return Results.Ok(response);
		})
		.WithName("GetProducts")
		.Produces<GetProductsResponse>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError)
		.WithSummary("Get Products")
		.WithDescription("Retrieves a list of all products in the catalog");
	}
}
