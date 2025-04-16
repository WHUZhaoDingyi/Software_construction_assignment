using System;
using System.Collections.Generic;

namespace OrderManagement.Domain
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string Customer { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

        public decimal TotalAmount
        {
            get
            {
                return OrderDetails.Sum(od => od.Amount);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Order order &&
                   OrderNumber == order.OrderNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderNumber);
        }

        public override string ToString()
        {
            return $"OrderNumber: {OrderNumber}, Customer: {Customer}, TotalAmount: {TotalAmount}";
        }
    }
}    