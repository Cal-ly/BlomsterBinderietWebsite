using HttpWebshopCookie.Data;
using HttpWebshopCookie.Models;
using HttpWebshopCookie.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HttpWebshopCookie.Tests
{
    [TestClass]
    public class OrderServiceTests
    {
        private Mock<ApplicationDbContext>? _mockContext;
        private OrderService? _orderService;
        private Mock<DbSet<Order>>? _mockOrderDbSet;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockOrderDbSet = new Mock<DbSet<Order>>();

            // Setup the OrderService with the mock context
            _orderService = new OrderService(_mockContext.Object);
        }

        [TestMethod]
        public void GetOrderItems_ShouldReturnOrderItems()
        {
            // Arrange
            var order = new Order
            {
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductItem = new Product { Name = "Product1", Price = 10 }, Quantity = 2 },
                    new OrderItem { ProductItem = new Product { Name = "Product2", Price = 20 }, Quantity = 1 }
                }
            };

            // Act
            var result = _orderService!.GetOrderItems(order);

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Product1", result[0].ProductName);
            Assert.AreEqual(2, result[0].Quantity);
            Assert.AreEqual(10, result[0].Price);
        }

        [TestMethod]
        public void GetTotalPriceString_ShouldReturnFormattedPrice()
        {
            // Arrange
            var order = new Order
            {
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { UnitPrice = 50m, Quantity = 2 },
                    new OrderItem { UnitPrice = 23.45m, Quantity = 1 }
                }
            };

            // Act
            var result = _orderService!.GetTotalPriceString(order);

            // Assert
            var expectedTotal = 50m * 2 + 23.45m;
            string? expectedTotalString = expectedTotal.ToString("C");
            Assert.AreEqual(expectedTotalString, result);
        }
    }
}
