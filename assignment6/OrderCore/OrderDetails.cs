using System;

namespace OrderCore
{
    public class OrderDetails : IEquatable<OrderDetails>
    {
        public required Goods Item { get; set; }
        public int Quantity { get; set; }

        public decimal Total => Item.Price * Quantity;

        public bool Equals(OrderDetails other) => 
            other != null &&
            Item.Name == other.Item.Name &&
            Item.Price == other.Item.Price &&
            Quantity == other.Quantity;

        public override bool Equals(object obj) => Equals(obj as OrderDetails);
        public override int GetHashCode() => HashCode.Combine(Item.Name, Item.Price, Quantity);
        public override string ToString() => $"{Item} ×{Quantity} 小计：¥{Total}";
    }
}