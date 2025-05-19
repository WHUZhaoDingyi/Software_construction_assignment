using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderCore.Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
        
        public int GoodsId { get; set; }
        
        [ForeignKey("GoodsId")]
        public virtual Goods? Goods { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        
        [NotMapped]
        public decimal CalculatedTotal => Goods?.Price * Quantity ?? 0;
        
        // 在保存前计算并设置总价
        public void CalculateTotal()
        {
            if (Goods != null)
            {
                Total = Goods.Price * Quantity;
            }
        }
        
        public override string ToString() => 
            $"{Goods?.Name ?? "未知商品"} ×{Quantity} 小计：¥{Total:F2}";
    }
}