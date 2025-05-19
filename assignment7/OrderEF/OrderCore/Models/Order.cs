using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OrderCore.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string OrderId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Customer { get; set; } = string.Empty;
        
        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        
        public virtual ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
        
        [NotMapped]
        public decimal Total => Details.Sum(d => d.Total);
        
        public void AddDetail(OrderDetail detail)
        {
            if (detail == null)
                throw new ArgumentNullException(nameof(detail), "订单明细不能为空");
                
            if (detail.Quantity <= 0)
                throw new ArgumentException("商品数量必须大于0", nameof(detail));
                
            if (Details.Any(d => d.GoodsId == detail.GoodsId))
                throw new ArgumentException($"商品ID {detail.GoodsId} 的明细已存在", nameof(detail));
            
            detail.CalculateTotal();
            Details.Add(detail);
        }
        
        public void RemoveDetail(int detailId)
        {
            var detail = Details.FirstOrDefault(d => d.Id == detailId);
            if (detail == null)
                throw new KeyNotFoundException($"未找到ID为 {detailId} 的订单明细");
            
            Details.Remove(detail);
        }
        
        public override string ToString() =>
            $"订单号：{OrderId}\n" +
            $"客户：{Customer}\n" +
            $"创建时间：{CreateTime:yyyy-MM-dd HH:mm:ss}\n" +
            $"总金额：¥{Total:F2}\n" +
            $"明细数量：{Details.Count}";
    }
}