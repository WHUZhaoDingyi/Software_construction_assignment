using System;

namespace OrderManagement.Domain
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }

        public override bool Equals(object obj)
        {
            return obj is OrderDetails details &&
                   ProductName == details.ProductName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProductName);
        }

        public override string ToString()
        {
            return $"ProductName: {ProductName}, Amount: {Amount}";
        }
    }
}    