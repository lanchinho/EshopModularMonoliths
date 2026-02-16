namespace Catalog.Products.Features.GetProductById;

internal class GetProductByIdHandler(CatalogDbContext dbContext)
	: IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
	public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
	{
		var product = await dbContext.Products
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == query.ProductId, cancellationToken);

		return new GetProductByIdResult(product.Adapt<ProductDto>());
	}
}
