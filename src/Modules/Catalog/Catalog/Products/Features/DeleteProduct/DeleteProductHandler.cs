namespace Catalog.Products.Features.DeleteProduct;

public record DeleteProductCommand(Guid ProductId)
	: ICommand<DeleteProductResult>;

public record DeleteProductResult(bool IsSuccess);

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
	public DeleteProductCommandValidator()
	{
		RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product Id is required");
	}
}

internal class DeleteProductHandler(CatalogDbContext dbContext)
	: ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
	public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
	{
		var productToDelete = await dbContext.Products
			.FindAsync(command.ProductId, cancellationToken)
			?? throw new ProductNotFoundException(command.ProductId);

		dbContext.Products.Remove(productToDelete);
		var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

		return new DeleteProductResult(result);
	}
}
