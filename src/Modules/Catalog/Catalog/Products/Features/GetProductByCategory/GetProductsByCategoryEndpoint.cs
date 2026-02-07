namespace Catalog.Products.Features.GetProductByCategory;

public record GetProductsByCategoryResponse(IEnumerable<ProductDto> Products);

public class GetProductsByCategoryEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/products/category/{category}", async(string category, ISender sender) =>
		{
			var result = await sender.Send(new GetProductByCategoryQuery(category));
			var response = result.Adapt<GetProductsByCategoryResponse>();
			return Results.Ok(response);
		})
		.WithName("GetProductByCategory")
		.Produces<GetProductsByCategoryResponse>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.WithSummary("Get Product by category")
		.WithDescription("Returns a list of products by their category name");
	}
}
