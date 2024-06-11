using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModals;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Linq;
using System.Security.Principal;
using BC = BCrypt.Net.BCrypt;

namespace BAL.Repositry
{
    public class ProductRepo : IProduct
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString = "User ID=postgres;Password=1234;Server=localhost;Port=5432;Database=ProductManagement;Integrated Security=true;Pooling=true;";

        public ProductRepo(ApplicationDbContext context) { _context = context; }

        public List<ProductVM> GetProductList(string productName, string change, bool boolvalue)
        {
            var product = _context.Products.Include(item => item.Category).Select(pro => new ProductVM
            {
                ProductName = pro.ProductName,
                ProductId = pro.ProductId,
                CategoryName = pro.Category.CategoryName,
                Price = (int)pro.Price,
                ProductDescription = pro.Description,
                UniqueNo = pro.UniqueNo,
                IsDeleted = (bool)pro.Isdeleted,
                Quantity = (int)pro.Quantity,
                DatePicker = pro.DateTimePicker.Value,
                featurePhoto = pro.FeaturePhoto,
                FileNames = _context.ProductPhotos.Where(item => item.ProductId == pro.ProductId).Select(item => item.PhotoName).ToList(),
            }).Where(item => string.IsNullOrEmpty(productName) || (item.ProductName.ToLower().Contains(productName) || item.UniqueNo.Contains(productName) || item.CategoryName.ToLower().Contains(productName) || item.ProductDescription.ToLower().Contains(productName))).ToList();

            if (boolvalue)
            {

                switch (change)
                {
                    case "ProductId":
                        product = product.OrderBy(p => p.ProductId).ToList();
                        break;

                    case "ProductName":
                        product = product.OrderBy(p => p.ProductName).ToList();
                        break;

                    case "Category":
                        product = product.OrderBy(p => p.CategoryName).ToList();
                        break;
                    case "Price":
                        product = product.OrderBy(p => p.Price).ToList();
                        break;
                    case "SerialNo":
                        product = product.OrderBy(p => p.UniqueNo).ToList();
                        break;
                    case "Description":
                        product = product.OrderBy(p => p.UniqueNo).ToList();
                        break;
                    case "Quantity":
                        product = product.OrderBy(p => p.Quantity).ToList();
                        break;
                    case "OrderDate":
                        product = product.OrderBy(p => p.DatePicker).ToList();
                        break;

                }
            }
            else
            {
                switch (change)
                {
                    case "ProductId":
                        product = product.OrderByDescending(p => p.ProductId).ToList();
                        break;

                    case "ProductName":
                        product = product.OrderByDescending(p => p.ProductName).ToList();
                        break;

                    case "Category":
                        product = product.OrderByDescending(p => p.CategoryName).ToList();
                        break;

                    case "Price":
                        product = product.OrderByDescending(p => p.Price).ToList();
                        break;
                    case "SerialNo":
                        product = product.OrderByDescending(p => p.UniqueNo).ToList();
                        break;
                    case "Description":
                        product = product.OrderByDescending(p => p.ProductDescription).ToList();
                        break;
                    case "Quantity":
                        product = product.OrderByDescending(p => p.Quantity).ToList();
                        break;
                    case "OrderDate":
                        product = product.OrderByDescending(p => p.DatePicker).ToList();
                        break;
                }
            }

            return product;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.Where(item => item.IsDeleted == false).ToList();
        }

        public bool DeleteProduct(int id)
        {
            var product = _context.Products.Where(item => item.ProductId == id).FirstOrDefault();
            product.Isdeleted = true;
            _context.Products.Update(product);
            _context.SaveChanges();
            return true;
        }

