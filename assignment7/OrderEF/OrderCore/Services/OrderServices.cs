using Microsoft.EntityFrameworkCore;
using OrderCore.Data;
using OrderCore.Models;

namespace OrderCore.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context) => _context = context;

        // 获取所有商品
        public async Task<List<Goods>> GetAllGoodsAsync() =>
            await _context.Goods.ToListAsync();

        // 获取单个商品
        public async Task<Goods?> GetGoodsByIdAsync(int id) =>
            await _context.Goods.FindAsync(id);

        // 添加商品
        public async Task AddGoodsAsync(Goods goods)
        {
            await _context.Goods.AddAsync(goods);
            await _context.SaveChangesAsync();
        }

        // 添加订单
        public async Task<Order> AddOrderAsync(Order order)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (await _context.Orders.AnyAsync(o => o.OrderId == order.OrderId))
                        throw new ArgumentException($"订单 {order.OrderId} 已存在");
                    
                    // 确保订单有明细
                    if (order.Details.Count == 0)
                        throw new ArgumentException("订单必须包含至少一个商品");
                    
                    // 为明细计算总价
                    foreach (var detail in order.Details)
                    {
                        // 加载商品信息
                        var goods = await _context.Goods.FindAsync(detail.GoodsId);
                        if (goods == null)
                            throw new ArgumentException($"商品ID {detail.GoodsId} 不存在");
                        
                        detail.Goods = goods;
                        detail.CalculateTotal();
                    }

                    // 添加订单和明细
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                    return order;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        // 删除订单
        public async Task DeleteOrderAsync(string orderId)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var order = await _context.Orders
                        .Include(o => o.Details)
                        .FirstOrDefaultAsync(o => o.OrderId == orderId)
                        ?? throw new KeyNotFoundException($"订单 {orderId} 不存在");

                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        // 更新订单
        public async Task UpdateOrderAsync(Order updatedOrder)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var existingOrder = await _context.Orders
                        .Include(o => o.Details)
                        .FirstOrDefaultAsync(o => o.OrderId == updatedOrder.OrderId)
                        ?? throw new KeyNotFoundException($"订单 {updatedOrder.OrderId} 不存在");

                    // 更新订单基本信息
                    _context.Entry(existingOrder).CurrentValues.SetValues(updatedOrder);

                    // 更新明细
                    foreach (var detail in updatedOrder.Details)
                    {
                        // 加载商品信息
                        var goods = await _context.Goods.FindAsync(detail.GoodsId);
                        if (goods == null)
                            throw new ArgumentException($"商品ID {detail.GoodsId} 不存在");
                        
                        detail.Goods = goods;
                        detail.CalculateTotal();
                        
                        var existingDetail = existingOrder.Details
                            .FirstOrDefault(d => d.GoodsId == detail.GoodsId);

                        if (existingDetail != null)
                        {
                            // 更新现有明细
                            _context.Entry(existingDetail).CurrentValues.SetValues(detail);
                        }
                        else
                        {
                            // 添加新明细
                            existingOrder.Details.Add(detail);
                        }
                    }

                    // 删除不再存在的明细
                    var removedDetails = existingOrder.Details
                        .Where(ed => !updatedOrder.Details.Any(ud => ud.GoodsId == ed.GoodsId))
                        .ToList();

                    foreach (var detail in removedDetails)
                    {
                        _context.OrderDetails.Remove(detail);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        // 按ID获取订单
        public async Task<Order?> GetOrderByIdAsync(string orderId) =>
            await _context.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Goods)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

        // 查询订单
        public async Task<List<Order>> QueryOrdersAsync(Func<Order, bool>? predicate = null)
        {
            var orders = await _context.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Goods)
                .ToListAsync();

            if (predicate != null)
                orders = orders.Where(predicate).ToList();

            return orders.OrderBy(o => o.Details.Sum(d => d.Total)).ToList();
        }
        
        // 按客户名查询订单
        public async Task<List<Order>> QueryOrdersByCustomerAsync(string customerName)
        {
            return await _context.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Goods)
                .Where(o => o.Customer.Contains(customerName))
                .ToListAsync();
        }
        
        // 按商品名称查询订单
        public async Task<List<Order>> QueryOrdersByProductNameAsync(string productName)
        {
            return await _context.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Goods)
                .Where(o => o.Details.Any(d => d.Goods != null && d.Goods.Name.Contains(productName)))
                .ToListAsync();
        }
        
        // 按金额范围查询订单
        public async Task<List<Order>> QueryOrdersByAmountRangeAsync(decimal min, decimal max)
        {
            var orders = await _context.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Goods)
                .ToListAsync();
                
            return orders
                .Where(o => o.Details.Sum(d => d.Total) >= min && o.Details.Sum(d => d.Total) <= max)
                .ToList();
        }
    }
}
