namespace HttpWebshopCookie.Services;

public class ProductService
{
    private readonly ApplicationDbContext _context;
    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }
    public List<Product> GetProducts()
    {
        return _context.Products.ToList();
    }
    public Product GetProductById(string id)
    {
        Product? product = _context.Products.FirstOrDefault(p => p.Id == id);
        return product ?? throw new Exception("Product not found");
    }
    public void AddProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }
    public void UpdateProduct(Product product)
    {
        _context.Products.Update(product);
        _context.SaveChanges();
    }
    public void DeleteProduct(string id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}
