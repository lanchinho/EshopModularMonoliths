namespace Catalog.Products.Features.DeleteProduct;

public record DeleteProductCommand(Guid ProductId)
	: ICommand<DeleteProductResult>;

public record DeleteProductResult(bool IsSuccess);

internal class DeleteProductHandler(CatalogDbContext dbContext)
	: ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
	public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
	{
		var productToDelete = await dbContext.Products
			.FindAsync(command.ProductId, cancellationToken)
			?? throw new Exception($"Product not found: {command.ProductId}");

		dbContext.Products.Remove(productToDelete);
		var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

		return new DeleteProductResult(result);
	}
}
