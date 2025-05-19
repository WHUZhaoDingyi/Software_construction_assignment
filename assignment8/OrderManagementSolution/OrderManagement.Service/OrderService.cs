using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain;

namespace OrderManagement.Service
{
    public class OrderService
    {
        private readonly OrderDbContext _context;

        public OrderService(OrderDbContext context)
        {
            _context = context;
        }

        // 添加订单
        public async Task AddOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "订单不能为空");

            if (_context.Orders.Any(o => o.OrderNumber == order.OrderNumber))
                throw new ArgumentException($"订单号 {order.OrderNumber} 已存在，不能重复添加");

            if (string.IsNullOrEmpty(order.Customer))
                throw new ArgumentException("客户名称不能为空");

            if (order.OrderDetails == null || !order.OrderDetails.Any())
                throw new ArgumentException("订单必须包含至少一个商品");

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        // 同步版本的添加订单（兼容原有代码）
        public void AddOrder(Order order)
        {
            AddOrderAsync(order).GetAwaiter().GetResult();
        }

        // 删除订单
        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new ArgumentException($"订单ID {orderId} 不存在，删除失败");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        // 同步版本的删除订单（兼容原有代码）
        public void DeleteOrder(int orderId)
        {
            DeleteOrderAsync(orderId).GetAwaiter().GetResult();
        }

        // 更新订单
        public async Task UpdateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "订单不能为空");

            var existingOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null)
                throw new ArgumentException($"订单ID {order.Id} 不存在，更新失败");

            // 检查订单号是否已被其他订单使用
            if (existingOrder.OrderNumber != order.OrderNumber &&
                await _context.Orders.AnyAsync(o => o.OrderNumber == order.OrderNumber && o.Id != order.Id))
                throw new ArgumentException($"订单号 {order.OrderNumber} 已被其他订单使用");

            // 更新基本属性
            existingOrder.OrderNumber = order.OrderNumber;
            existingOrder.Customer = order.Customer;

            // 更新订单明细
            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);
            existingOrder.OrderDetails.Clear();

            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                foreach (var detail in order.OrderDetails)
                {
                    existingOrder.OrderDetails.Add(new OrderDetails
                    {
                        ProductName = detail.ProductName,
                        Amount = detail.Amount
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        // 同步版本的更新订单（兼容原有代码）
        public void UpdateOrder(Order order)
        {
            UpdateOrderAsync(order).GetAwaiter().GetResult();
        }

        // 查询所有订单
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .OrderBy(o => o.OrderNumber)
                .ToListAsync();
        }

        // 根据ID查询订单
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // 根据订单号查询订单
        public async Task<Order> GetOrderByNumberAsync(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
                throw new ArgumentException("订单号不能为空");

            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        // 根据客户名称查询订单
        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerName)
        {
            if (string.IsNullOrEmpty(customerName))
                throw new ArgumentException("客户名称不能为空");

            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Customer.Contains(customerName))
                .OrderBy(o => o.OrderNumber)
                .ToListAsync();
        }

        // 根据金额范围查询订单
        public async Task<List<Order>> GetOrdersByAmountRangeAsync(decimal minAmount, decimal maxAmount)
        {
            if (minAmount < 0)
                throw new ArgumentException("最小金额不能小于0");

            if (maxAmount < minAmount)
                throw new ArgumentException("最大金额不能小于最小金额");

            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.TotalAmount >= minAmount && o.TotalAmount <= maxAmount)
                .OrderBy(o => o.TotalAmount)
                .ToListAsync();
        }

        // 根据条件查询订单（内存中处理）
        public List<Order> QueryOrders(Func<Order, bool> predicate)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .AsEnumerable() // 将查询结果加载到内存中
                .Where(predicate)
                .OrderBy(o => o.TotalAmount)
                .ToList();
        }

        // 排序订单
        public List<Order> SortOrders(Func<Order, object> keySelector = null)
        {
            var query = _context.Orders.Include(o => o.OrderDetails);

            if (keySelector == null)
            {
                return query
                    .OrderBy(o => o.OrderNumber)
                    .ToList();
            }
            
            return query
                .AsEnumerable() // 将查询结果加载到内存中
                .OrderBy(keySelector)
                .ToList();
        }
    }
}    