using System;
using OrderManagement.Domain;
using OrderManagement.Service;

namespace OrderManagement.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            using (var context = new OrderDbContext())
            {
                var service = new OrderService(context);

                try
                {
                    // 添加订单
                    var order = new Order
                    {
                        OrderNumber = "001",
                        Customer = "张三",
                        OrderDetails = new List<OrderDetails>
                        {
                            new OrderDetails { ProductName = "商品1", Amount = 100 }
                        }
                    };
                    service.AddOrder(order);

                    // 查询订单
                    var orders = service.QueryOrders(o => o.Customer == "张三");
                    foreach (var o in orders)
                    {
                        Console.WriteLine(o);
                    }

                    // 修改订单
                    order.Customer = "李四";
                    service.UpdateOrder(order);

                    // 删除订单
                    service.DeleteOrder(order.Id);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}    