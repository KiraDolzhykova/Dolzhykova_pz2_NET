using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LaptopStoreAvalonia.ViewModels
{
    public class MainViewModel
    {
        private readonly AppDbContext _context;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();
        public ObservableCollection<Manufacture> Manufactures { get; set; }
        public ObservableCollection<Order> Orders { get; set; }

        public MainViewModel()
        {
            Customers = new ObservableCollection<Customer>
            {
                new Customer { CustomerId = 1, Name = "Клієнт 1", Email = "email1@example.com", Phone = "123456789" },
                new Customer { CustomerId = 2, Name = "Клієнт 2", Email = "email2@example.com", Phone = "987654321" }
            };
        }

        // private void LoadData()
        // {
        //     // Продукти з включенням даних про виробника
        //     Products = new ObservableCollection<Product>(
        //         _context.Products.Include(p => p.Manufacture).AsEnumerable());

        //     // Покупці
        //     Customers = new ObservableCollection<Customer>(_context.Customers.AsEnumerable());

        //     // Виробники
        //     Manufactures = new ObservableCollection<Manufacture>(_context.Manufactures.AsEnumerable());

        //     // Замовлення з включенням даних про покупця та продукт
        //     Orders = new ObservableCollection<Order>(
        //         _context.Orders
        //             .Include(o => o.Customer)
        //             .Include(o => o.Product)
        //             .AsEnumerable());
        // }
    }
}
