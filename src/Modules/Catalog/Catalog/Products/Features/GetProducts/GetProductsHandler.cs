namespace Catalog.Products.Features.GetProducts
{
	public record GetProductsQuery()
		: IQuery<GetProductsResult>;

	public record GetProductsResult(IEnumerable<ProductDto> Products);

	internal class GetProductsHandler(CatalogDbContext dbContext)
		: IQueryHandler<GetProductsQuery, GetProductsResult>
	{
		public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
		{
			var products = await dbContext.Products
				.AsNoTracking()
				.OrderBy(p => p.Name)
				.ToListAsync(cancellationToken);
			
			return new GetProductsResult(products.Adapt<List<ProductDto>>());
		}
	}
}
