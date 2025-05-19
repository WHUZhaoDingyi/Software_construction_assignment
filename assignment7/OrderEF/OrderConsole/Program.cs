using Microsoft.EntityFrameworkCore;
using OrderCore.Data;
using OrderCore.Models;
using OrderCore.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("正在初始化数据库...");

try
{
    // 假设数据库已经存在并配置好
    var context = new AppDbContext();
    
    // 假设数据库已经迁移完成
    Console.WriteLine("假设数据库已经迁移完成，直接使用...");
    
    var service = new OrderService(context);
    
    // 假设初始商品数据已存在
    Console.WriteLine("假设初始商品数据已存在...");
    
    await RunMainMenuAsync(service);
}
catch (Exception ex)
{
    Console.WriteLine($"初始化失败: {ex.Message}");
    Console.WriteLine("请确保MySQL服务已启动并且连接字符串正确。");
    Console.WriteLine("按任意键退出...");
    Console.ReadKey();
}

async Task RunMainMenuAsync(OrderService service)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("=== 基于Entity Framework Core的订单管理系统 ===");
        Console.WriteLine("1. 创建新订单");
        Console.WriteLine("2. 查询订单");
        Console.WriteLine("3. 修改订单");
        Console.WriteLine("4. 删除订单");
        Console.WriteLine("5. 管理商品");
        Console.WriteLine("0. 退出系统");
        Console.Write("请选择操作: ");

        string? choice = Console.ReadLine();
        
        try
        {
            switch (choice)
            {
                case "1":
                    await CreateOrderAsync(service);
                    break;
                case "2":
                    await QueryOrdersMenuAsync(service);
                    break;
                case "3":
                    await UpdateOrderAsync(service);
                    break;
                case "4":
                    await DeleteOrderAsync(service);
                    break;
                case "5":
                    await ManageGoodsAsync(service);
                    break;
                case "0":
                    Console.WriteLine("谢谢使用，再见！");
                    return;
                default:
                    Console.WriteLine("无效输入，请重新选择");
                    await Task.Delay(1000);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
        }
    }
}

async Task CreateOrderAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 创建新订单 ===");
    
    var order = new Order { OrderId = GenerateOrderId() };
    
    Console.WriteLine($"自动生成订单号: {order.OrderId}");
    Console.Write("请输入客户名称: ");
    string? customerName = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(customerName))
    {
        Console.WriteLine("客户名称不能为空！");
        WaitForKey();
        return;
    }
    order.Customer = customerName;

    var goodsList = await service.GetAllGoodsAsync();
    Console.WriteLine("\n可选商品列表:");
    foreach (var goods in goodsList)
    {
        Console.WriteLine($"[{goods.Id}] {goods.Name} - 单价: ¥{goods.Price:N2}");
    }

    bool hasDetails = false;
    while (true)
    {
        Console.Write("\n输入商品编号 (输入0完成订单): ");
        if (!int.TryParse(Console.ReadLine(), out var goodsId) || goodsId == 0) 
            break;

        var selectedGoods = goodsList.FirstOrDefault(g => g.Id == goodsId);
        if (selectedGoods == null)
        {
            Console.WriteLine("无效商品编号，请重新输入");
            continue;
        }

        Console.Write($"输入 {selectedGoods.Name} 的购买数量: ");
        if (!int.TryParse(Console.ReadLine(), out var quantity) || quantity <= 0)
        {
            Console.WriteLine("无效数量，请输入大于0的整数");
            continue;
        }

        try
        {
            var detail = new OrderDetail
            { 
                GoodsId = selectedGoods.Id,
                Goods = selectedGoods,
                Quantity = quantity
            };
            
            detail.CalculateTotal();
            order.Details.Add(detail);
            
            Console.WriteLine($"{selectedGoods.Name} ×{quantity} 已添加到订单");
            hasDetails = true;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"添加商品失败: {ex.Message}");
        }
    }

    if (!hasDetails)
    {
        Console.WriteLine("订单未添加商品，已取消创建");
        WaitForKey();
        return;
    }

    try
    {
        var createdOrder = await service.AddOrderAsync(order);
        Console.WriteLine("\n=== 订单创建成功 ===");
        await PrintOrderDetailsAsync(createdOrder);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n创建订单失败: {ex.Message}");
    }
    
    WaitForKey();
}

