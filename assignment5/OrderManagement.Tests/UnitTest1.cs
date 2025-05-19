using NUnit.Framework;
using OrderManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderManagement.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderService service;
        private Goods goods1;
        private Goods goods2;
        private Order testOrder;

        [SetUp]
        public void Setup()
        {
            service = new OrderService();
            
            // 创建测试商品
            goods1 = new Goods { Id = "G001", Name = "测试商品1", Price = 100M };
            goods2 = new Goods { Id = "G002", Name = "测试商品2", Price = 200M };
            
            // 创建测试订单
            testOrder = new Order("TEST001") { Customer = "测试客户" };
            testOrder.AddDetails(new OrderDetails { Item = goods1, Quantity = 2 });
        }

        // 添加订单测试
        [Test]
        public void AddOrder_ShouldAddValidOrder()
        {
            // 添加有效订单
            service.AddOrder(testOrder);
            
            // 确认订单被添加
            var result = service.QueryOrders(o => o.OrderId == "TEST001").ToList();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Customer, Is.EqualTo("测试客户"));
        }

        [Test]
        public void AddOrder_ShouldThrowWhenDuplicateOrder()
        {
            // 添加订单
            service.AddOrder(testOrder);
            
            // 重复添加同一订单 - 应抛出异常
            Assert.Throws<ArgumentException>(() => service.AddOrder(testOrder));
        }

        [Test]
        public void AddOrder_ShouldThrowWhenDuplicateDetails()
        {
            // 创建包含重复明细的订单
            var order = new Order("TEST002") { Customer = "测试客户2" };
            order.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            
            // 添加相同商品 - 应抛出异常
            Assert.Throws<ArgumentException>(() => order.AddDetails(new OrderDetails { Item = goods1, Quantity = 2 }));
        }

        [Test]
        public void AddOrder_ShouldThrowForEmptyOrder()
        {
            // 创建没有明细的订单
            var emptyOrder = new Order("TEST003") { Customer = "测试客户3" };
            
            // 添加空订单 - 应抛出异常
            Assert.Throws<ArgumentException>(() => service.AddOrder(emptyOrder));
        }

        // 删除订单测试
        [Test]
        public void RemoveOrder_ShouldRemoveExistingOrder()
        {
            // 添加订单
            service.AddOrder(testOrder);
            
            // 删除订单
            service.RemoveOrder("TEST001");
            
            // 确认订单已被删除
            var result = service.QueryOrders().ToList();
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveOrder_ShouldThrowWhenOrderNotFound()
        {
            // 删除不存在的订单 - 应抛出异常
            Assert.Throws<KeyNotFoundException>(() => service.RemoveOrder("INVALID"));
        }

        // 修改订单测试
        [Test]
        public void UpdateOrder_ShouldUpdateCustomerSuccessfully()
        {
            // 添加订单
            service.AddOrder(testOrder);
            
            // 创建更新后的订单
            var newOrder = new Order("TEST001") { Customer = "新客户" };
            newOrder.AddDetails(new OrderDetails { Item = goods1, Quantity = 2 });
            
            // 更新订单
            service.UpdateOrder(newOrder);
            
            // 确认订单已更新
            var updated = service.QueryOrders(o => o.OrderId == "TEST001").First();
            Assert.That(updated.Customer, Is.EqualTo("新客户"));
        }

        [Test]
        public void UpdateOrder_ShouldThrowWhenOrderNotFound()
        {
            // 更新不存在的订单 - 应抛出异常
            var invalidOrder = new Order("INVALID") { Customer = "无效客户" };
            invalidOrder.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            Assert.Throws<KeyNotFoundException>(() => service.UpdateOrder(invalidOrder));
        }

        // 查询测试
        [Test]
        public void QueryOrders_ShouldReturnEmptyForNoMatches()
        {
            // 添加测试订单
            service.AddOrder(testOrder);
            
            // 查询不匹配的订单
            var result = service.QueryOrders(o => o.OrderId == "NOT_EXIST");
            
            // 确认结果为空
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void QueryOrders_ShouldSortByTotalAmount()
        {
            // 创建并添加多个测试订单
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

        // 按订单号查询测试
        [Test]
        public void QueryByOrderId_ShouldReturnMatchingOrders()
        {
            // 创建并添加多个测试订单
            var order1 = new Order("ORDER-001") { Customer = "客户A" };
            order1.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            
            var order2 = new Order("ORDER-002") { Customer = "客户B" };
            order2.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            
            var order3 = new Order("OTHER-001") { Customer = "客户C" };
            order3.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            
            service.AddOrder(order1);
            service.AddOrder(order2);
            service.AddOrder(order3);
            
            // 按订单号查询
            var result = service.QueryByOrderId("ORDER").ToList();
            
            // 确认查询结果
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(o => o.OrderId == "ORDER-001"), Is.True);
            Assert.That(result.Any(o => o.OrderId == "ORDER-002"), Is.True);
        }

        // 按客户查询测试
        [Test]
        public void QueryByCustomer_ShouldReturnMatchingOrders()
        {
            // 创建并添加多个测试订单
            var order1 = new Order("ORD001") { Customer = "张三" };
            order1.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            
            var order2 = new Order("ORD002") { Customer = "张四" };
            order2.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            
            var order3 = new Order("ORD003") { Customer = "李五" };
            order3.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            
            service.AddOrder(order1);
            service.AddOrder(order2);
            service.AddOrder(order3);
            
            // 按客户名查询
            var result = service.QueryByCustomer("张").ToList();
            
            // 确认查询结果
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(o => o.Customer == "张三"), Is.True);
            Assert.That(result.Any(o => o.Customer == "张四"), Is.True);
        }

        // 按商品名称查询测试
        [Test]
        public void QueryByProductName_ShouldReturnMatchingOrders()
        {
            // 准备商品
            var phone = new Goods { Id = "P001", Name = "手机", Price = 2999M };
            var laptop = new Goods { Id = "P002", Name = "笔记本电脑", Price = 5999M };
            var headset = new Goods { Id = "P003", Name = "耳机", Price = 299M };
            
            // 创建并添加多个测试订单
            var order1 = new Order("ORD001") { Customer = "客户A" };
            order1.AddDetails(new OrderDetails { Item = phone, Quantity = 1 });
            
            var order2 = new Order("ORD002") { Customer = "客户B" };
            order2.AddDetails(new OrderDetails { Item = laptop, Quantity = 1 });
            
            var order3 = new Order("ORD003") { Customer = "客户C" };
            order3.AddDetails(new OrderDetails { Item = headset, Quantity = 1 });
            order3.AddDetails(new OrderDetails { Item = phone, Quantity = 1 });
            
            service.AddOrder(order1);
            service.AddOrder(order2);
            service.AddOrder(order3);
            
            // 按商品名查询
            var result = service.QueryByProductName("手机").ToList();
            
            // 确认查询结果
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(o => o.OrderId == "ORD001"), Is.True);
            Assert.That(result.Any(o => o.OrderId == "ORD003"), Is.True);
        }

        // 按金额范围查询测试
        [Test]
        public void QueryByAmountRange_ShouldReturnMatchingOrders()
        {
            // 创建并添加多个测试订单
            var order1 = new Order("ORD001") { Customer = "客户A" };
            order1.AddDetails(new OrderDetails { Item = goods1, Quantity = 5 }); // 500元
            
            var order2 = new Order("ORD002") { Customer = "客户B" };
            order2.AddDetails(new OrderDetails { Item = goods1, Quantity = 2 }); // 200元
            
            var order3 = new Order("ORD003") { Customer = "客户C" };
            order3.AddDetails(new OrderDetails { Item = goods2, Quantity = 2 }); // 400元
            
            service.AddOrder(order1);
            service.AddOrder(order2);
            service.AddOrder(order3);
            
            // 按金额范围查询
            var result = service.QueryByAmountRange(300, 600).ToList();
            
            // 确认查询结果
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(o => o.OrderId == "ORD003"), Is.True); // 400元
            Assert.That(result.Any(o => o.OrderId == "ORD001"), Is.True); // 500元
        }

        // 排序测试
        [Test]
        public void SortOrders_ShouldSortByIdByDefault()
        {
            // 创建并添加多个测试订单
            var order3 = new Order("ORD003") { Customer = "客户C" };
            order3.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            
            var order1 = new Order("ORD001") { Customer = "客户A" };
            order1.AddDetails(new OrderDetails { Item = goods1, Quantity = 1 });
            
            service.AddOrder(order3);
            service.AddOrder(order1);
            
            // 默认排序
            service.SortOrders();
            var result = service.GetAllOrders();
            
            // 确认排序结果
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].OrderId, Is.EqualTo("ORD001"));
            Assert.That(result[1].OrderId, Is.EqualTo("ORD003"));
        }

        [Test]
        public void SortOrders_ShouldUseCustomComparison()
        {
            // 创建并添加多个测试订单
            var order1 = new Order("ORD001") { Customer = "客户A" };
            order1.AddDetails(new OrderDetails { Item = goods1, Quantity = 5 }); // 500元
            
            var order2 = new Order("ORD002") { Customer = "客户B" };
            order2.AddDetails(new OrderDetails { Item = goods1, Quantity = 2 }); // 200元
            
            service.AddOrder(order1);
            service.AddOrder(order2);
            
            // 自定义排序（按金额降序）
            service.SortOrders((x, y) => y.Total.CompareTo(x.Total));
            var result = service.GetAllOrders();
            
            // 确认排序结果
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].OrderId, Is.EqualTo("ORD001")); // 500元
            Assert.That(result[1].OrderId, Is.EqualTo("ORD002")); // 200元
        }

        // ToString测试
        [Test]
        public void ToString_ShouldFormatOrderCorrectly()
        {
            // 创建测试订单和明细
            var order = new Order("ORD001") { Customer = "张三" };
            var goods = new Goods { Id = "G001", Name = "商品A", Price = 100M };
            order.AddDetails(new OrderDetails { Item = goods, Quantity = 2 });
            
            // 获取订单的字符串表示
            var str = order.ToString();
            
            // 确认包含关键信息
            Assert.That(str.Contains("订单号：ORD001"), Is.True);
            Assert.That(str.Contains("客户：张三"), Is.True);
            Assert.That(str.Contains("总金额：¥200.00"), Is.True);
            Assert.That(str.Contains("商品A"), Is.True);
        }

        // 辅助方法
        private Order CreateOrder(string id, decimal total = 0)
        {
            var order = new Order(id) { Customer = "测试客户" };
            if (total > 0)
            {
                var amount = (int)total / 100;
                order.AddDetails(new OrderDetails 
                { 
                    Item = new Goods { Id = $"G-{Guid.NewGuid()}", Name = $"商品{amount}", Price = 100 },
                    Quantity = amount
                });
            }
            return order;
        }
    }
}