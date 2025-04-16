using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain;

namespace OrderManagement.Service
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 修改为你的数据库连接字符串
            optionsBuilder.UseMySQL("server=localhost;user=root;password=yourpassword;database=OrderManagement");
        }
    }
}