        public bool IsAddTheProduct(ProductVM product)
        {
            Product product1 = new Product();
            product1.ProductName = product.ProductName;
            product1.CategoryId = _context.Categories.Where(item => item.CategoryName == product.CategoryName).Select(item => item.CategoryId).FirstOrDefault();
            product1.UniqueNo = Guid.NewGuid().ToString();
            product1.Price = (int)product.Price;
            product1.Description = product.ProductDescription;
            product1.Isdeleted = false;
            product1.DateTimePicker = product.DatePicker;
            product1.Quantity = product.Quantity;
            product1.FeaturePhoto = product.FileNames.FirstOrDefault();
            _context.Products.Add(product1);
            _context.SaveChanges();
            if (product.FileNames.Count > 0)
            {
                foreach (var file in product.FileNames)
                {
                    ProductPhoto photo = new ProductPhoto();
                    photo.ProductId = product1.ProductId;
                    photo.PhotoName = file;
                    _context.ProductPhotos.Add(photo);
                    _context.SaveChanges();
                }
            }

            return true;
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Include(item => item.Category).Include(item => item.ProductPhotos).Where(item => item.ProductId == id).FirstOrDefault();
        }

        public bool UpdateProduct(int id, ProductVM product)
        {
            var updateProduct = _context.Products.Where(item => item.ProductId == id).FirstOrDefault();
            updateProduct.UniqueNo = product.UniqueNo;
            updateProduct.Price = product.Price;
            updateProduct.Description = product.ProductDescription;
            updateProduct.ProductName = product.ProductName;
            updateProduct.DateTimePicker = product.DatePicker;
            updateProduct.Quantity = product.Quantity;
            updateProduct.FeaturePhoto = product.featurePhoto;
            updateProduct.CategoryId = _context.Categories.Where(item => item.CategoryName == product.CategoryName).Select(item => item.CategoryId).FirstOrDefault();
            _context.Update(updateProduct);
            _context.SaveChanges();
            if (product.FileNames.Count > 0)
            {
                foreach (var file in product.FileNames)
                {
                    ProductPhoto photo = new ProductPhoto();
                    photo.ProductId = id;
                    photo.PhotoName = file;
                    _context.ProductPhotos.Add(photo);
                    _context.SaveChanges();
                }
            }
            return true;
        }

        public User GetUser(string email)
        {
            return _context.Users.Where(item => item.Email == email).FirstOrDefault();
        }

        public void AddUser(RegistrationVM registration)
        {
            User user = new User();
            user.UserId = Guid.NewGuid().ToString();
            user.Password = BC.HashPassword(registration.Passwordhash);
            user.Email = registration.Email;
            _context.Add(user);
            _context.SaveChanges();
        }

        public bool IsLoginValid(loginVM login)
        {
            var user = _context.Users.Where(item => item.Email == login.Email).FirstOrDefault();
            if (user == null || !BC.Verify(login.Passwordhash, user.Password))
            {
                return false;
            }
            else
            {
                return true;
            }


        }
        public List<ProductPhoto> getProductPhoto(int id)
        {
            var photo = _context.ProductPhotos.Where(item => item.ProductId == id).ToList();
            return photo;
        }

        public bool DeletePhoto(string photoId)
        {
            var photo = _context.ProductPhotos.Include(item => item.Product).Where(item => item.PhotoName == photoId).FirstOrDefault();
            var photos = _context.ProductPhotos.Where(item => item.ProductId == photo.Product.ProductId).ToList();
            if (photos.Count > 0)
            {
                if (photo.Product.FeaturePhoto == photoId)
                {
                    photo.Product.FeaturePhoto = photos.Where(item => item.PhotoName != photoId).FirstOrDefault().PhotoName;
                }
            }
            else
            {
                return false;
            }

            _context.ProductPhotos.Remove(photo);
            _context.SaveChanges();
            return true;
        }

