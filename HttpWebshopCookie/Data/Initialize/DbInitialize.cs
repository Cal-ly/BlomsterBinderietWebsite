namespace HttpWebshopCookie.Data;
public static class DbInitialize
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();
        if (context.Products.Any())
        {
            return;
        }

        context.Products.AddRange(MockProducts.ProductCatalog);
        context.SaveChanges();
    }
}