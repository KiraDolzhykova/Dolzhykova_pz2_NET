using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Media;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
//using LaptopStoreAvalonia.ViewModels;


namespace LaptopStoreAvalonia
{
    public partial class MainWindow : Window
    {
        private AppDbContext _context;
        public Manufacture? SelectedManufacture {get;set;}
        public Customer? SelectedCustomer {get;set;}
        public Product? SelectedProduct {get;set;}
        public ObservableCollection<Manufacture> Manufactures { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _context.Database.EnsureCreated();
            LoadData();
            SelectedManufacture = null;
            SelectedCustomer = null;
            SelectedProduct = null;
            DataContext = this;
            // Manufactures = new ObservableCollection<Manufacture>(_context.Manufactures.ToList());
            // ManufacturesGrid.ItemsSource = Manufactures;
        }

        private void LoadData()
        {
            ProductsGrid.ItemsSource = _context.Products.ToList();
            ManufacturesGrid.ItemsSource = _context.Manufactures.ToList();
            CustomersGrid.ItemsSource = _context.Customers.ToList();
            OrdersGrid.ItemsSource = _context.Orders.ToList();
            ManufactureComboBox.ItemsSource = _context.Manufactures.ToList();
            ProductComboBox.ItemsSource = _context.Products.ToList();
            CustomerComboBox.ItemsSource = _context.Customers.ToList();
            // Завантажуємо дані з бази даних і присвоюємо їх до ObservableCollection
            Manufactures = new ObservableCollection<Manufacture>(_context.Manufactures.ToList());
            Customers = new ObservableCollection<Customer>(_context.Customers.ToList());
            Products = new ObservableCollection<Product>(_context.Products.ToList());
            Orders = new ObservableCollection<Order>(_context.Orders.ToList());
        }


