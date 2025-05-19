using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderCore
{
    public class Order : IEquatable<Order>
    {
        public string OrderId { get; }
        public required string Customer { get; set; } = "";  // 非空属性必须初始化
        public DateTime CreateTime { get; }
        public List<OrderDetails> Details { get; init; } = new List<OrderDetails>();  // 允许初始化赋值
        public decimal Total => Details.Sum(d => d.Total);

        public Order(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("订单号不能为空", nameof(id));
            OrderId = id;
            CreateTime = DateTime.Now;
        }

        public void AddDetails(OrderDetails detail)
        {
            if (detail == null)
                throw new ArgumentNullException(nameof(detail), "订单明细不能为空");
            if (detail.Quantity <= 0)
                throw new ArgumentException("商品数量必须大于0");
            if (detail.Item == null)
                throw new ArgumentException("商品不能为空");
            
            if (Details.Contains(detail))
                throw new ArgumentException($"订单明细 {detail.Item.Name} 已存在");
            
            // 设置明细ID
            detail.Id = Details.Count > 0 ? Details.Max(d => d.Id) + 1 : 1;
            Details.Add(detail);
        }

        public void RemoveDetails(int detailId)
        {
            var detail = Details.FirstOrDefault(d => d.Id == detailId);
            if (detail == null)
                throw new KeyNotFoundException($"未找到ID为 {detailId} 的订单明细");
            
            Details.Remove(detail);
        }

        public void RemoveDetails(Goods item)
        {
            var detail = Details.FirstOrDefault(d => d.Item?.Equals(item) == true);
            if (detail == null)
                throw new KeyNotFoundException($"未找到商品 {item.Name} 的订单明细");
            
            Details.Remove(detail);
        }

        // 处理可为空参数
        public bool Equals(Order? other) => other is not null && OrderId == other.OrderId;
        public override bool Equals(object? obj) => obj is Order other && Equals(other);
        public override int GetHashCode() => OrderId.GetHashCode();
        
        public override string ToString() =>
            $"订单号：{OrderId}\n" +
            $"客户：{Customer}\n" +
            $"创建时间：{CreateTime:yyyy-MM-dd HH:mm:ss}\n" +
            $"总金额：¥{Total:F2}\n" +
            $"明细：\n{string.Join("\n", Details.Select(d => $"  {d}"))}";

        public Order Clone() => new Order(OrderId)
        {
            Customer = this.Customer,
            // 使用 init 属性允许初始化赋值
            Details = this.Details.Select(d => new OrderDetails
            {
                Id = d.Id,
                Item = d.Item != null ? new Goods 
                { 
                    Id = d.Item.Id, 
                    Name = d.Item.Name, 
                    Price = d.Item.Price 
                } : null,
                ItemId = d.ItemId,
                ItemName = d.ItemName,
                ItemPrice = d.ItemPrice,
                Quantity = d.Quantity
            }).ToList()
        };
    }
}