using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderCore.Models
{
    [Table("Goods")]
    public class Goods
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        // 导航属性
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
        
        public override string ToString() => $"{Name}(¥{Price:F2})";
    }
}