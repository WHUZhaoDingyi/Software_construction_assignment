using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain;

namespace OrderManagement.Service
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        // 添加带参数的构造函数
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        // 无参构造函数，用于设计时和测试
        public OrderDbContext() 
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // 只有在未配置时才使用这个连接字符串
                optionsBuilder.UseMySql("server=localhost;user=root;password=123456;database=OrderManagement;CharSet=utf8mb4", 
                    ServerVersion.AutoDetect("server=localhost;user=root;password=123456;database=OrderManagement"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置Order实体
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Customer).IsRequired().HasMaxLength(100);
                
                // 配置一对多关系
                entity.HasMany(e => e.OrderDetails)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade); // 删除订单时级联删除订单明细
                
                // 添加索引
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.Customer);
            });

            // 配置OrderDetails实体
            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            });
            
            // 添加种子数据（可选）
            // SeedData(modelBuilder);
            
            base.OnModelCreating(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // 添加示例订单
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    OrderNumber = "ORD-001",
                    Customer = "张三"
                },
                new Order
                {
                    Id = 2,
                    OrderNumber = "ORD-002",
                    Customer = "李四"
                }
            );
            
            // 添加示例订单明细
            modelBuilder.Entity<OrderDetails>().HasData(
                new OrderDetails
                {
                    Id = 1,
                    ProductName = "商品1",
                    Amount = 100.00m
                },
                new OrderDetails
                {
                    Id = 2,
                    ProductName = "商品2",
                    Amount = 200.00m
                },
                new OrderDetails
                {
                    Id = 3,
                    ProductName = "商品3",
                    Amount = 300.00m
                }
            );
        }
    }
}