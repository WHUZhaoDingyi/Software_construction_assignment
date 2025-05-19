using System;

namespace OrderCore
{
    public class Goods : IEquatable<Goods>
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }

        public bool Equals(Goods? other)
        {
            if (other == null) return false;
            return Id == other.Id;
        }

        public override bool Equals(object? obj) => Equals(obj as Goods);
        public override int GetHashCode() => Id?.GetHashCode() ?? 0;
        public override string ToString() => $"{Name}(¥{Price:F2})";
    }
}