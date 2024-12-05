using System.Collections.Generic;
using LaptopStoreAvalonia;

namespace LaptopStoreAvalonia
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int ManufactureId { get; set; }
        public required Manufacture Manufacture { get;set; }
        
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}