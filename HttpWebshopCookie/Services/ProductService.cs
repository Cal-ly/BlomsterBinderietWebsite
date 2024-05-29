namespace HttpWebshopCookie.Services
{
    /// <summary>
    /// Service class for managing products.
    /// </summary>
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when the context is null.</exception>
        public ProductService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null.");
        }

        /// <summary>
        /// Gets the list of all products.
        /// </summary>
        /// <returns>The list of products.</returns>
        /// <exception cref="InvalidOperationException">Thrown when unable to retrieve products.</exception>
        public List<Product> GetProducts()
        {
            try
            {
                return _context.Products.ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve products.", ex);
            }
        }

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product with the specified ID.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the ID is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the product is not found.</exception>
        public Product GetProductById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "Product ID cannot be null or empty.");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            return product ?? throw new InvalidOperationException("Product not found.");
        }

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="product">The product to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when the product is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when unable to add the product.</exception>
        public void AddProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            }

            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to add product.", ex);
            }
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="product">The product to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when the product is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when unable to update the product.</exception>
        public void UpdateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            }

            try
            {
                _context.Products.Update(product);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to update product.", ex);
            }
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when the ID is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when unable to delete the product or product not found.</exception>
        public void DeleteProduct(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "Product ID cannot be null or empty.");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found.");
            }

            try
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to delete product.", ex);
            }
        }
    }
}