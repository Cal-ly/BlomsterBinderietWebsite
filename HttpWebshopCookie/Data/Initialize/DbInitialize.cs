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
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                context.Products.AddRange(MockProducts.ProductCatalog);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
        context.SaveChanges();

    }
}