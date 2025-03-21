using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderManagement.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderService service;
        private Order testOrder;

        [SetUp]
        public void Setup()
        {
            service = new OrderService();
            testOrder = new Order("TEST001") { Customer = "Test" };
            testOrder.Details.Add(new OrderDetails
            {
                Item = new Goods { Name = "商品", Price = 10 },
                Quantity = 2
            });
        }

        [Test]
        public void AddOrder_ShouldThrowWhenDuplicate()
        {
            service.AddOrder(testOrder);
            Assert.Throws<ArgumentException>(() => service.AddOrder(testOrder));
        }

        [Test]
        public void RemoveOrder_ShouldThrowWhenNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() => service.RemoveOrder("INVALID"));
        }

        [Test]
        public void QueryOrders_ShouldReturnCorrectResults()
        {
            service.AddOrder(testOrder);
            var result = service.QueryOrders(o => o.OrderId == "TEST001").ToList();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].OrderId, Is.EqualTo("TEST001"));
        }

        
    [Test]
    public void QueryOrders_ShouldReturnEmptyForNonExistingOrder()
    {
        var result = service.QueryOrders(o => o.OrderId == "INVALID").ToList();
        Assert.That(result.Count, Is.EqualTo(0));
    }

    }
}



