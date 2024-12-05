using System.Collections.Generic;
using System;
using LaptopStoreAvalonia;

namespace LaptopStoreAvalonia
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public required Customer Customer { get; set; }
        public int ProductId { get; set; }
        public required Product Product { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}