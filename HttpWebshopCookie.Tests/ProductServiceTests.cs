using HttpWebshopCookie.Data;
using HttpWebshopCookie.Models;
using HttpWebshopCookie.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpWebshopCookie.Tests
{
    [TestClass]
    public class ProductServiceTests
    {
        private ApplicationDbContext? _context;
        private ProductService? _productService;

        [TestInitialize]
        public void Setup()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Initialize ProductService with in-memory context
            _productService = new ProductService(_context);
        }

        [TestMethod]
        public void GetProducts_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = "1", Name = "Product1", Description = "Description1", Price = 10.0M },
                new Product { Id = "2", Name = "Product2", Description = "Description2", Price = 20.0M }
            };

            _context!.Products.AddRange(products);
            _context.SaveChanges();

            // Act
            var result = _productService!.GetProducts();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Product1", result[0].Name);
            Assert.AreEqual("Product2", result[1].Name);
        }

        [TestMethod]
        public void GetProductById_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = "1";
            var product = new Product { Id = productId, Name = "Product1", Description = "Description1", Price = 10.0M };
            _context!.Products.Add(product);
            _context.SaveChanges();

            // Act
            var result = _productService!.GetProductById(productId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productId, result.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception), "Product not found")]
        public void GetProductById_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Act
            _productService!.GetProductById("nonexistent-product");
        }

        [TestMethod]
        public void AddProduct_ShouldAddProduct()
        {
            // Arrange
            var product = new Product { Id = "1", Name = "Product1", Description = "Description1", Price = 10.0M };

            // Act
            _productService!.AddProduct(product);

            // Assert
            Assert.AreEqual(1, _context!.Products.Count());
            Assert.AreEqual("Product1", _context.Products.First().Name);
        }

        [TestMethod]
        public void UpdateProduct_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product { Id = "1", Name = "Product1", Description = "Description1", Price = 10.0M };
            _context!.Products.Add(product);
            _context.SaveChanges();

            // Act
            product.Name = "UpdatedProduct";
            _productService!.UpdateProduct(product);

            // Assert
            var updatedProduct = _context.Products.First();
            Assert.AreEqual("UpdatedProduct", updatedProduct.Name);
        }

        [TestMethod]
        public void DeleteProduct_ShouldRemoveProduct_WhenProductExists()
        {
            // Arrange
            var productId = "1";
            var product = new Product { Id = productId, Name = "Product1", Description = "Description1", Price = 10.0M };
            _context!.Products.Add(product);
            _context.SaveChanges();

            // Act
            _productService!.DeleteProduct(productId);

            // Assert
            Assert.AreEqual(0, _context.Products.Count());
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context!.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
