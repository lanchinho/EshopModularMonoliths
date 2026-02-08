namespace Basket.Basket.Dtos;

public record ShoppingCartItemDto(Guid Id, Guid ProductId, int Quantity, string Color, decimal Price, string ProductName);
