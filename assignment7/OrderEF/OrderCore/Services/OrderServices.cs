using Microsoft.EntityFrameworkCore;
using OrderCore.Data;
using OrderCore.Models;

namespace OrderCore.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context) => _context = context;

        public async Task<List<Goods>> GetAllGoodsAsync() =>
            await _context.Goods.ToListAsync();

        public async Task AddOrderAsync(Order order)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (await _context.Orders.AnyAsync(o => o.OrderId == order.OrderId))
                        throw new ArgumentException($"订单 {order.OrderId} 已存在");

                    await _context.Orders.AddAsync(order);
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

        public async Task DeleteOrderAsync(string orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Details)
                .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new KeyNotFoundException($"订单 {orderId} 不存在");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

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

                    _context.Entry(existingOrder).CurrentValues.SetValues(updatedOrder);

                    // 更新明细
                    foreach (var detail in updatedOrder.Details)
                    {
                        var existingDetail = existingOrder.Details
                            .FirstOrDefault(d => d.GoodsId == detail.GoodsId);

                        if (existingDetail != null)
                            _context.Entry(existingDetail).CurrentValues.SetValues(detail);
                        else
                            existingOrder.Details.Add(detail);
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

        public async Task<List<Order>> QueryOrdersAsync(Func<Order, bool>? predicate = null)
        {
            var orders = await _context.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Goods)
                .ToListAsync(); // 先执行查询，避免 EF Core 翻译 Total

            if (predicate != null)
                orders = orders.Where(predicate).ToList(); // 在内存中筛选

            return orders.OrderBy(o => o.Details.Sum(d => d.Total)).ToList(); // 在内存中排序
        }
    }
}