async Task UpdateOrderAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 修改订单 ===");
    Console.Write("请输入要修改的订单号: ");
    string? orderId = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(orderId))
    {
        Console.WriteLine("订单号不能为空！");
        WaitForKey();
        return;
    }
    
    try
    {
        var order = await service.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            Console.WriteLine($"未找到订单 {orderId}");
            WaitForKey();
            return;
        }
        
        Console.WriteLine("\n当前订单信息:");
        await PrintOrderDetailsAsync(order);
        
        Console.WriteLine("\n请输入新的客户名称 (留空保持不变):");
        string? newCustomer = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newCustomer))
        {
            order.Customer = newCustomer;
        }
        
        var goodsList = await service.GetAllGoodsAsync();
        
        Console.WriteLine("\n修改订单明细 (当前明细):");
        int i = 1;
        foreach (var detail in order.Details)
        {
            Console.WriteLine($"{i++}. {detail.Goods?.Name ?? "未知商品"} - 数量: {detail.Quantity} - 小计: ¥{detail.Total:N2}");
        }
        
        Console.WriteLine("\n可选操作:");
        Console.WriteLine("1. 添加新商品");
        Console.WriteLine("2. 修改商品数量");
        Console.WriteLine("3. 删除商品");
        Console.WriteLine("4. 完成修改");
        
        bool detailsChanged = false;
        while (true)
        {
            Console.Write("\n请选择操作 (1-4): ");
            string? op = Console.ReadLine();
            
            if (op == "4") break;
            
            switch (op)
            {
                case "1": // 添加新商品
                    Console.WriteLine("\n可选商品列表:");
                    foreach (var goods in goodsList)
                    {
                        Console.WriteLine($"[{goods.Id}] {goods.Name} - 单价: ¥{goods.Price:N2}");
                    }
                    
                    Console.Write("\n输入商品编号: ");
                    if (!int.TryParse(Console.ReadLine(), out var goodsId))
                    {
                        Console.WriteLine("无效输入");
                        continue;
                    }
                    
                    var selectedGoods = goodsList.FirstOrDefault(g => g.Id == goodsId);
                    if (selectedGoods == null)
                    {
                        Console.WriteLine("无效商品编号");
                        continue;
                    }
                    
                    var existingDetail = order.Details.FirstOrDefault(d => d.GoodsId == goodsId);
                    if (existingDetail != null)
                    {
                        Console.WriteLine($"此商品已在订单中，数量: {existingDetail.Quantity}");
                        Console.Write("是否修改数量? (y/n): ");
                        if (Console.ReadLine()?.ToLower() != "y")
                            continue;
                        
                        Console.Write("输入新数量: ");
                        if (!int.TryParse(Console.ReadLine(), out var newQty) || newQty <= 0)
                        {
                            Console.WriteLine("无效数量");
                            continue;
                        }
                        
                        existingDetail.Quantity = newQty;
                        existingDetail.CalculateTotal();
                        Console.WriteLine("数量已更新");
                    }
                    else
                    {
                        Console.Write("输入购买数量: ");
                        if (!int.TryParse(Console.ReadLine(), out var qty) || qty <= 0)
                        {
                            Console.WriteLine("无效数量");
                            continue;
                        }
                        
                        var newDetail = new OrderDetail
                        { 
                            GoodsId = selectedGoods.Id,
                            Goods = selectedGoods,
                            Quantity = qty
                        };
                        
                        newDetail.CalculateTotal();
                        order.Details.Add(newDetail);
                        
                        Console.WriteLine($"{selectedGoods.Name} ×{qty} 已添加到订单");
                    }
                    detailsChanged = true;
                    break;
                    
                case "2": // 修改商品数量
                    if (order.Details.Count == 0)
                    {
                        Console.WriteLine("订单中没有商品");
                        continue;
                    }
                    
                    Console.WriteLine("\n当前商品列表:");
                    for (int j = 0; j < order.Details.Count; j++)
                    {
                        var detail = order.Details.ElementAt(j);
                        Console.WriteLine($"{j+1}. {detail.Goods?.Name ?? "未知商品"} - 当前数量: {detail.Quantity}");
                    }
                    
                    Console.Write("\n选择要修改的商品序号: ");
                    if (!int.TryParse(Console.ReadLine(), out var idx) || idx < 1 || idx > order.Details.Count)
                    {
                        Console.WriteLine("无效选择");
                        continue;
                    }
                    
                    var detailToUpdate = order.Details.ElementAt(idx - 1);
                    Console.Write($"输入 {detailToUpdate.Goods?.Name ?? "未知商品"} 的新数量: ");
                    if (!int.TryParse(Console.ReadLine(), out var newQuantity) || newQuantity <= 0)
                    {
                        Console.WriteLine("无效数量");
                        continue;
                    }
                    
                    detailToUpdate.Quantity = newQuantity;
                    detailToUpdate.CalculateTotal();
                    Console.WriteLine("数量已更新");
                    detailsChanged = true;
                    break;
                    
                case "3": // 删除商品
                    if (order.Details.Count == 0)
                    {
                        Console.WriteLine("订单中没有商品");
                        continue;
                    }
                    
                    Console.WriteLine("\n当前商品列表:");
                    for (int j = 0; j < order.Details.Count; j++)
                    {
                        var detail = order.Details.ElementAt(j);
                        Console.WriteLine($"{j+1}. {detail.Goods?.Name ?? "未知商品"} - 数量: {detail.Quantity}");
                    }
                    
                    Console.Write("\n选择要删除的商品序号: ");
                    if (!int.TryParse(Console.ReadLine(), out var delIdx) || delIdx < 1 || delIdx > order.Details.Count)
                    {
                        Console.WriteLine("无效选择");
                        continue;
                    }
                    
                    var detailToRemove = order.Details.ElementAt(delIdx - 1);
                    order.Details.Remove(detailToRemove);
                    Console.WriteLine($"{detailToRemove.Goods?.Name ?? "未知商品"} 已从订单中删除");
                    detailsChanged = true;
                    break;
                    
                default:
                    Console.WriteLine("无效选择");
                    break;
            }
            
            // 如果订单明细发生变化，显示更新后的明细
            if (detailsChanged)
            {
                Console.WriteLine("\n更新后的订单明细:");
                i = 1;
                foreach (var detail in order.Details)
                {
                    Console.WriteLine($"{i++}. {detail.Goods?.Name ?? "未知商品"} - 数量: {detail.Quantity}");
                }
                detailsChanged = false;
            }
        }
        
        if (order.Details.Count == 0)
        {
            Console.WriteLine("订单必须至少包含一个商品，修改取消");
            WaitForKey();
            return;
        }
        
        // 更新订单
        await service.UpdateOrderAsync(order);
        Console.WriteLine("\n订单修改成功!");
        
        // 重新获取并显示最新订单信息
        var updatedOrder = await service.GetOrderByIdAsync(order.OrderId);
        if (updatedOrder != null)
        {
            await PrintOrderDetailsAsync(updatedOrder);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"修改订单失败: {ex.Message}");
    }
    
    WaitForKey();
}

