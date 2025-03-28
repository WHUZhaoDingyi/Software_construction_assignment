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
            if (orders.Contains(order)) throw new ArgumentException($"订单 {order.OrderId} 已存在");
            if (order.Details.Distinct().Count() != order.Details.Count) 
                throw new ArgumentException("存在重复订单明细");
            orders.Add(order);
        }

        public void RemoveOrder(string orderId)
        {
            var order = orders.FirstOrDefault(o => o.OrderId == orderId) ?? 
                throw new KeyNotFoundException($"订单 {orderId} 不存在");
            orders.Remove(order);
        }

        public void UpdateOrder(Order newOrder)
        {
            var index = orders.FindIndex(o => o.OrderId == newOrder.OrderId);
            if (index == -1) throw new KeyNotFoundException($"订单 {newOrder.OrderId} 不存在");
            orders[index] = newOrder;
        }

        public IEnumerable<Order> QueryOrders(Func<Order, bool> predicate = null) => 
            (predicate == null ? orders : orders.Where(predicate)).OrderBy(o => o.Total);

        public void SortOrders(Comparison<Order> comparison = null) => 
            orders.Sort(comparison ?? ((x, y) => string.Compare(x.OrderId, y.OrderId)));
    }
}