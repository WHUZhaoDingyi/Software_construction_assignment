using System;

namespace OrderCore
{
    public class OrderDetails : IEquatable<OrderDetails>
    {
        public Goods Item { get; set; }
        public int Quantity { get; set; }

        // 计算订单明细的小计，确保 Item 不为 null
        public decimal Total
        {
            get
            {
                if (Item == null)
                {
                    throw new InvalidOperationException("商品信息不能为空");
                }
                return Item.Price * Quantity;
            }
        }

        // 判断两个订单明细是否相等
        public bool Equals(OrderDetails other)
        {
            // 如果 other 是 null 或者本身没有 Item，则不能比较
            if (other == null || other.Item == null || Item == null)
                return false;

            return Item.Name == other.Item.Name && Item.Price == other.Item.Price && Quantity == other.Quantity;
        }

        // 重写 Equals 方法，确保多态性
        public override bool Equals(object obj) => Equals(obj as OrderDetails);

        // 重写 GetHashCode 方法
        public override int GetHashCode() =>
            HashCode.Combine(Item?.Name, Item?.Price, Quantity);

        // 重写 ToString 方法，返回订单明细的字符串表示
        public override string ToString() =>
            $"{Item?.Name ?? "未知商品"} ×{Quantity} 小计：¥{Total}";
    }
}