        private async void ShowMessage(string message, bool isSuccess)
        {
            // Відображення повідомлення через Popup
            PopupMessage.Text = message;
            if (isSuccess){
                MessagePopupBorder.Background = Brushes.Green;
            }
            else {
                MessagePopupBorder.Background = Brushes.Red;
            }
            MessagePopup.IsOpen = true;
            await Task.Delay(3000);
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                MessagePopup.IsOpen = false;
            });
        }

        #region Продукти
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedManufacture == null || ProductName == null || ProductPrice == null)
            {
                ShowMessage("Будь ласка, заповність всі поля.", false);
                return;
            }
            if (string.IsNullOrWhiteSpace(ProductName.Text) || 
                !decimal.TryParse(ProductPrice.Text, out var price))
            {
                ShowMessage("Будь ласка, введіть коректні дані для продукту.", false);
                return;
            }

            var newProduct = new Product
            {
                Name = ProductName.Text,
                Price = price,
                ManufactureId = SelectedManufacture.ManufactureId,
                Manufacture = SelectedManufacture
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();
            LoadData();

            // Очищення полів
            ProductName.Clear();
            ProductPrice.Clear();

            ShowMessage("Продукт успішно додано.", true);
        }
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selectedProduct)
            {
                _context.SaveChanges();
                LoadData();
            }
            else
            {
                ShowMessage("Будь ласка, виберіть продукт для редагування.", false);
            }
        }
        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            LoadData(); // Перевантажуємо дані
        }
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selectedProduct)
            {
                _context.Products.Remove(selectedProduct);
                _context.SaveChanges();
                LoadData();
                ShowMessage("Продукт успішно видалено.", true);
            }
            else
            {
                ShowMessage("Будь ласка, виберіть продукт для видалення.", false);
            }
        }
        private void ApplyProductFilter_Click(object sender, RoutedEventArgs e)
        {
            string productName = FilterProductName.Text;
            bool minPriceValid = decimal.TryParse(FilterMinPrice.Text, out decimal minPrice);
            bool maxPriceValid = decimal.TryParse(FilterMaxPrice.Text, out decimal maxPrice);

            var filteredProducts = _context.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(productName))
            {
                filteredProducts = filteredProducts.Where(p => p.Name.Contains(productName));
            }
            if (minPriceValid)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= minPrice);
            }
            if (maxPriceValid)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= maxPrice);
            }
            ProductsGrid.ItemsSource = filteredProducts.ToList();
        }

        private void SearchProduct_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchProductField.Text;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchResults = _context.Products
                    .Include(o => o.Manufacture)
                    .Where(o => 
                        o.Name.Contains(searchQuery) ||
                        o.Price.ToString().Contains(searchQuery) ||
                        o.Manufacture.Name.Contains(searchQuery))
                    .ToList();

                ProductsGrid.ItemsSource = searchResults;
            }
            else
            {
                LoadData();
            }
        }

        private void ResetProductFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterProductName.Clear();
            FilterMaxPrice.Clear();
            FilterMinPrice.Clear();
            LoadData();
        }
        #endregion

        #region Виробники
        private void AddManufacture_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManufactureName.Text) || string.IsNullOrWhiteSpace(ManufactureCountry.Text))
            {
                ShowMessage("Будь ласка, заповніть всі поля.", false);
                return;
            }

            var newManufacture = new Manufacture
            {
                Name = ManufactureName.Text,
                Country = ManufactureCountry.Text
            };

            _context.Manufactures.Add(newManufacture);
            _context.SaveChanges();
            LoadData();

            // Очищення полів
            ManufactureName.Clear();
            ManufactureCountry.Clear();

            ShowMessage("Виробника успішно додано.", true);
        }
        private void EditManufacture_Click(object sender, RoutedEventArgs e)
        {
            if (ManufacturesGrid.SelectedItem is Manufacture selectedManufacture)
            {
                _context.SaveChanges();
                LoadData();
            }
            else
            {
                ShowMessage("Будь ласка, виберіть виробника для редагування.", false);
            }
        }
        private void UpdateManufacture_Click(object sender, RoutedEventArgs e)
        {
            LoadData(); // Перевантажуємо дані
        }
        private void DeleteManufacture_Click(object sender, RoutedEventArgs e)
        {
            if (ManufacturesGrid.SelectedItem is Manufacture selectedManufacture)
            {
                _context.Manufactures.Remove(selectedManufacture);
                _context.SaveChanges();
                LoadData();
                ShowMessage("Виробника успішно видалено.", true);
            }
            else
            {
                ShowMessage("Будь ласка, виберіть виробника для видалення.", false);
            }
        }
        private void ApplyManufactureFilter_Click(object sender, RoutedEventArgs e)
        {
            string manufactureName = FilterManufactureName.Text;
            string manufactureCountry = FilterManufactureCountry.Text;
            
            var filteredManufactures = _context.Manufactures.AsQueryable();
            if (!string.IsNullOrWhiteSpace(manufactureName))
            {
                filteredManufactures = filteredManufactures.Where(p => p.Name.Contains(manufactureName));
            }
            if (!string.IsNullOrWhiteSpace(manufactureCountry))
            {
                filteredManufactures = filteredManufactures.Where(p => p.Country.Contains(manufactureCountry));
            }
            ManufacturesGrid.ItemsSource = filteredManufactures.ToList();
        }

        private void SearchManufacture_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchManufactureField.Text;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchResults = _context.Manufactures
                    .Where(o => 
                        o.Name.Contains(searchQuery) ||
                        o.Country.Contains(searchQuery))
                    .ToList();

                ManufacturesGrid.ItemsSource = searchResults;
            }
            else
            {
                LoadData();
            }
        }

        private void ResetManufactureFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterManufactureName.Clear();
            FilterManufactureCountry.Clear();
            LoadData();
        }
        #endregion

        #region Покупці
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomerName.Text) || string.IsNullOrWhiteSpace(CustomerEmail.Text) || string.IsNullOrWhiteSpace(CustomerPhone.Text))
            {
                ShowMessage("Будь ласка, заповніть всі поля.", false);
                return;
            }

            var newCustomer = new Customer
            {
                Name = CustomerName.Text,
                Email = CustomerEmail.Text,
                Phone = CustomerPhone.Text
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();
            LoadData();

            CustomerName.Clear();
            CustomerEmail.Clear();
            CustomerPhone.Clear();

            ShowMessage("Покупця успішно додано.", true);
        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersGrid.SelectedItem is Customer selectedCustomer)
            {
                _context.SaveChanges();
                LoadData();
            }
            else
            {
                ShowMessage("Будь ласка, виберіть продукт для редагування.", false);
            }
        }
        private void UpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            LoadData(); // Перевантажуємо дані
        }
        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersGrid.SelectedItem is Customer selectedCustomer)
            {
                _context.Customers.Remove(selectedCustomer);
                _context.SaveChanges();
                LoadData();
                ShowMessage("Продукт успішно видалено.", true);
            }
            else
            {
                ShowMessage("Будь ласка, виберіть продукт для видалення.", false);
            }
        }
        private void ApplyCustomerFilter_Click(object sender, RoutedEventArgs e)
        {
            string customerName = FilterCustomerName.Text;

            var filteredCustomers = _context.Customers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                filteredCustomers = filteredCustomers.Where(p => p.Name.Contains(customerName));
            }
            CustomersGrid.ItemsSource = filteredCustomers.ToList();
        }

        private void SearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchCustomerField.Text;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchResults = _context.Customers
                    .Where(o => 
                        o.Name.Contains(searchQuery) ||
                        o.Email.Contains(searchQuery) ||
                        o.Phone.Contains(searchQuery))
                    .ToList();

                CustomersGrid.ItemsSource = searchResults;
            }
            else
            {
                LoadData(); 
            }
        }

        private void ResetCustomerFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterCustomerName.Clear();
            LoadData();
        }
        #endregion

        #region Замовлення
        private void AddOrder_Click(object? sender, RoutedEventArgs e)
        {
            if (SelectedCustomer == null || SelectedProduct == null || OrderDate == null || OrderQuantity == null)
            {
                ShowMessage("Будь ласка, заповність всі поля.", false);
                return;
            }

            var quantity = int.Parse(OrderQuantity.Text);
            var totalPrice = SelectedProduct.Price * quantity;

            var newOrder = new Order
            {
                CustomerId = SelectedCustomer.CustomerId,
                ProductId = SelectedProduct.ProductId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                OrderDate = DateTime.Now,
                Customer = SelectedCustomer,
                Product = SelectedProduct
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();
            OrderQuantity.Clear();
            OrderDate.Clear();
            SelectedCustomer = null;
            SelectedProduct = null;

            ShowMessage("Замовлення додано успішно!", true);
        }
        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                //selectedOrder.Name = "Оновлений продукт";
                _context.SaveChanges();
                LoadData();
            }
            else
            {
                ShowMessage("Будь ласка, виберіть продукт для редагування.", false);
            }
        }
        private void UpdateOrder_Click(object sender, RoutedEventArgs e)
        {
            LoadData(); // Перевантажуємо дані
        }
        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                _context.Orders.Remove(selectedOrder);
                _context.SaveChanges();
                LoadData();
                ShowMessage("Продукт успішно видалено.", true);
            }
            else
            {
                ShowMessage("Будь ласка, виберіть продукт для видалення.", false);
            }
        }
        private void ApplyOrderFilter_Click(object sender, RoutedEventArgs e)
        {
            // Параметри фільтрації
            string filterDate = FilterOrderDate.Text;
            bool quantityValid = int.TryParse(FilterQuantity.Text, out int quantity);
            bool minTotalPrice = decimal.TryParse(FilterOrderMinTotalPrice.Text, out decimal minPriceValid);
            bool maxTotalPrice = decimal.TryParse(FilterOrderMaxTotalPrice.Text, out decimal maxPriceValid);
            string customerName = FilterOrderCustomerName.Text;
            string productName = FilterOrderProductName.Text;
            var filteredProducts = _context.Products.AsQueryable();

            var filteredOrders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .AsQueryable();

            // Фільтруємо за датою
            if (DateTime.TryParse(filterDate, out DateTime parsedDate))
            {
                filteredOrders = filteredOrders.Where(o => o.OrderDate.Date == parsedDate.Date);
            }

            if (quantityValid)
            {
                filteredOrders = filteredOrders.Where(o => o.Quantity == quantity);
            }

            if (minTotalPrice)
            {
                filteredOrders = filteredOrders.Where(p => p.TotalPrice >= minPriceValid);
            }
            if (maxTotalPrice)
            {
                filteredOrders = filteredOrders.Where(p => p.TotalPrice <= maxPriceValid);
            }

            // Фільтруємо за ім'ям клієнта
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                filteredOrders = filteredOrders.Where(o => o.Customer.Name.Contains(customerName));
            }

            // Фільтруємо за назвою продукту
            if (!string.IsNullOrWhiteSpace(productName))
            {
                filteredOrders = filteredOrders.Where(o => o.Product.Name.Contains(productName));
            }

            // Відображення результатів
            OrdersGrid.ItemsSource = filteredOrders.ToList();
        }


        private void SearchOrder_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchOrderField.Text;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchResults = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Product)
                    .Where(o => 
                        o.Customer.Name.Contains(searchQuery) || 
                        o.Product.Name.Contains(searchQuery) || 
                        o.OrderDate.ToString().Contains(searchQuery) ||
                        o.TotalPrice.ToString().Contains(searchQuery))
                    .ToList();

                OrdersGrid.ItemsSource = searchResults;
            }
            else
            {
                LoadData(); // Завантаження всіх даних
            }
        }


        private void ResetOrderFilters_Click(object sender, RoutedEventArgs e)
        {
            FilterOrderDate.Clear();
            FilterQuantity.Clear();
            FilterOrderMinTotalPrice.Clear();
            FilterOrderMaxTotalPrice.Clear();
            FilterOrderCustomerName.Clear();
            FilterOrderProductName.Clear();
            LoadData();
        }
        #endregion


        // private void NavigateToProducts(object sender, RoutedEventArgs e)
        // {
        //     MainTabControl.SelectedIndex = 0; // Продукти
        // }

        // private void NavigateToCustomers(object sender, RoutedEventArgs e)
        // {
        //     MainTabControl.SelectedIndex = 1; // Клієнти
        // }

        // private void NavigateToManufactures(object sender, RoutedEventArgs e)
        // {
        //     MainTabControl.SelectedIndex = 2; // Виробники
        // }

        // private void NavigateToOrders(object sender, RoutedEventArgs e)
        // {
        //     MainTabControl.SelectedIndex = 3; // Замовлення
        // }

        
    }
}