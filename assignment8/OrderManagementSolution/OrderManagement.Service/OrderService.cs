using System;
using System.Collections.Generic;
using System.Linq;
using OrderManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace OrderManagement.Service
{
    public class OrderService
    {
        private readonly OrderDbContext _context;

        public OrderService(OrderDbContext context)
        {
            _context = context;
        }

        public void AddOrder(Order order)
        {
            if (_context.Orders.Any(o => o.OrderNumber == order.OrderNumber))
            {
                throw new ArgumentException("订单号已存在，不能重复添加。");
            }
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void DeleteOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                throw new ArgumentException("订单不存在，删除失败。");
            }
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            var existingOrder = _context.Orders.Find(order.Id);
            if (existingOrder == null)
            {
                throw new ArgumentException("订单不存在，修改失败。");
            }
            existingOrder.OrderNumber = order.OrderNumber;
            existingOrder.Customer = order.Customer;
            existingOrder.OrderDetails = order.OrderDetails;
            _context.SaveChanges();
        }

        public List<Order> QueryOrders(Func<Order, bool> predicate)
        {
            return _context.Orders
                           .Include(o => o.OrderDetails)
                           .Where(predicate)
                           .OrderBy(o => o.TotalAmount)
                           .ToList();
        }

        public List<Order> SortOrders(Func<Order, object> keySelector = null)
        {
            if (keySelector == null)
            {
                return _context.Orders
                               .Include(o => o.OrderDetails)
                               .OrderBy(o => o.OrderNumber)
                               .ToList();
            }
            return _context.Orders
                           .Include(o => o.OrderDetails)
                           .OrderBy(keySelector)
                           .ToList();
        }
    }
}    