using BAL.Interface;
using DAL.ViewModals;
using DemoCrudMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using DAL.DataModels;
//using System.Web.Mvc;

namespace DemoCrudMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProduct _product;
        private readonly IOrder _order;
        private readonly IUploadFile _uploadFile;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IProduct product, IEmailService emailService,IOrder order,IUploadFile uploadFile)
        {
            _logger = logger;
            _order= order;  
            _uploadFile=uploadFile; 
            _emailService = emailService;
            _product = product;
        }

        public IActionResult Index()
        {

            var Email = HttpContext.Session.GetString("email");
            if (Email != null)
            {
                return RedirectToAction("Product");
            }
            var Product = _product.GetAllProducts("", null, null);
            ViewBag.Count = Product.Count();
            var a = HttpContext.Session.GetInt32("CurrentCart");
            if (a == null)
            {
                HttpContext.Session.SetInt32("CurrentCart", 0);
            }
            ViewBag.OrderNotification = _product.GetNotificationTwoHours();
            return View();
        }

        public IActionResult GetProductListing(string searchValue, string[] selectedPrices)
        {
            if (selectedPrices.Length == 0)
            {
                var Productes = _product.GetAllProducts(searchValue, null, null);
                return PartialView("ProductListingPartial", Productes);
            }

            int MinPrice = int.MaxValue;
            int MaxPrice = int.MinValue;

            foreach (var PriceRange in selectedPrices)
            {
                var Parts = PriceRange.Split('-');
                int Min = int.Parse(Parts[0]);
                int Max = int.Parse(Parts[1]);

                if (Min < MinPrice)
                {
                    MinPrice = Min;
                }

                if (Max > MaxPrice)
                {
                    MaxPrice = Max;
                }
            }

            var Products = _product.GetAllProducts(searchValue, MinPrice, MaxPrice);

            if (HttpContext.Session.GetInt32("CurrentCart") == null)
            {
                HttpContext.Session.SetInt32("CurrentCart", 0);
            }

            return PartialView("ProductListingPartial", Products);
        }

        public IActionResult GetProductData(string searchValue, int currentPage, int pageSize, string change, bool boolvalue)
        {

            ViewBag.Categorys = _product.GetAllCategories();
            var Data = _product.GetProductList(searchValue, change, boolvalue);
            int TotalItems = Data.Count();
            //Count TotalPage
            int TotalPages = (int)Math.Ceiling((double)TotalItems / pageSize);
            List<ProductVM> PaginatedData = Data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = TotalPages;

            ViewBag.CurrentPage = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalEntries = TotalItems;

            return PartialView("productPartial", PaginatedData);
        }


        public IActionResult Delete(int id)
        {
            if (_product.DeleteProduct(id))
            {
                return Ok();
            }
            else { return BadRequest(); }
        }

        public IActionResult AddTask()
        {
            var Email = HttpContext.Session.GetString("email");
            if (Email == null)
            {
                return RedirectToAction("index", "login");
            }
            ViewBag.Categorys = _product.GetAllCategories();

            return View();
        }

        public IActionResult Product()
        {
            var Email = HttpContext.Session.GetString("email");
            if (Email == null)
            {
                return RedirectToAction("index", "login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TaskAddAsync(ProductVM product)
        {
            ViewBag.Categorys = _product.GetAllCategories();
            if (!ModelState.IsValid)
            {
                return View("AddTask", product);
            }
            else
            {
                if (product.Files == null || !product.Files.Any())
                {
                    ModelState.AddModelError("Files", "Please upload at least one file.");
                    return View("AddTask", product);
                }

                var UploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Ensure the uploads folder exists
                if (!Directory.Exists(UploadsFolder))
                {
                    Directory.CreateDirectory(UploadsFolder);
                }

                foreach (var File in product.Files)
                {
                    if (File != null && File.Length > 0)
                    {
                      var UniqueFileName= _uploadFile.UploadFile(File);

                        product.FileNames.Add(UniqueFileName);
                    }
                }
                if (_product.IsAddTheProduct(product))
                {
                    TempData["SuccessMessage"] = "Product Add Succefully!!!";
                    return RedirectToAction("Product");
                }

                return View("AddTask");
            }
        }

        public IActionResult UpdateProduct(int Id)
        {
            string Email = HttpContext.Session.GetString("email");
            if (Email == null)
            {
                return RedirectToAction("index", "login");
            }
            var GetProduct = _product.GetProductById(Id);
            var GetProductFile = _product.GetProductPhoto(Id); ;
            ViewBag.Categorys = _product.GetAllCategories();
            var Product = new ProductVM();
            Product.ProductId = Id;
            Product.UniqueNo = GetProduct.UniqueNo;
            Product.ProductName = GetProduct.ProductName;
            Product.DatePicker = GetProduct.DateTimePicker.Value;
            Product.Quantity = (int)GetProduct.Quantity;
            Product.FeaturePhoto = GetProduct.FeaturePhoto;
            foreach (var File in GetProductFile)
            {
                Product.FileNames.Add(File.PhotoName);
            }
            Product.ProductDescription = GetProduct.Description;
            Product.CategoryName = GetProduct.Category.CategoryName;
            Product.Price = (int)GetProduct.Price;
            return View(Product);
        }

        public async Task<IActionResult> ProductUpdateAsync(int Id, ProductVM Product, string FeaturePhoto)
        {
            if (FeaturePhoto == null)
            {
                Product.FeaturePhoto = Product.Files.FirstOrDefault().FileName;
            }
            else
            {

                Product.FeaturePhoto = FeaturePhoto;
            }
            ViewBag.Categorys = _product.GetAllCategories();
            if (!ModelState.IsValid)
            {
                Product.ProductId = Id;
                var GetProductFile = _product.GetProductPhoto(Id); ;
                foreach (var file in GetProductFile)
                {
                    Product.FileNames.Add(file.PhotoName);
                }
                return View("UpdateProduct", Product);
            }
            else
            {

                if (Product.Files == null || !Product.Files.Any())
                {
                    if (_product.UpdateProduct(Id, Product))
                    {
                        TempData["SuccessMessage"] = "Product Update Succefully!!!";
                        return RedirectToAction("updateProduct", new { id = Id });
                    }
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var File in Product.Files)
                {
                    if (File != null && File.Length > 0)
                    {
                        var UniqueFileName = _uploadFile.UploadFile(File);

                        Product.FileNames.Add(UniqueFileName);
                    }
                }
                if (_product.UpdateProduct(Id, Product))
                {
                    TempData["SuccessMessage"] = "Product Update Succefully!!!";
                    return RedirectToAction("updateProduct", new { id = Id });
                }

                return View("updateProduct");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult DeletPhoto(string filename)
        {
            if (_product.DeletePhoto(filename))
            {
                return Json(new { success = true });
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult Restore(int Id)
        {
            if (_product.IsRestore(Id))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult ProductDetails(int Id)
        {
            var ProductDetails = _product.GetProductById(Id);
            ViewBag.ProductCount = _product.ProductCountSevenDay(Id);
            return View(ProductDetails);
        }
        public IActionResult AddToCart()
        {
            return View();
        }
        public IActionResult GetCartDetails([FromBody] Dictionary<string, int> cart)
        {
            //int[] a = 21;
            var CartId = _product.GetCartItems(cart);
            return PartialView("CartPartial", CartId);
        }
        public IActionResult UpdateCartItem(int quantity, int totalPrice, int productId)
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult CheckOut(List<int> productID, List<int> CartPrice, List<int> CartQuantity, List<string> Cartname, List<int> quantity)
        {
            List<CartItems> CartItems = new List<CartItems>();
            for (int i = 0; i < productID.Count; i++)
            {
                CartItems CartItem = new CartItems(); // Instantiate a new CartItems object
                CartItem.ProductId = productID[i];
                CartItem.CartItemQuantity = quantity[i];
                CartItem.CartItemName = Cartname[i];
                CartItem.CartItemPrice = CartPrice[i];
                CartItem.CartFileName = _product.GetProductPhoto(productID[i]).FirstOrDefault().PhotoName;

                CartItems.Add(CartItem);
            }
            return View(CartItems);
        }
        public IActionResult PlaceOrder(string FirstName, string LastName, string Email, string Address, int ZipCode, string City, List<int> CartQuantity, List<int> CartPrice, List<int> ProductId, int flexRadioDefault, string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            if (flexRadioDefault == 2)
            {
                if (razorpay_order_id != null && razorpay_payment_id !=null && razorpay_signature!=null)
                {
                    var attributes = new Dictionary<string, string>
                {
                    { "razorpay_payment_id", razorpay_payment_id },
                    { "razorpay_order_id", razorpay_order_id },
                    { "razorpay_signature", razorpay_signature }
                };

                    try
                    {
                        Utils.verifyPaymentSignature(attributes);
                        var CustomerDetails1 = _product.AddCustomerDetaile(FirstName, LastName, Email, Address, ZipCode, City);
                        var OrderDetails1 = _product.OrderDetails(CartQuantity, CartPrice, ProductId, CustomerDetails1.CustomerId,razorpay_order_id, razorpay_payment_id,razorpay_signature,flexRadioDefault);
                        return RedirectToAction("OrderSuccessful", "Order", new { OrderUniqId = razorpay_order_id });
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Payment verification failed: " + ex.Message;
                        return RedirectToAction("CheckOut");
                    }
                }
                var Amount = 0;
                for (int i = 0; i < CartPrice.Count; i++)
                {
                    Amount += CartPrice[i] * CartQuantity[i];
                }
                Dictionary<string, object> Input = new Dictionary<string, object>();
                Input.Add("amount", Amount*100); // this amount should be same as transaction amount
                Input.Add("currency", "INR");
                Input.Add("receipt", "12121");

                string Key = "rzp_test_QhgKuwa09iAtCQ";
                string Secret = "EPN5K5oBZerW2xhW103kenEQ";

                RazorpayClient client = new RazorpayClient(Key, Secret);

                Razorpay.Api.Order Order = client.Order.Create(Input);
                var OrderId = Order["id"].ToString();
                MerchantOrder Merchant = new MerchantOrder();
                Merchant.FirstName = FirstName;
                Merchant.LastName = LastName;
                Merchant.Address = Address;
                Merchant.Email= Email;
                Merchant.OrderId = OrderId;
                Merchant.ProductId = ProductId;
                Merchant.CartPrice = CartPrice;
                Merchant.Amount = Amount;
                Merchant.CartQuantity = CartQuantity;
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 128 // Optionally increase the max depth if needed
                };
                var jsonData = JsonSerializer.Serialize(new { data = Merchant }, options);
                return Content(jsonData, "application/json");
            }
            else
            {

                var CustomerId = _product.AddCustomerDetaile(FirstName, LastName, Email, Address, ZipCode, City);
                var UniqNumber = Guid.NewGuid().ToString();
                var OrderDetails = _product.OrderDetails(CartQuantity, CartPrice, ProductId, CustomerId.CustomerId, UniqNumber,null,null,1);

                if (OrderDetails)
                {
                    var orderTrackingUrl = Url.Action("TrackOrder", "Order", new { id = UniqNumber }, protocol: HttpContext.Request.Scheme);

                    var body = $@"
                             <h1>Thank you for your order!</h1>
                             <p>Your order has been successfully placed. You can track your order using the link below:</p>
                             <p><a href='{HtmlEncoder.Default.Encode(orderTrackingUrl)}'>Track your order</a></p>
                             <p>Thank you for shopping with us!</p>
                             <p>Best regards,</p>
                             <p>Your Company Name</p>";
                    if (_emailService.IsSendEmail(CustomerId.Email, "Thank You For Order", body))
                    {
                        return RedirectToAction("OrderSuccessful", "Order", new { OrderUniqId = UniqNumber });
                    }
                    else
                    {
                        TempData["Error"] = "Email Send UnSuccessFull";
                    }
                    TempData["SuccessMessage"] = "Order Send SuccessFully";
                }

                return RedirectToAction("Index");
            }

        }

        public IActionResult CartViewBag()
        {
            var CurrentCount = HttpContext.Session.GetInt32("CurrentCart");
            ViewBag.CartCount = CurrentCount + 1;
            HttpContext.Session.SetInt32("CurrentCart", (int)(CurrentCount + 1));
            return Json(new { newCount = ViewBag.CartCount });
        }
        public IActionResult GetNotificationData()
        {
            var CountOfNotification = HttpContext.Session.GetInt32("CountNotify");

            HttpContext.Session.SetInt32("CountNotify", 1);


            var OrderLastTwoHours = _product.OrderLastTwoHors();
            var NotificationData = OrderLastTwoHours.Select(item => new
            {

                Product = _order.OrderDetailsById(item.OrderUniqId).First().ProductName.Split(new char[] {' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0],
            item.OrderProductId,
                
                Customer = new
                {
                    item.Customer.FirstName,
                    item.Customer.LastName,

                },
                OrderTime = item.OrderDate.HasValue
                   ? DateTime.Now.TimeOfDay - item.OrderDate.Value.TimeOfDay
                   : TimeSpan.Zero
            }).ToList();
            var Options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 128 // Optionally increase the max depth if needed
            };
            var jsonData = JsonSerializer.Serialize(new { data = NotificationData }, Options);
            return Content(jsonData, "application/json");



        }
        public IActionResult GetNotification()
        {
            List<NotificationVM> LstDataSubmit = new List<NotificationVM>();
            var Options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 128 // Optionally increase the max depth if needed
            };
            LstDataSubmit = _product.GetNotificationTwoHours();
            var JsonData = JsonSerializer.Serialize(new { data = LstDataSubmit }, Options);
            return Content(JsonData, "application/json");

        }
    }
}