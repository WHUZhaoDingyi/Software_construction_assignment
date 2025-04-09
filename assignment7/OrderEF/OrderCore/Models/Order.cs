using System.Collections.Generic;

namespace OrderCore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string Customer { get; set; }
        public List<OrderDetail> Details { get; set; } = new();
    }
}