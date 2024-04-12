namespace HttpWebshopCookie.Data;

public static class MockProducts
{
    public static List<Product> ProductCatalog { get; set; } =
    [
        new Product
        {
            Name = "Product 1",
            Description = "Description of product 1",
            Price = 100
        },
        new Product
        {
            Name = "Product 2",
            Description = "Description of product 2",
            Price = 200
        },
        new Product
        {
            Name = "Product 3",
            Description = "Description of product 3",
            Price = 300
        },
        new Product
        {
            Name = "Product 4",
            Description = "Description of product 4",
            Price = 400
        },
        new Product
        {
            Name = "Product 5",
            Description = "Description of product 5",
            Price = 500
        }
    ];
}