async Task QueryOrdersMenuAsync(OrderService service)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("=== 查询订单 ===");
        Console.WriteLine("1. 查询所有订单");
        Console.WriteLine("2. 按订单号查询");
        Console.WriteLine("3. 按客户名称查询");
        Console.WriteLine("4. 按商品名称查询");
        Console.WriteLine("5. 按金额范围查询");
        Console.WriteLine("0. 返回主菜单");
        Console.Write("请选择查询方式: ");
        
        string? choice = Console.ReadLine();
        
        try
        {
            switch (choice)
            {
                case "1":
                    await ListAllOrdersAsync(service);
                    break;
                case "2":
                    await QueryOrderByIdAsync(service);
                    break;
                case "3":
                    await QueryOrdersByCustomerAsync(service);
                    break;
                case "4":
                    await QueryOrdersByProductAsync(service);
                    break;
                case "5":
                    await QueryOrdersByAmountRangeAsync(service);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("无效选择");
                    await Task.Delay(1000);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询失败: {ex.Message}");
            WaitForKey();
        }
    }
}

async Task ListAllOrdersAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 所有订单列表 ===");
    
    var orders = await service.QueryOrdersAsync();
    
    if (orders.Count == 0)
    {
        Console.WriteLine("没有任何订单记录");
    }
    else
    {
        Console.WriteLine($"共有 {orders.Count} 个订单:");
        await PrintOrderListAsync(orders);
    }
    
    WaitForKey();
}

