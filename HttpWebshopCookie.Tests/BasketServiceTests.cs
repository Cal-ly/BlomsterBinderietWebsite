using HttpContextMoq;
using HttpWebshopCookie.Data;
using HttpWebshopCookie.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HttpWebshopCookie.Tests
{
    [TestClass]
    public class BasketServiceTests
    {
        private Mock<OrderService>? _mockOrderService;
        private ApplicationDbContext? _context;
        private HttpContextMock? _httpContextMock;
        private HttpContextAccessorMock? _httpContextAccessorMock;

        [TestInitialize]
        public void Setup()
        {
            // Setup InMemory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);

            // Setup HttpContextMock
            _httpContextMock = new HttpContextMock();
            _httpContextAccessorMock = new HttpContextAccessorMock();

            // Configure Session
            var sessionMock = new Mock<ISession>();
            _httpContextMock.Session = sessionMock.Object;

            // Setup Mock OrderService
            _mockOrderService = new Mock<OrderService>(_context);
        }

        //[TestMethod]
        //public void GetOrCreateBasket_ShouldCreateNewBasketIfNotExists()
        //{
        //    // Arrange
        //    var basketService = new BasketService(_httpContextAccessorMock!, _context!, _mockOrderService!.Object);
        //    _httpContextAccessorMock!.HttpContext.Response.Cookies.Append("BasketId", string.Empty);

        //    // Act
        //    var basket = basketService?.GetOrCreateBasket(_httpContextAccessorMock, _context!);

        //    // Assert
        //    Assert.IsNotNull(basket);
        //    Assert.AreEqual(1, _context!.Baskets.Count());
        //    bool cookieExists = _httpContextAccessorMock.HttpContext.Request.Cookies.TryGetValue("BasketId", out string? cookieValue);
        //    Assert.IsFalse(cookieExists);
        //    Assert.IsTrue(string.IsNullOrEmpty(cookieValue));
        //}

        [TestCleanup]
        public void Cleanup()
        {
            _context!.Baskets.RemoveRange(_context.Baskets);
            _context.Products.RemoveRange(_context.Products);
            _context.SaveChanges();
        }
    }
}