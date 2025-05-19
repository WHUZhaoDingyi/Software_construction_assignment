using System;

namespace OrderCore
{
    public class OrderDetails : IEquatable<OrderDetails>
    {
        // 添加ID属性
        public int Id { get; set; }
        
        private Goods? _item;
        
        public Goods? Item 
        { 
            get => _item; 
            set 
            {
                _item = value;
                // 设置商品时记录商品ID、名称和价格，即使后续Item为null也能显示
                if (value != null)
                {
                    ItemId = value.Id.ToString();
                    ItemName = value.Name;
                    ItemPrice = value.Price;
                }
            } 
        }
        
        // 冗余存储商品信息，防止引用丢失
        public string ItemId { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal ItemPrice { get; set; }
        
        public int Quantity { get; set; }

        // 计算订单明细的小计，确保 Item 不为 null
        public decimal Total
        {
            get
            {
                if (Item == null)
                {
                    // 使用冗余存储的商品价格
                    return ItemPrice * Quantity;
                }
                return Item.Price * Quantity;
            }
        }

        // 判断两个订单明细是否相等
        public bool Equals(OrderDetails? other)
        {
            if (other == null)
                return false;
                
            // 优先使用ItemId比较，保证唯一性
            if (!string.IsNullOrEmpty(ItemId) && !string.IsNullOrEmpty(other.ItemId))
                return ItemId == other.ItemId;
            
            // 如果没有ID则使用Item对象比较
            if (Item != null && other.Item != null)
                return Item.Equals(other.Item);
            
            // 最后使用名称和价格
            return ItemName == other.ItemName && ItemPrice == other.ItemPrice;
        }

        // 重写 Equals 方法，确保多态性
        public override bool Equals(object? obj) => Equals(obj as OrderDetails);

        // 重写 GetHashCode 方法
        public override int GetHashCode() =>
            HashCode.Combine(ItemId ?? ItemName, ItemPrice);

        // 重写 ToString 方法，返回订单明细的字符串表示
        public override string ToString() =>
            $"{ItemName ?? Item?.Name ?? "未知商品"} ×{Quantity} 小计：¥{Total:F2}";
    }
}