async Task QueryOrderByIdAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 按订单号查询 ===");
    Console.Write("请输入订单号: ");
    string? orderId = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(orderId))
    {
        Console.WriteLine("订单号不能为空");
        WaitForKey();
        return;
    }
    
    var order = await service.GetOrderByIdAsync(orderId);
    
    if (order == null)
    {
        Console.WriteLine($"未找到订单 {orderId}");
    }
    else
    {
        Console.WriteLine("查询结果:");
        await PrintOrderDetailsAsync(order);
    }
    
    WaitForKey();
}

async Task QueryOrdersByCustomerAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 按客户名称查询 ===");
    Console.Write("请输入客户名称: ");
    string? customerName = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(customerName))
    {
        Console.WriteLine("客户名称不能为空");
        WaitForKey();
        return;
    }
    
    var orders = await service.QueryOrdersByCustomerAsync(customerName);
    
    if (orders.Count == 0)
    {
        Console.WriteLine($"未找到客户名称包含 '{customerName}' 的订单");
    }
    else
    {
        Console.WriteLine($"找到 {orders.Count} 个订单:");
        await PrintOrderListAsync(orders);
    }
    
    WaitForKey();
}

async Task QueryOrdersByProductAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 按商品名称查询 ===");
    Console.Write("请输入商品名称: ");
    string? productName = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(productName))
    {
        Console.WriteLine("商品名称不能为空");
        WaitForKey();
        return;
    }
    
    var orders = await service.QueryOrdersByProductNameAsync(productName);
    
    if (orders.Count == 0)
    {
        Console.WriteLine($"未找到包含商品名称 '{productName}' 的订单");
    }
    else
    {
        Console.WriteLine($"找到 {orders.Count} 个订单:");
        await PrintOrderListAsync(orders);
    }
    
    WaitForKey();
}

async Task QueryOrdersByAmountRangeAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 按金额范围查询 ===");
    
    Console.Write("请输入最小金额: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal min) || min < 0)
    {
        Console.WriteLine("无效金额，必须为非负数");
        WaitForKey();
        return;
    }
    
    Console.Write("请输入最大金额: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal max) || max < min)
    {
        Console.WriteLine("无效金额，必须大于等于最小金额");
        WaitForKey();
        return;
    }
    
    var orders = await service.QueryOrdersByAmountRangeAsync(min, max);
    
    if (orders.Count == 0)
    {
        Console.WriteLine($"未找到金额在 ¥{min:N2} 至 ¥{max:N2} 范围内的订单");
    }
    else
    {
        Console.WriteLine($"找到 {orders.Count} 个订单:");
        await PrintOrderListAsync(orders);
    }
    
    WaitForKey();
}

async Task DeleteOrderAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 删除订单 ===");
    Console.Write("请输入要删除的订单号: ");
    string? orderId = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(orderId))
    {
        Console.WriteLine("订单号不能为空");
        WaitForKey();
        return;
    }
    
    var order = await service.GetOrderByIdAsync(orderId);
    
    if (order == null)
    {
        Console.WriteLine($"未找到订单 {orderId}");
        WaitForKey();
        return;
    }
    
    Console.WriteLine("\n将要删除以下订单:");
    await PrintOrderDetailsAsync(order);
    
    Console.Write("\n确定要删除此订单吗? (y/n): ");
    if (Console.ReadLine()?.ToLower() != "y")
    {
        Console.WriteLine("已取消删除操作");
        WaitForKey();
        return;
    }
    
    try
    {
        await service.DeleteOrderAsync(orderId);
        Console.WriteLine("订单已成功删除");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"删除失败: {ex.Message}");
    }
    
    WaitForKey();
}

