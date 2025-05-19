using Microsoft.EntityFrameworkCore;
using OrderCore.Models;

namespace OrderCore.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Goods> Goods { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

        public AppDbContext() { }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(
                    "server=localhost;port=3306;database=orderdb;user=root;password=123456;CharSet=utf8mb4",
                    ServerVersion.AutoDetect("server=localhost;port=3306;database=orderdb;user=root;password=123456"),
                    options => options.EnableRetryOnFailure(3)
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // 订单配置
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(o => o.OrderId).IsUnique();
                
                entity.HasMany(o => o.Details)
                      .WithOne(d => d.Order)
                      .HasForeignKey(d => d.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 订单明细配置
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(od => new { od.OrderId, od.GoodsId });
                
                entity.HasOne(od => od.Goods)
                      .WithMany(g => g.OrderDetails)
                      .HasForeignKey(od => od.GoodsId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            // 商品配置
            modelBuilder.Entity<Goods>(entity =>
            {
                entity.HasIndex(g => g.Name);
            });
            
            // 添加种子数据
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // 添加商品数据
            modelBuilder.Entity<Goods>().HasData(
                new Goods { Id = 1, Name = "手机", Price = 4999m },
                new Goods { Id = 2, Name = "笔记本电脑", Price = 7999m },
                new Goods { Id = 3, Name = "耳机", Price = 899m },
                new Goods { Id = 4, Name = "平板电脑", Price = 3999m },
                new Goods { Id = 5, Name = "智能手表", Price = 1999m }
            );
        }
    }
}