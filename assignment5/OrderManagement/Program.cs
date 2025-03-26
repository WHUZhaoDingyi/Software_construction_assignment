using System;
using System.Collections.Generic;
using System.Linq;


// 商品类
public class Goods
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public override string ToString() => $"{Name}(¥{Price})";
}

// 订单明细类
public class OrderDetails : IEquatable<OrderDetails>
{
    public Goods Item { get; set; }
    public int Quantity { get; set; }

    public decimal Total => Item.Price * Quantity;

    public bool Equals(OrderDetails other) =>
        other != null &&
        Item.Name == other.Item.Name &&
        Item.Price == other.Item.Price &&
        Quantity == other.Quantity;

    public override bool Equals(object obj) => Equals(obj as OrderDetails);
    
    public override int GetHashCode() => 
        HashCode.Combine(Item.Name, Item.Price, Quantity);

    public override string ToString() =>
        $"{Item} ×{Quantity} 小计：¥{Total}";
}

// 订单类
public class Order : IEquatable<Order>
{
    public string OrderId { get; }
    public string Customer { get; set; }
    public List<OrderDetails> Details { get; } = new List<OrderDetails>();
    
    public decimal Total => Details.Sum(d => d.Total);

    public Order(string id) => OrderId = id;

    public bool Equals(Order other) => other?.OrderId == OrderId;
    
    public override bool Equals(object obj) => Equals(obj as Order);
    
    public override int GetHashCode() => OrderId.GetHashCode();
    
    public override string ToString() =>
        $"订单号：{OrderId}\n客户：{Customer}\n总金额：¥{Total}\n" +
        "明细：" + string.Join("\n  ", Details) + "\n"+"\n";
}

// 订单服务类
public class OrderService
{
    private readonly List<Order> orders = new List<Order>();

    // 添加订单
    public void AddOrder(Order order)
    {
        if (orders.Contains(order))
            throw new ArgumentException($"订单 {order.OrderId} 已存在");
        
        if (order.Details.Distinct().Count() != order.Details.Count)
            throw new ArgumentException("存在重复订单明细");
        
        orders.Add(order);
    }

    // 删除订单
    public void RemoveOrder(string orderId)
    {
        var order = orders.FirstOrDefault(o => o.OrderId == orderId);
        if (order == null)
            throw new KeyNotFoundException($"订单 {orderId} 不存在");
        
        orders.Remove(order);
    }

    // 更新订单
    public void UpdateOrder(Order newOrder)
    {
        var index = orders.FindIndex(o => o.OrderId == newOrder.OrderId);
        if (index == -1)
            throw new KeyNotFoundException($"订单 {newOrder.OrderId} 不存在");
        
        orders[index] = newOrder;
    }

    // 查询订单
    public IEnumerable<Order> QueryOrders(
        Func<Order, bool> predicate = null) =>
        (predicate == null ? orders : orders.Where(predicate))
            .OrderBy(o => o.Total);

    // 排序
    public void SortOrders(Comparison<Order> comparison = null)
    {
        // 如果没有传入自定义比较器，则默认按 OrderId 排序
        orders.Sort(comparison ?? ((x, y) =>
            string.Compare(x.OrderId, y.OrderId, StringComparison.Ordinal)));
    }
}

class Program
{
    static void Main()
    {
        var service = new OrderService();
        
        // 测试数据
        var goods1 = new Goods { Name = "手机", Price = 2999 };
        var goods2 = new Goods { Name = "耳机", Price = 399 };
        
        try
        {
            var order1 = new Order("2023001") { Customer = "张三" };
            order1.Details.AddRange(new[] {
                new OrderDetails { Item = goods1, Quantity = 1 },
                new OrderDetails { Item = goods2, Quantity = 2 }
            });
            
            service.AddOrder(order1);
            
            // 测试重复订单
            service.AddOrder(order1); // 这里会抛出异常
        }
        catch (Exception e)
        {
            Console.WriteLine($"错误：{e.Message}");
        }

        // 查询演示
        var results = service.QueryOrders(o => 
            o.Details.Any(d => d.Item.Name.Contains("手机")));
        
        Console.WriteLine("查询结果：");
        foreach (var order in results)
        {
            Console.WriteLine(order);
        }
    }
}

