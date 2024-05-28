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

        //[TestMethod]
        //public void GetOrder_ShouldReturnOrder_WhenOrderExists()
        //{
        //    // Arrange
        //    var orderId = "test-order";
        //    var order = new Order { Id = orderId };
        //    var orders = new List<Order> { order }.AsQueryable();

        //    _mockOrderDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
        //    _mockOrderDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
        //    _mockOrderDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
        //    _mockOrderDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

        //    _mockContext.Setup(c => c.Orders).Returns(_mockOrderDbSet.Object);

        //    // Act
        //    var result = _orderService.GetOrder(orderId);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(orderId, result.Id);
        //}

        //[TestMethod]
        //public void GetOrder_ShouldReturnNull_WhenOrderDoesNotExist()
        //{
        //    // Arrange
        //    var orderId = "non-existent-order";
        //    _mockContext.Setup(m => m.Orders.Find(orderId)).Returns((Order)null);

        //    // Act
        //    var result = _orderService.GetOrder(orderId);

        //    // Assert
        //    Assert.IsNull(result);
        //}

        //[TestMethod]
        //public void UpdateOrderStatus_ShouldUpdateStatus_WhenOrderExists()
        //{
        //    // Arrange
        //    var orderId = "test-order";
        //    var order = new Order { Id = orderId, Status = OrderStatus.Pending };
        //    _mockContext.Setup(m => m.Orders.Find(orderId)).Returns(order);

        //    // Act
        //    _orderService.UpdateOrderStatus(orderId, OrderStatus.Completed);

        //    // Assert
        //    Assert.AreEqual(OrderStatus.Completed, order.Status);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void UpdateOrderStatus_ShouldThrowException_WhenOrderDoesNotExist()
        //{
        //    // Arrange
        //    var orderId = "non-existent-order";
        //    _mockContext.Setup(m => m.Orders.Find(orderId)).Returns((Order)null);

        //    // Act
        //    _orderService.UpdateOrderStatus(orderId, OrderStatus.Completed);
        //}

        //[TestMethod]
        //public void DeleteOrder_ShouldRemoveOrder_WhenOrderExists()
        //{
        //    // Arrange
        //    var orderId = "test-order";
        //    var order = new Order { Id = orderId };
        //    _mockContext.Setup(m => m.Orders.Find(orderId)).Returns(order);
        //    _mockContext.Setup(m => m.Orders.Remove(order));

        //    // Act
        //    _orderService.DeleteOrder(orderId);

        //    // Assert
        //    _mockContext.Verify(m => m.Orders.Remove(order), Times.Once);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void DeleteOrder_ShouldThrowException_WhenOrderDoesNotExist()
        //{
        //    // Arrange
        //    var orderId = "non-existent-order";
        //    _mockContext.Setup(m => m.Orders.Find(orderId)).Returns((Order)null);

        //    // Act
        //    _orderService.DeleteOrder(orderId);
        //}
    }
}
