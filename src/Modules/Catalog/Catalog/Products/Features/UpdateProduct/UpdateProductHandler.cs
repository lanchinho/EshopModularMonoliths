namespace Catalog.Products.Features.UpdateProduct
{
	public record UpdateProductCommand(ProductDto Product)
		: ICommand<UpdateProductResult>;

	public record UpdateProductResult(bool IsSuccess);

	public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
	{
		public UpdateProductCommandValidator()
		{
			RuleFor(x => x.Product.Id).NotEmpty().WithMessage("Id is required");
			RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");			
			RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than zero");
		}
	}

	internal class UpdateProductHandler(CatalogDbContext dbContext)
		: ICommandHandler<UpdateProductCommand, UpdateProductResult>
	{
		public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
		{
			var product = await dbContext.Products
				.FindAsync([command.Product.Id], cancellationToken: cancellationToken)
				?? throw new Exception($"Product with id {command.Product.Id} not found.");

			UpdateProductWithNewValues(product, command.Product);

			dbContext.Products.Update(product);
			var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

			return new UpdateProductResult(result);
		}

		private static void UpdateProductWithNewValues(Product product, ProductDto productDto)
		{
			product.Update(
				name: productDto.Name,
				category: productDto.Category,
				description: productDto.Description,
				imageFile: productDto.ImageFile,
				price: productDto.Price);
		}
	}
}
