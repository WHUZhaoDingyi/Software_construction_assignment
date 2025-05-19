using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderCore
{
    public class OrderService
    {
        private readonly List<Order> orders = new List<Order>();

        public void AddOrder(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (orders.Contains(order)) throw new ArgumentException($"订单 {order.OrderId} 已存在");
            if (!order.Details.Any()) throw new ArgumentException("订单必须包含至少一个商品");
            orders.Add(order);
        }

        public void RemoveOrder(string orderId)
        {
            var order = GetOrderById(orderId);
            orders.Remove(order);
        }

        public void UpdateOrder(Order newOrder)
        {
            if (newOrder == null) throw new ArgumentNullException(nameof(newOrder));
            
            var index = orders.FindIndex(o => o.OrderId == newOrder.OrderId);
            if (index == -1) throw new KeyNotFoundException($"订单 {newOrder.OrderId} 不存在");
            orders[index] = newOrder;
        }

        public Order GetOrderById(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("订单号不能为空", nameof(orderId));
                
            var order = orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
                throw new KeyNotFoundException($"订单 {orderId} 不存在");
                
            return order;
        }

        // 按订单号查询
        public IEnumerable<Order> QueryByOrderId(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentException("订单号不能为空", nameof(orderId));
                
            return orders
                .Where(o => o.OrderId.Contains(orderId))
                .OrderBy(o => o.Total);
        }

        // 按客户查询
        public IEnumerable<Order> QueryByCustomer(string customer)
        {
            if (string.IsNullOrWhiteSpace(customer))
                throw new ArgumentException("客户名不能为空", nameof(customer));
                
            return orders
                .Where(o => o.Customer.Contains(customer))
                .OrderBy(o => o.Total);
        }

        // 按商品名称查询
        public IEnumerable<Order> QueryByProductName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("商品名不能为空", nameof(productName));
                
            return orders
                .Where(o => o.Details.Any(d => 
                    (d.Item?.Name?.Contains(productName) ?? false) || 
                    (d.ItemName?.Contains(productName) ?? false)
                ))
                .OrderBy(o => o.Total);
        }

        // 按金额范围查询
        public IEnumerable<Order> QueryByAmountRange(decimal min, decimal max)
        {
            if (min < 0)
                throw new ArgumentException("最小金额不能小于0", nameof(min));
            if (max < min)
                throw new ArgumentException("最大金额不能小于最小金额", nameof(max));
                
            return orders
                .Where(o => o.Total >= min && o.Total <= max)
                .OrderBy(o => o.Total);
        }

        // 通用查询
        public IEnumerable<Order> QueryOrders(Func<Order, bool>? predicate = null) => 
            (predicate == null ? orders : orders.Where(predicate)).OrderBy(o => o.Total);

        // 获取所有订单
        public List<Order> GetAllOrders() => orders.ToList();

        // 排序方法 - 默认按订单号排序
        public void SortOrders(Comparison<Order>? comparison = null) => 
            orders.Sort(comparison ?? ((x, y) => string.Compare(x.OrderId, y.OrderId)));
    }
}