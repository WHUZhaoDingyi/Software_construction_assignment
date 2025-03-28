namespace OrderCore
{
    public class Goods
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }

        public override string ToString() => $"{Name}(¥{Price})";
    }
}