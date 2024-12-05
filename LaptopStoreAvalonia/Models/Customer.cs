using System.Collections.Generic;
using LaptopStoreAvalonia;

namespace LaptopStoreAvalonia
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