async Task ManageGoodsAsync(OrderService service)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("=== 商品管理 ===");
        Console.WriteLine("1. 查看所有商品");
        Console.WriteLine("2. 添加新商品");
        Console.WriteLine("0. 返回主菜单");
        Console.Write("请选择操作: ");
        
        string? choice = Console.ReadLine();
        
        try
        {
            switch (choice)
            {
                case "1":
                    await ListAllGoodsAsync(service);
                    break;
                case "2":
                    await AddGoodsAsync(service);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("无效选择");
                    await Task.Delay(1000);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作失败: {ex.Message}");
            WaitForKey();
        }
    }
}

async Task ListAllGoodsAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 商品列表 ===");
    
    var goods = await service.GetAllGoodsAsync();
    
    if (goods.Count == 0)
    {
        Console.WriteLine("没有任何商品记录");
    }
    else
    {
        Console.WriteLine($"共有 {goods.Count} 种商品:");
        foreach (var item in goods)
        {
            Console.WriteLine($"[{item.Id}] {item.Name} - 单价: ¥{item.Price:N2}");
        }
    }
    
    WaitForKey();
}

async Task AddGoodsAsync(OrderService service)
{
    Console.Clear();
    Console.WriteLine("=== 添加新商品 ===");
    
    Console.Write("商品名称: ");
    string? name = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("商品名称不能为空");
        WaitForKey();
        return;
    }
    
    Console.Write("商品价格: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
    {
        Console.WriteLine("无效价格，必须为正数");
        WaitForKey();
        return;
    }
    
    var goods = new Goods { Name = name, Price = price };
    
    try
    {
        await service.AddGoodsAsync(goods);
        Console.WriteLine($"商品 '{name}' 添加成功，价格: ¥{price:N2}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"添加商品失败: {ex.Message}");
    }
    
    WaitForKey();
}

async Task PrintOrderListAsync(List<Order> orders)
{
    Console.WriteLine("\n订单列表:");
    Console.WriteLine(new string('-', 60));
    Console.WriteLine($"{"订单号",-15} {"客户",-15} {"创建时间",-20} {"总金额",10}");
    Console.WriteLine(new string('-', 60));
    
    foreach (var order in orders)
    {
        Console.WriteLine($"{order.OrderId,-15} {order.Customer,-15} {order.CreateTime,-20:yyyy-MM-dd HH:mm:ss} {order.Total,10:C}");
    }
    
    Console.WriteLine("\n输入订单号查看详情，直接按回车返回菜单: ");
    string? idToView = Console.ReadLine();
    
    if (!string.IsNullOrWhiteSpace(idToView))
    {
        var orderToView = orders.FirstOrDefault(o => o.OrderId == idToView);
        if (orderToView != null)
        {
            await PrintOrderDetailsAsync(orderToView);
        }
        else
        {
            Console.WriteLine($"未找到订单号 {idToView}");
            WaitForKey();
        }
    }
}

async Task PrintOrderDetailsAsync(Order order)
{
    Console.WriteLine(new string('=', 50));
    Console.WriteLine($"订单号: {order.OrderId}");
    Console.WriteLine($"客户: {order.Customer}");
    Console.WriteLine($"创建时间: {order.CreateTime:yyyy-MM-dd HH:mm:ss}");
    Console.WriteLine(new string('-', 50));
    Console.WriteLine("商品明细:");
    Console.WriteLine($"{"商品",-20} {"单价",10} {"数量",6} {"小计",12}");
    Console.WriteLine(new string('-', 50));
    
    decimal total = 0;
    foreach (var detail in order.Details)
    {
        string productName = detail.Goods?.Name ?? "未知商品";
        decimal unitPrice = detail.Goods?.Price ?? 0;
        decimal subtotal = detail.Total;
        total += subtotal;
        
        Console.WriteLine($"{productName,-20} {unitPrice,10:C} {detail.Quantity,6} {subtotal,12:C}");
    }
    
    Console.WriteLine(new string('-', 50));
    Console.WriteLine($"{"总计:",-37} {total,12:C}");
    Console.WriteLine(new string('=', 50));
}

string GenerateOrderId() => 
    $"ORD{DateTime.Now:yyyyMMdd}{Guid.NewGuid().ToString("N")[..4].ToUpper()}";

void WaitForKey()
{
    Console.WriteLine("\n按任意键继续...");
    Console.ReadKey();
}
