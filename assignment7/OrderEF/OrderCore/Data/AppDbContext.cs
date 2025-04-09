using Microsoft.EntityFrameworkCore;
using OrderCore.Models;

namespace OrderCore.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Goods> Goods { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;port=3306;database=orderdb;user=zhaodingyi;password=Order@1412",
                ServerVersion.Parse("8.0.25-mysql"),
                options => options.EnableRetryOnFailure(3)
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderId)
                .IsUnique();

            modelBuilder.Entity<OrderDetail>()
                .HasIndex(od => new { od.OrderId, od.GoodsId })
                .IsUnique();
        }
    }
}