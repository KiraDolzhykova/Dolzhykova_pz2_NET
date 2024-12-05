using System.Collections.Generic;
using LaptopStoreAvalonia;

namespace LaptopStoreAvalonia
{
    public class Manufacture
    {
        public int ManufactureId { get; set; }
        public required string Name { get; set; }
        public required string Country { get; set; }
        
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}