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
        private Goods testGoods;

        [SetUp]
        public void Setup()
        {
            service = new OrderService();
            testGoods = new Goods { Name = "测试商品", Price = 100 };
            
            testOrder = new Order("TEST001") { Customer = "测试客户" };
            testOrder.Details.Add(new OrderDetails 
            { 
                Item = testGoods, 
                Quantity = 2 
            });
        }

        // 添加订单测试
        [Test]
        public void AddOrder_ShouldAddValidOrder()
        {
            service.AddOrder(testOrder);
            var result = service.QueryOrders(o => o.OrderId == "TEST001").ToList();
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddOrder_ShouldThrowWhenDuplicateOrder()
        {
            service.AddOrder(testOrder);
            Assert.Throws<ArgumentException>(() => service.AddOrder(testOrder));
        }

        [Test]
        public void AddOrder_ShouldThrowWhenDuplicateDetails()
        {
            var order = new Order("TEST002");
            order.Details.AddRange(new[] {
                new OrderDetails { Item = testGoods, Quantity = 1 },
                new OrderDetails { Item = testGoods, Quantity = 1 } // 重复明细
            });
            
            Assert.Throws<ArgumentException>(() => service.AddOrder(order));
        }

        // 删除订单测试
        [Test]
        public void RemoveOrder_ShouldRemoveExistingOrder()
        {
            service.AddOrder(testOrder);
            service.RemoveOrder("TEST001");
            Assert.That(service.QueryOrders().Count(), Is.EqualTo(0));
        }

        [Test]
        public void RemoveOrder_ShouldThrowWhenOrderNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() => service.RemoveOrder("INVALID"));
        }

        // 修改订单测试
        [Test]
        public void UpdateOrder_ShouldUpdateCustomerSuccessfully()
        {
            service.AddOrder(testOrder);
            var newOrder = new Order("TEST001") { Customer = "新客户" };
            newOrder.Details.Add(testOrder.Details.First());
            
            service.UpdateOrder(newOrder);
            var updated = service.QueryOrders(o => o.OrderId == "TEST001").First();
            Assert.That(updated.Customer, Is.EqualTo("新客户"));
        }

        [Test]
        public void UpdateOrder_ShouldThrowWhenOrderNotFound()
        {
            var invalidOrder = new Order("INVALID");
            Assert.Throws<KeyNotFoundException>(() => service.UpdateOrder(invalidOrder));
        }

        // 查询测试
        [Test]
        public void QueryOrders_ShouldReturnEmptyForNoMatches()
        {
            service.AddOrder(testOrder);
            var result = service.QueryOrders(o => o.OrderId == "NOT_EXIST");
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void QueryOrders_ShouldSortByTotalAmount()
        {
            var order1 = CreateOrder("A001", total: 500);
            var order2 = CreateOrder("A002", total: 300);
            service.AddOrder(order1);
            service.AddOrder(order2);
            
            var result = service.QueryOrders().ToList();
            Assert.That(result[0].OrderId, Is.EqualTo("A002"));
            Assert.That(result[1].OrderId, Is.EqualTo("A001"));
        }

        [Test]
        public void QueryOrders_ShouldFilterByItemName()
        {
            service.AddOrder(testOrder);
            var result = service.QueryOrders(o => 
                o.Details.Any(d => d.Item.Name.Contains("测试")));
            
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        // 排序测试
        [Test]
        public void SortOrders_ShouldSortByIdByDefault()
        {
            var order3 = CreateOrder("A003");
            var order1 = CreateOrder("A001");
            service.AddOrder(order3);
            service.AddOrder(order1);
            
            service.SortOrders();
            var result = service.QueryOrders().ToList();
            Assert.That(result[0].OrderId, Is.EqualTo("A001"));
            Assert.That(result[1].OrderId, Is.EqualTo("A003"));
        }

        [Test]
        public void SortOrders_ShouldUseCustomComparison()
        {
            var order1 = CreateOrder("A001", total: 200);
            var order2 = CreateOrder("A002", total: 500);
            service.AddOrder(order1);
            service.AddOrder(order2);
            
            // 按总金额降序排序
            service.SortOrders((x, y) => y.Total.CompareTo(x.Total));
            var result = service.QueryOrders().ToList();
            Assert.That(result[0].OrderId, Is.EqualTo("A001"));
            Assert.That(result[1].OrderId, Is.EqualTo("A002")); 
        }

        // ToString测试
        [Test]
        public void ToString_ShouldFormatOrderCorrectly()
        {
            var expected = 
                "订单号：TEST001\n客户：测试客户\n总金额：¥200\n" +
                "明细：测试商品(¥100) ×2 小计：¥200\n\n";
            
            Assert.That(testOrder.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ToString_ShouldFormatDetailsCorrectly()
        {
            var detail = new OrderDetails 
            { 
                Item = testGoods, 
                Quantity = 3 
            };
            
            Assert.That(detail.ToString(), 
                Is.EqualTo("测试商品(¥100) ×3 小计：¥300"));
        }

        // 辅助方法
        private Order CreateOrder(string id, decimal total = 0)
        {
            var order = new Order(id);
            if (total > 0)
            {
                order.Details.Add(new OrderDetails 
                { 
                    Item = new Goods { Price = total }, 
                    Quantity = 1 
                });
            }
            return order;
        }
    }
}