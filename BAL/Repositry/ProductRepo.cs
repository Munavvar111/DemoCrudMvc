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

        public List<ProductVM> GetProductList(string ProductName, string Change, bool Boolvalue)
        {
            var ProductsList = _context.Products.Include(item => item.Category).Select(pro => new ProductVM
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
                FeaturePhoto = pro.FeaturePhoto,
                FileNames = _context.ProductPhotos.Where(item => item.ProductId == pro.ProductId).Select(item => item.PhotoName).ToList(),
            }).Where(item => string.IsNullOrEmpty(ProductName) || (item.ProductName.ToLower().Contains(ProductName) || item.UniqueNo.Contains(ProductName) || item.CategoryName.ToLower().Contains(ProductName) || item.ProductDescription.ToLower().Contains(ProductName))).ToList();

            if (Boolvalue)
            {

                switch (Change)
                {
                    case "ProductId":
                        ProductsList = ProductsList.OrderBy(p => p.ProductId).ToList();
                        break;

                    case "ProductName":
                        ProductsList = ProductsList.OrderBy(p => p.ProductName).ToList();
                        break;

                    case "Category":
                        ProductsList = ProductsList.OrderBy(p => p.CategoryName).ToList();
                        break;
                    case "Price":
                        ProductsList = ProductsList.OrderBy(p => p.Price).ToList();
                        break;
                    case "SerialNo":
                        ProductsList = ProductsList.OrderBy(p => p.UniqueNo).ToList();
                        break;
                    case "Description":
                        ProductsList = ProductsList.OrderBy(p => p.UniqueNo).ToList();
                        break;
                    case "Quantity":
                        ProductsList = ProductsList.OrderBy(p => p.Quantity).ToList();
                        break;
                    case "OrderDate":
                        ProductsList = ProductsList.OrderBy(p => p.DatePicker).ToList();
                        break;

                }
            }
            else
            {
                switch (Change)
                {
                    case "ProductId":
                        ProductsList = ProductsList.OrderByDescending(p => p.ProductId).ToList();
                        break;

                    case "ProductName":
                        ProductsList = ProductsList.OrderByDescending(p => p.ProductName).ToList();
                        break;

                    case "Category":
                        ProductsList = ProductsList.OrderByDescending(p => p.CategoryName).ToList();
                        break;

                    case "Price":
                        ProductsList = ProductsList.OrderByDescending(p => p.Price).ToList();
                        break;
                    case "SerialNo":
                        ProductsList = ProductsList.OrderByDescending(p => p.UniqueNo).ToList();
                        break;
                    case "Description":
                        ProductsList = ProductsList.OrderByDescending(p => p.ProductDescription).ToList();
                        break;
                    case "Quantity":
                        ProductsList = ProductsList.OrderByDescending(p => p.Quantity).ToList();
                        break;
                    case "OrderDate":
                        ProductsList = ProductsList.OrderByDescending(p => p.DatePicker).ToList();
                        break;
                }
            }
            _context.Dispose();

            return ProductsList;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.Where(item => item.IsDeleted == false).ToList();
        }

        public bool DeleteProduct(int Id)
        {
            var Product = _context.Products.Where(item => item.ProductId == Id).FirstOrDefault();
            Product.Isdeleted = true;
            _context.Products.Update(Product);
            _context.SaveChanges();
            _context.Dispose();

            return true;
        }

        public bool IsAddTheProduct(ProductVM Product)
        {
            Product product1 = new Product();
            product1.ProductName = Product.ProductName;
            product1.CategoryId = _context.Categories.Where(item => item.CategoryName == Product.CategoryName).Select(item => item.CategoryId).FirstOrDefault();
            product1.UniqueNo = Guid.NewGuid().ToString();
            product1.Price = (int)Product.Price;
            product1.Description = Product.ProductDescription;
            product1.Isdeleted = false;
            product1.DateTimePicker = Product.DatePicker;
            product1.Quantity = Product.Quantity;
            product1.FeaturePhoto = Product.FileNames.FirstOrDefault();
            _context.Products.Add(product1);
            _context.SaveChanges();
            if (Product.FileNames.Count > 0)
            {
                foreach (var File in Product.FileNames)
                {
                    ProductPhoto photo = new ProductPhoto();
                    photo.ProductId = product1.ProductId;
                    photo.PhotoName = File;
                    _context.ProductPhotos.Add(photo);
                    _context.SaveChanges();
                }
                _context.Dispose();

            }

            return true;
        }

        public Product GetProductById(int Id)
        {
            return _context.Products.Include(item => item.Category).Include(item => item.ProductPhotos).Where(item => item.ProductId == Id).FirstOrDefault();
        }

        public bool UpdateProduct(int Id, ProductVM Product)
        {
            var UpdateProduct = _context.Products.Where(item => item.ProductId == Id).FirstOrDefault();
            UpdateProduct.UniqueNo = Product.UniqueNo;
            UpdateProduct.Price = Product.Price;
            UpdateProduct.Description = Product.ProductDescription;
            UpdateProduct.ProductName = Product.ProductName;
            UpdateProduct.DateTimePicker = Product.DatePicker;
            UpdateProduct.Quantity = Product.Quantity;
            UpdateProduct.FeaturePhoto = Product.FeaturePhoto;
            UpdateProduct.CategoryId = _context.Categories.Where(item => item.CategoryName == Product.CategoryName).Select(item => item.CategoryId).FirstOrDefault();
            _context.Update(UpdateProduct);
            _context.SaveChanges();
            if (Product.FileNames.Count > 0)
            {
                foreach (var File in Product.FileNames)
                {
                    ProductPhoto photo = new ProductPhoto();
                    photo.ProductId = Id;
                    photo.PhotoName = File;
                    _context.ProductPhotos.Add(photo);
                    _context.SaveChanges();
                }
                _context.Dispose();

            }
            return true;
        }

        public User GetUser(string Email)
        {
            return _context.Users.Where(item => item.Email == Email).FirstOrDefault();
        }

        public void AddUser(RegistrationVM Registration)
        {
            User User = new User();
            User.UserId = Guid.NewGuid().ToString();
            User.Password = BC.HashPassword(Registration.Passwordhash);
            User.Email = Registration.Email;
            _context.Add(User);
            _context.SaveChanges();
            _context.Dispose();

        }

        public bool IsLoginValid(loginVM Login)
        {
            var User = _context.Users.Where(item => item.Email == Login.Email).FirstOrDefault();
            if (User == null || !BC.Verify(Login.Passwordhash, User.Password))
            {
                _context.Dispose();
                return false;
            }
            else
            {
                _context.Dispose();

                return true;
            }


        }
        public List<ProductPhoto> GetProductPhoto(int Id)
        {
            var photo = _context.ProductPhotos.Where(item => item.ProductId == Id).ToList();
            _context.Dispose();

            return photo;
        }

        public bool DeletePhoto(string PhotoId)
        {
            var Photo = _context.ProductPhotos.Include(item => item.Product).Where(item => item.PhotoName == PhotoId).FirstOrDefault();
            var Photos = _context.ProductPhotos.Where(item => item.ProductId == Photo.Product.ProductId).ToList();
            if (Photos.Count > 0)
            {
                if (Photo.Product.FeaturePhoto == PhotoId)
                {
                    Photo.Product.FeaturePhoto = Photos.Where(item => item.PhotoName != PhotoId).FirstOrDefault().PhotoName;
                }
            }
            else
            {
                return false;
            }

            _context.ProductPhotos.Remove(Photo);
            _context.SaveChanges();
            _context.Dispose();

            return true;
        }

        public bool IsRestore(int Id)
        {
            var Product = _context.Products.FirstOrDefault(item => item.ProductId == Id);
            Product.Isdeleted = false;
            _context.Products.Update(Product);
            _context.SaveChanges();
            _context.Dispose();

            return true;
        }

        public IEnumerable<Product> GetAllProducts(string SearchValue, int? MinPrice, int? MaxPrice)
        {
            var Products = _context.Products
                .Include(item => item.Category)
                .Include(item => item.ProductPhotos)
                .Where(item => string.IsNullOrEmpty(SearchValue) || item.ProductName.ToLower().Contains(SearchValue.ToLower()));

            if (MinPrice.HasValue && MaxPrice.HasValue)
            {
                Products = Products.Where(item => item.Price >= MinPrice.Value && item.Price <= MaxPrice.Value);
            }
            _context.Dispose();

            return Products.ToList();
        }


        public List<CartItems> GetCartItems( Dictionary<string, int> Cart)
        {
            var ProductIds = Cart.Keys.ToList(); // Extract product IDs from the cart dictionary
            var CartItems = _context.Products
                .Where(item => ProductIds.Contains(item.ProductId.ToString())) // Convert ProductId to string for comparison
                .Select(item => new CartItems
                {
                    CartItemName = item.ProductName,
                    CartItemPrice = (int)item.Price,
                    CartMaxQuantity = (int)item.Quantity,
                    ProductId = item.ProductId,
                    CartFileName = item.FeaturePhoto,
                    CartItemQuantity = Cart[item.ProductId.ToString()] // Get the current quantity from the cart dictionary based on the product ID
                })
                .ToList();
            _context.Dispose();

            return CartItems;

        }
        public Customer AddCustomerDetaile(string FirstName, string LastName, string Email, string Address, int ZipCode, string City)
        {
            var email = _context.Customers.FirstOrDefault(item => item.Email == Email);
            if (email == null)
            {
                Customer Customer = new Customer();
                Customer.FirstName = FirstName;
                Customer.LastName = LastName;
                Customer.Email = Email;
                Customer.Address = Address;
                Customer.City = City;
                Customer.ZipCode = ZipCode;
                _context.Customers.Add(Customer);
                _context.SaveChanges();
                _context.Dispose();

                return Customer;
            }
            else
            {
                return email;
            }
        }
        public bool OrderDetails(List<int> CartQuantity, List<int> CartPrice, List<int> ProductId, int CustomerId, string UniqNumber, string? RazorPayPaymentId, string? RazorpaySignature, int pType)
        {
   
            for (int i = 0; i < CartQuantity.Count; i++)
            {

                Order Order = new Order();
                Order.OrderPrice = (int)CartPrice[i];
                Order.OrderQuantity = (int)CartQuantity[i];
                Order.CustomerId = CustomerId;
                Order.ProductId = ProductId[i];
                var Product = _context.Products.FirstOrDefault(item => item.ProductId == ProductId[i]);
                Product.Quantity = Product.Quantity - CartQuantity[i];
                Order.OrderDate = DateTime.Now;
                Order.UniqOrderId= UniqNumber;  
                Order.Status = 1;
                _context.Products.Update(Product);
                _context.Orders.Add(Order);
                _context.SaveChanges();

            }
            OrderStatusLog OrderStatusLog = new OrderStatusLog();
            OrderStatusLog.OrderStatus =1;
            OrderStatusLog.UniqOrderId =UniqNumber;
            OrderStatusLog.UpDatedDate = DateTime.Now;
            _context.OrderStatusLogs.Add(OrderStatusLog);
            _context.SaveChanges();

            Payment Payment=new Payment();
            Payment.PaymentOrderId= UniqNumber;
            Payment.PaymentType = pType;
            Payment.RazorPaymentId = RazorPayPaymentId;
            Payment.RazorSignature=RazorpaySignature;
            _context.Payments.Add(Payment); 
            _context.SaveChanges();

            OrderProduct OrderProduct= new OrderProduct();
            OrderProduct.CustomerId = CustomerId;
            OrderProduct.OrderUniqId = UniqNumber;
            OrderProduct.OrderDate = DateTime.Now;
            OrderProduct.NotificationBool = true;
            OrderProduct.PaymentId=Payment.PaymentId;
            _context.OrderProducts.Add(OrderProduct);
            _context.SaveChanges(); 
            _context.Dispose();

            return true;
        }

		public int ProductCountSevenDay(int Id)
        {
            return _context.Orders.Where(item=>item.ProductId== Id &&  item.OrderDate>=DateTime.Now.AddDays(-7)).Count(); 
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
