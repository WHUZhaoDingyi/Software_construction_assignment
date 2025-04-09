using Microsoft.EntityFrameworkCore;
using OrderCore.Data;
using OrderCore.Models;
using OrderCore.Services;

var context = new AppDbContext();

// 数据库迁移
await context.Database.MigrateAsync();

var service = new OrderService(context);

// 初始化商品数据
if (!await context.Goods.AnyAsync())
{
    context.Goods.AddRange(
        new Goods { Name = "手机", Price = 2999m },
        new Goods { Name = "耳机", Price = 399m },
        new Goods { Name = "充电器", Price = 99m }
    );
    await context.SaveChangesAsync();
}

while (true)
{
    Console.Clear();
    Console.WriteLine("=== 订单管理系统 ===");
    Console.WriteLine("1. 创建新订单");
    Console.WriteLine("2. 查询所有订单");
    Console.WriteLine("3. 删除订单");
    Console.WriteLine("4. 退出系统");
    Console.Write("请选择操作: ");

    switch (Console.ReadLine())
    {
        case "1":
            await CreateOrderAsync(service);
            break;
        case "2":
            await ListOrdersAsync(service);
            break;
        case "3":
            await DeleteOrderAsync(service);
            break;
        case "4":
            return;
        default:
            Console.WriteLine("无效输入，请重新选择");
            Thread.Sleep(1000);
            break;
    }
}

async Task CreateOrderAsync(OrderService service)
{
    var order = new Order { OrderId = GenerateOrderId() };
    
    Console.Write("请输入客户名称: ");
    order.Customer = Console.ReadLine();

    var goodsList = await service.GetAllGoodsAsync();
    Console.WriteLine("\n可选商品列表:");
    foreach (var goods in goodsList)
    {
        Console.WriteLine($"[{goods.Id}] {goods.Name} - 单价: {goods.Price:C}");
    }

    while (true)
    {
        Console.Write("\n输入商品编号 (0完成): ");
        if (!int.TryParse(Console.ReadLine(), out var goodsId) || goodsId == 0) break;

        var selectedGoods = goodsList.FirstOrDefault(g => g.Id == goodsId);
        if (selectedGoods == null)
        {
            Console.WriteLine("无效商品编号");
            continue;
        }

        Console.Write("输入购买数量: ");
        if (!int.TryParse(Console.ReadLine(), out var quantity) || quantity <= 0)
        {
            Console.WriteLine("无效数量");
            continue;
        }

        order.Details.Add(new OrderDetail 
        { 
            GoodsId = selectedGoods.Id,
            Quantity = quantity
        });
    }

    if (order.Details.Count == 0)
    {
        Console.WriteLine("订单未添加商品，已取消");
        return;
    }

    try
    {
        await service.AddOrderAsync(order);
        Console.WriteLine($"\n订单创建成功! 订单号: {order.OrderId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n错误: {ex.Message}");
    }
    Console.WriteLine("按任意键继续...");
    Console.ReadKey();
}

async Task ListOrdersAsync(OrderService service)
{
    var orders = await service.QueryOrdersAsync();
    
    Console.WriteLine("\n=== 订单列表 ===");
    foreach (var order in orders)
    {
        Console.WriteLine($"订单号: {order.OrderId}");
        Console.WriteLine($"客户: {order.Customer}");
        Console.WriteLine("商品明细:");
        foreach (var detail in order.Details)
        {
            Console.WriteLine($"- {detail.Goods.Name} ×{detail.Quantity} 小计: {detail.Total:C}");
        }
        Console.WriteLine($"总金额: {order.Details.Sum(d => d.Total):C}\n");
    }
    Console.WriteLine("按任意键继续...");
    Console.ReadKey();
}

async Task DeleteOrderAsync(OrderService service)
{
    Console.Write("\n请输入要删除的订单号: ");
    var orderId = Console.ReadLine();

    try
    {
        await service.DeleteOrderAsync(orderId);
        Console.WriteLine("订单删除成功");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"错误: {ex.Message}");
    }
    Console.WriteLine("按任意键继续...");
    Console.ReadKey();
}

string GenerateOrderId() => 
    $"{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
