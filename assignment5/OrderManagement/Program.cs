using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderManagement
{
    // 商品类
    public class Goods : IEquatable<Goods>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public bool Equals(Goods other)
        {
            if (other == null) return false;
            return Id == other.Id;
        }

        public override bool Equals(object obj) => Equals(obj as Goods);
        public override int GetHashCode() => Id?.GetHashCode() ?? 0;
        public override string ToString() => $"{Name}(¥{Price:F2})";
    }

    // 订单明细类
    public class OrderDetails : IEquatable<OrderDetails>
    {
        public Goods Item { get; set; }
        public int Quantity { get; set; }

        public decimal Total => Item == null ? 0 : Item.Price * Quantity;

        public bool Equals(OrderDetails other) =>
            other != null &&
            Item?.Id == other.Item?.Id;

        public override bool Equals(object obj) => Equals(obj as OrderDetails);
        
        public override int GetHashCode() => 
            HashCode.Combine(Item?.Id);

        public override string ToString() =>
            $"{Item} ×{Quantity} 小计：¥{Total:F2}";
    }

    // 订单类
    public class Order : IEquatable<Order>
    {
        public string OrderId { get; }
        public string Customer { get; set; }
        public DateTime CreateTime { get; }
        public List<OrderDetails> Details { get; } = new List<OrderDetails>();
        
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
            
            Details.Add(detail);
        }

        public void RemoveDetails(Goods item)
        {
            var detail = Details.FirstOrDefault(d => d.Item.Equals(item));
            if (detail == null)
                throw new KeyNotFoundException($"未找到商品 {item.Name} 的订单明细");
            
            Details.Remove(detail);
        }

        public bool Equals(Order other) => other?.OrderId == OrderId;
        
        public override bool Equals(object obj) => Equals(obj as Order);
        
        public override int GetHashCode() => OrderId?.GetHashCode() ?? 0;
        
        public override string ToString() =>
            $"订单号：{OrderId}\n" +
            $"客户：{Customer}\n" +
            $"创建时间：{CreateTime:yyyy-MM-dd HH:mm:ss}\n" +
            $"总金额：¥{Total:F2}\n" +
            $"明细：\n{string.Join("\n", Details.Select(d => $"  {d}"))}\n";
    }

    // 订单服务类
    public class OrderService
    {
        private readonly List<Order> orders = new List<Order>();

        // 添加订单
        public void AddOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "订单不能为空");
            if (orders.Contains(order))
                throw new ArgumentException($"订单 {order.OrderId} 已存在");
            if (!order.Details.Any())
                throw new ArgumentException("订单必须包含至少一个商品");
            
            orders.Add(order);
        }

        // 删除订单
        public void RemoveOrder(string orderId)
        {
            var order = GetOrderById(orderId);
            orders.Remove(order);
        }

        // 修改订单
        public void UpdateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "订单不能为空");
            
            var index = orders.FindIndex(o => o.OrderId == order.OrderId);
            if (index == -1)
                throw new KeyNotFoundException($"订单 {order.OrderId} 不存在");
            
            orders[index] = order;
        }

        // 获取订单
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
                .Where(o => o.Details.Any(d => d.Item.Name.Contains(productName)))
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
        public IEnumerable<Order> QueryOrders(Func<Order, bool> predicate = null)
        {
            var query = predicate == null ? orders : orders.Where(predicate);
            return query.OrderBy(o => o.Total);
        }

        // 排序方法 - 默认按订单号排序
        public void SortOrders(Comparison<Order> comparison = null)
        {
            orders.Sort(comparison ?? ((x, y) => string.Compare(x.OrderId, y.OrderId, StringComparison.Ordinal)));
        }

        // 获取所有订单
        public List<Order> GetAllOrders() => orders.ToList();
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("======= 欢迎使用订单管理系统 =======\n");
            
            try
            {
                var service = new OrderService();
                LoadSampleData(service);
                
                while (true)
                {
                    ShowMenu();
                    string choice = Console.ReadLine();
                    
                    try
                    {
                        switch (choice)
                        {
                            case "1":
                                AddOrder(service);
                                break;
                            case "2":
                                RemoveOrder(service);
                                break;
                            case "3":
                                UpdateOrder(service);
                                break;
                            case "4":
                                QueryOrder(service);
                                break;
                            case "5":
                                SortOrders(service);
                                break;
                            case "6":
                                ShowAllOrders(service);
                                break;
                            case "0":
                                Console.WriteLine("\n感谢使用订单管理系统，再见！");
                                return;
                            default:
                                Console.WriteLine("\n无效的选择，请重新输入！");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n操作失败：{ex.Message}");
                    }
                    
                    Console.WriteLine("\n按任意键继续...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"系统错误：{ex.Message}");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("请选择操作：");
            Console.WriteLine("1. 添加订单");
            Console.WriteLine("2. 删除订单");
            Console.WriteLine("3. 修改订单");
            Console.WriteLine("4. 查询订单");
            Console.WriteLine("5. 订单排序");
            Console.WriteLine("6. 显示所有订单");
            Console.WriteLine("0. 退出系统");
            Console.Write("请输入选项（0-6）：");
        }

        static void LoadSampleData(OrderService service)
        {
            // 创建测试商品
            var phone = new Goods { Id = "P001", Name = "手机", Price = 4999M };
            var laptop = new Goods { Id = "P002", Name = "笔记本电脑", Price = 7999M };
            var headset = new Goods { Id = "P003", Name = "耳机", Price = 899M };
            
            // 创建测试订单
            var order1 = new Order("ORD2023001") { Customer = "张三" };
            order1.AddDetails(new OrderDetails { Item = phone, Quantity = 1 });
            order1.AddDetails(new OrderDetails { Item = headset, Quantity = 1 });
            
            var order2 = new Order("ORD2023002") { Customer = "李四" };
            order2.AddDetails(new OrderDetails { Item = laptop, Quantity = 1 });
            
            var order3 = new Order("ORD2023003") { Customer = "王五" };
            order3.AddDetails(new OrderDetails { Item = phone, Quantity = 2 });
            
            // 添加测试订单
            service.AddOrder(order1);
            service.AddOrder(order2);
            service.AddOrder(order3);
            
            Console.WriteLine("已加载示例数据（3个订单）");
        }

        static Goods InputGoods()
        {
            Console.Write("请输入商品ID：");
            string id = Console.ReadLine();
            
            Console.Write("请输入商品名称：");
            string name = Console.ReadLine();
            
            Console.Write("请输入商品价格：");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
                throw new ArgumentException("价格必须为正数");
            
            return new Goods { Id = id, Name = name, Price = price };
        }

        static Order InputOrder()
        {
            Console.Write("请输入订单号：");
            string orderId = Console.ReadLine();
            
            Console.Write("请输入客户名称：");
            string customer = Console.ReadLine();
            
            var order = new Order(orderId) { Customer = customer };
            
            while (true)
            {
                Console.WriteLine("\n添加订单明细（输入0结束）：");
                
                Console.Write("请输入商品ID（输入0结束）：");
                string goodsId = Console.ReadLine();
                if (goodsId == "0") break;
                
                Console.Write("请输入商品名称：");
                string goodsName = Console.ReadLine();
                
                Console.Write("请输入商品价格：");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
                    throw new ArgumentException("价格必须为正数");
                
                Console.Write("请输入购买数量：");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                    throw new ArgumentException("数量必须为正整数");
                
                var goods = new Goods { Id = goodsId, Name = goodsName, Price = price };
                order.AddDetails(new OrderDetails { Item = goods, Quantity = quantity });
                
                Console.WriteLine("明细已添加");
            }
            
            return order;
        }

        static void AddOrder(OrderService service)
        {
            Console.WriteLine("\n===== 添加订单 =====");
            var order = InputOrder();
            service.AddOrder(order);
            Console.WriteLine("\n订单添加成功：");
            PrintOrder(order);
        }

        static void RemoveOrder(OrderService service)
        {
            Console.WriteLine("\n===== 删除订单 =====");
            Console.Write("请输入要删除的订单号：");
            string orderId = Console.ReadLine();
            
            var order = service.GetOrderById(orderId);
            Console.WriteLine("\n订单详情：");
            PrintOrder(order);
            
            Console.Write("\n确认删除？(y/n)：");
            string confirm = Console.ReadLine().ToLower();
            
            if (confirm == "y")
            {
                service.RemoveOrder(orderId);
                Console.WriteLine("订单已成功删除！");
            }
            else
            {
                Console.WriteLine("已取消删除操作！");
            }
        }

        static void UpdateOrder(OrderService service)
        {
            Console.WriteLine("\n===== 修改订单 =====");
            Console.Write("请输入要修改的订单号：");
            string orderId = Console.ReadLine();
            
            var oldOrder = service.GetOrderById(orderId);
            Console.WriteLine("\n当前订单详情：");
            PrintOrder(oldOrder);
            
            Console.WriteLine("\n请输入新的订单信息：");
            Console.Write("请输入客户名称：");
            string customer = Console.ReadLine();
            
            var newOrder = new Order(orderId) { Customer = customer };
            
            while (true)
            {
                Console.WriteLine("\n添加订单明细（输入0结束）：");
                
                Console.Write("请输入商品ID（输入0结束）：");
                string goodsId = Console.ReadLine();
                if (goodsId == "0") break;
                
                Console.Write("请输入商品名称：");
                string goodsName = Console.ReadLine();
                
                Console.Write("请输入商品价格：");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
                    throw new ArgumentException("价格必须为正数");
                
                Console.Write("请输入购买数量：");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                    throw new ArgumentException("数量必须为正整数");
                
                var goods = new Goods { Id = goodsId, Name = goodsName, Price = price };
                newOrder.AddDetails(new OrderDetails { Item = goods, Quantity = quantity });
                
                Console.WriteLine("明细已添加");
            }
            
            service.UpdateOrder(newOrder);
            Console.WriteLine("\n订单修改成功：");
            PrintOrder(newOrder);
        }

        static void QueryOrder(OrderService service)
        {
            Console.WriteLine("\n===== 查询订单 =====");
            Console.WriteLine("请选择查询方式：");
            Console.WriteLine("1. 按订单号查询");
            Console.WriteLine("2. 按客户名称查询");
            Console.WriteLine("3. 按商品名称查询");
            Console.WriteLine("4. 按订单金额范围查询");
            Console.Write("请选择查询方式（1-4）：");
            
            string choice = Console.ReadLine();
            IEnumerable<Order> results = null;
            
            switch (choice)
            {
                case "1":
                    Console.Write("请输入订单号关键词：");
                    string orderId = Console.ReadLine();
                    results = service.QueryByOrderId(orderId);
                    break;
                case "2":
                    Console.Write("请输入客户名称关键词：");
                    string customer = Console.ReadLine();
                    results = service.QueryByCustomer(customer);
                    break;
                case "3":
                    Console.Write("请输入商品名称关键词：");
                    string productName = Console.ReadLine();
                    results = service.QueryByProductName(productName);
                    break;
                case "4":
                    Console.Write("请输入最小金额：");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal min) || min < 0)
                        throw new ArgumentException("金额必须为非负数");
                    
                    Console.Write("请输入最大金额：");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal max) || max < min)
                        throw new ArgumentException("最大金额不能小于最小金额");
                    
                    results = service.QueryByAmountRange(min, max);
                    break;
                default:
                    Console.WriteLine("无效的选择");
                    return;
            }
            
            PrintOrders(results);
        }

        static void SortOrders(OrderService service)
        {
            Console.WriteLine("\n===== 订单排序 =====");
            Console.WriteLine("请选择排序方式：");
            Console.WriteLine("1. 按订单号排序（默认）");
            Console.WriteLine("2. 按金额升序排序");
            Console.WriteLine("3. 按金额降序排序");
            Console.WriteLine("4. 按客户名称排序");
            Console.WriteLine("5. 按创建时间排序");
            Console.Write("请选择排序方式（1-5）：");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    service.SortOrders();
                    Console.WriteLine("\n订单已按订单号排序：");
                    break;
                case "2":
                    service.SortOrders((o1, o2) => o1.Total.CompareTo(o2.Total));
                    Console.WriteLine("\n订单已按金额升序排序：");
                    break;
                case "3":
                    service.SortOrders((o1, o2) => o2.Total.CompareTo(o1.Total));
                    Console.WriteLine("\n订单已按金额降序排序：");
                    break;
                case "4":
                    service.SortOrders((o1, o2) => string.Compare(o1.Customer, o2.Customer, StringComparison.Ordinal));
                    Console.WriteLine("\n订单已按客户名称排序：");
                    break;
                case "5":
                    service.SortOrders((o1, o2) => o1.CreateTime.CompareTo(o2.CreateTime));
                    Console.WriteLine("\n订单已按创建时间排序：");
                    break;
                default:
                    Console.WriteLine("无效的选择，使用默认排序");
                    service.SortOrders();
                    break;
            }
            
            PrintOrders(service.GetAllOrders());
        }

        static void ShowAllOrders(OrderService service)
        {
            Console.WriteLine("\n===== 所有订单 =====");
            
            var orders = service.GetAllOrders();
            if (orders.Count == 0)
            {
                Console.WriteLine("暂无订单");
                return;
            }
            
            PrintOrders(orders);
        }

        static void PrintOrders(IEnumerable<Order> orders)
        {
            var orderList = orders.ToList();
            
            if (orderList.Count == 0)
            {
                Console.WriteLine("未找到符合条件的订单");
                return;
            }
            
            Console.WriteLine($"\n共找到 {orderList.Count} 个订单：\n");
            
            foreach (var order in orderList)
            {
                PrintOrder(order);
                Console.WriteLine(new string('-', 40));
            }
        }

        static void PrintOrder(Order order)
        {
            Console.WriteLine(order);
        }
    }
}

