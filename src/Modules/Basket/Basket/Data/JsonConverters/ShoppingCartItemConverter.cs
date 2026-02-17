using System.Text.Json;
using System.Text.Json.Serialization;

namespace Basket.Data.JsonConverters;

public class ShoppingCartItemConverter : JsonConverter<ShoppingCartItem>
{
    public override ShoppingCartItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDocument = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDocument.RootElement;

        var id = rootElement.GetProperty("id").GetGuid();
        var shoppingCartId = rootElement.GetProperty("shoppingCartId").GetGuid();
        var productid = rootElement.GetProperty("productid").GetGuid();
        var productName = rootElement.GetProperty("productName").GetString();
        var color = rootElement.GetProperty("color").GetString();
        var quantity = rootElement.GetProperty("quantity").GetInt32();
        var price = rootElement.GetProperty("price").GetDecimal();

        return new ShoppingCartItem(id, shoppingCartId, productid, quantity, color ?? string.Empty, price,
            productName ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, ShoppingCartItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id.ToString());
        writer.WriteString("shoppingCartId", value.ShoppingCartId.ToString());
        writer.WriteString("productid", value.ProductId.ToString());
        writer.WriteString("productName", value.ProductName);
        writer.WriteString("color", value.Color);
        writer.WriteNumber("quantity", value.Quantity);
        writer.WriteNumber("price", value.Price);
        
        writer.WriteEndObject();
    }
}