        public bool IsRestore(int id)
        {
            var product = _context.Products.FirstOrDefault(item => item.ProductId == id);
            product.Isdeleted = false;
            _context.Products.Update(product);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<Product> GetAllProducts(string searchValue, int? minPrice, int? maxPrice)
        {
            var products = _context.Products
                .Include(item => item.Category)
                .Include(item => item.ProductPhotos)
                .Where(item => string.IsNullOrEmpty(searchValue) || item.ProductName.ToLower().Contains(searchValue.ToLower()));

            if (minPrice.HasValue && maxPrice.HasValue)
            {
                products = products.Where(item => item.Price >= minPrice.Value && item.Price <= maxPrice.Value);
            }

            return products.ToList();
        }


        public List<CartItems> GetCartItems( Dictionary<string, int> cart)
        {
            var productIds = cart.Keys.ToList(); // Extract product IDs from the cart dictionary
            var cartItems = _context.Products
                .Where(item => productIds.Contains(item.ProductId.ToString())) // Convert ProductId to string for comparison
                .Select(item => new CartItems
                {
                    CartItemName = item.ProductName,
                    CartItemPrice = (int)item.Price,
                    CartMaxQuantity = (int)item.Quantity,
                    ProductId = item.ProductId,
                    CartFileName = item.FeaturePhoto,
                    CartItemQuantity = cart[item.ProductId.ToString()] // Get the current quantity from the cart dictionary based on the product ID
                })
                .ToList();

            return cartItems;

        }
        public Customer AddCustomerDetaile(string FirstName, string LastName, string Email, string Address, int ZipCode, string City)
        {
            var email = _context.Customers.FirstOrDefault(item => item.Email == Email);
            if (email == null)
            {
                Customer customer = new Customer();
                customer.FirstName = FirstName;
                customer.LastName = LastName;
                customer.Email = Email;
                customer.Address = Address;
                customer.City = City;
                customer.ZipCode = ZipCode;
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return customer;
            }
            else
            {
                return email;
            }
        }
        public bool OrderDetails(List<int> CartQuantity, List<int> CartPrice, List<int> ProductId, int CustomerId,string uniqNumber )
        {
   
            for (int i = 0; i < CartQuantity.Count; i++)
            {

                Order order = new Order();
                order.OrderPrice = (int)CartPrice[i];
                order.OrderQuantity = (int)CartQuantity[i];
                order.CustomerId = CustomerId;
                order.ProductId = ProductId[i];
                var product = _context.Products.FirstOrDefault(item => item.ProductId == ProductId[i]);
                product.Quantity = product.Quantity - CartQuantity[i];
                order.OrderDate = DateTime.Now;
                order.UniqOrderId= uniqNumber;  
                order.Status = 1;
                _context.Products.Update(product);
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            OrderStatusLog orderStatusLog = new OrderStatusLog();
            orderStatusLog.OrderStatus =1;
            orderStatusLog.UniqOrderId =uniqNumber;
            orderStatusLog.UpDatedDate = DateTime.Now;
            _context.OrderStatusLogs.Add(orderStatusLog);
            _context.SaveChanges();

            OrderProduct orderProduct= new OrderProduct();
            orderProduct.CustomerId = CustomerId;
            orderProduct.OrderUniqId = uniqNumber;
            orderProduct.OrderDate = DateTime.Now;
            orderProduct.NotificationBool = true;
            _context.OrderProducts.Add(orderProduct);
            _context.SaveChanges();     
            return true;
        }

		public int ProductCountSevenDay(int id)
        {
            return _context.Orders.Where(item=>item.ProductId== id &&  item.OrderDate>=DateTime.Now.AddDays(-7)).Count(); 
        }
        public List<OrderProduct> OrderLastTwoHors()
        {
            return _context.OrderProducts.Include(item=>item.Customer).Where(item => item.OrderDate > DateTime.Now.AddHours(-2)).ToList();
        }

        public List<NotificationVM> GetNotificationTwoHours()
        {
            return _context.OrderProducts.Where(item=>item.OrderDate>DateTime.Now.AddHours(-2) && item.NotificationBool).Select(item=>new NotificationVM
            {
                OrderId=item.OrderUniqId,
                CustomerName=item.Customer.FirstName,
            }).ToList();
        }
    }
}
