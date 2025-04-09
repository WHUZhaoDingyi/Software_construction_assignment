namespace OrderCore.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int GoodsId { get; set; }
        public Goods Goods { get; set; }
        public int Quantity { get; set; }

        public decimal Total { get; set; }  // 确保这个字段在数据库中存在
        //public decimal Total => Goods?.Price * Quantity ?? 0;
    }
}