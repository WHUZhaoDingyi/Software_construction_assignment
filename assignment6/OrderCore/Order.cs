using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderCore
{
    public class Order : IEquatable<Order>
    {
        public string OrderId { get; }
        public required string Customer { get; set; } = "";  // 非空属性必须初始化
        public List<OrderDetails> Details { get; init; } = new List<OrderDetails>();  // 允许初始化赋值
        public decimal Total => Details.Sum(d => d.Total);

        public Order(string id) => OrderId = id;

        // 处理可为空参数
        public bool Equals(Order? other) => other is not null && OrderId == other.OrderId;
        public override bool Equals(object? obj) => obj is Order other && Equals(other);
        public override int GetHashCode() => OrderId.GetHashCode();
        
        public override string ToString() =>
            $"订单号：{OrderId}\n客户：{Customer}\n总金额：¥{Total}\n明细：" + 
            string.Join("\n  ", Details) + "\n";

        public Order Clone() => new Order(OrderId)
        {
            Customer = this.Customer,
            // 使用 init 属性允许初始化赋值
            Details = this.Details.Select(d => new OrderDetails
            {
                Item = new Goods { Name = d.Item.Name, Price = d.Item.Price },
                Quantity = d.Quantity
            }).ToList()
        };
    }
}