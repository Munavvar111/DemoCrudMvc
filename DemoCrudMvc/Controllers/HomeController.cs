using BAL.Interface;
using DAL.ViewModals;
using DemoCrudMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Text.Encodings.Web;

namespace DemoCrudMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProduct _product;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IProduct product, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
            _product = product;
        }

        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("email");
            if (email != null)
            {
                return RedirectToAction("Product");
            }
            var product = _product.GetAllProducts("", null, null);
            ViewBag.Count = product.Count();
            var a = HttpContext.Session.GetInt32("CurrentCart");
            if (a == null)
            {
                HttpContext.Session.SetInt32("CurrentCart", 0);
            }
            return View();
        }

        public IActionResult GetProductListing(string searchValue, string[] selectedPrices)
        {
            if (selectedPrices.Length == 0)
            {
                var productes = _product.GetAllProducts(searchValue, null, null);
                return PartialView("ProductListingPartial", productes);
            }

            int minPrice = int.MaxValue;
            int maxPrice = int.MinValue;

            foreach (var priceRange in selectedPrices)
            {
                var parts = priceRange.Split('-');
                int min = int.Parse(parts[0]);
                int max = int.Parse(parts[1]);

                if (min < minPrice)
                {
                    minPrice = min;
                }

                if (max > maxPrice)
                {
                    maxPrice = max;
                }
            }

            var products = _product.GetAllProducts(searchValue, minPrice, maxPrice);

            if (HttpContext.Session.GetInt32("CurrentCart") == null)
            {
                HttpContext.Session.SetInt32("CurrentCart", 0);
            }

            return PartialView("ProductListingPartial", products);
        }

        public IActionResult GetProductData(string searchValue, int currentPage, int pageSize, string change, bool boolvalue)
        {

            ViewBag.Categorys = _product.GetAllCategories();
            var data = _product.GetProductList(searchValue, change, boolvalue);
            int totalItems = data.Count();
            //Count TotalPage
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<ProductVM> paginatedData = data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;

            ViewBag.CurrentPage = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalEntries = totalItems;

            return PartialView("productPartial", paginatedData);
        }


        public IActionResult delete(int id)
        {
            if (_product.DeleteProduct(id))
            {
                return Ok();
            }
            else { return BadRequest(); }
        }

        public IActionResult AddTask()
        {
            var email = HttpContext.Session.GetString("email");
            if (email == null)
            {
                return RedirectToAction("index", "login");
            }
            ViewBag.Categorys = _product.GetAllCategories();

            return View();
        }

        public IActionResult Product()
        {
            var email = HttpContext.Session.GetString("email");
            if (email == null)
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

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in product.Files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        product.FileNames.Add(uniqueFileName);
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

        public IActionResult updateProduct(int id)
        {
            var email = HttpContext.Session.GetString("email");
            if (email == null)
            {
                return RedirectToAction("index", "login");
            }
            var getProduct = _product.GetProductById(id);
            var getProductFile = _product.getProductPhoto(id); ;
            ViewBag.Categorys = _product.GetAllCategories();
            var product = new ProductVM();
            product.ProductId = id;
            product.UniqueNo = getProduct.UniqueNo;
            product.ProductName = getProduct.ProductName;
            product.DatePicker = getProduct.DateTimePicker.Value;
            product.Quantity = (int)getProduct.Quantity;
            product.featurePhoto = getProduct.FeaturePhoto;
            foreach (var file in getProductFile)
            {
                product.FileNames.Add(file.PhotoName);
            }
            product.ProductDescription = getProduct.Description;
            product.CategoryName = getProduct.Category.CategoryName;
            product.Price = (int)getProduct.Price;
            return View(product);
        }

        public async Task<IActionResult> ProductUpdateAsync(int id, ProductVM product, string featurePhoto)
        {
            if (featurePhoto == null)
            {
                product.featurePhoto = product.Files.FirstOrDefault().FileName;
            }
            else
            {

                product.featurePhoto = featurePhoto;
            }
            ViewBag.Categorys = _product.GetAllCategories();
            if (!ModelState.IsValid)
            {
                product.ProductId = id;
                var getProductFile = _product.getProductPhoto(id); ;
                foreach (var file in getProductFile)
                {
                    product.FileNames.Add(file.PhotoName);
                }
                return View("updateProduct", product);
            }
            else
            {

                if (product.Files == null || !product.Files.Any())
                {
                    if (_product.UpdateProduct(id, product))
                    {
                        TempData["SuccessMessage"] = "Product Update Succefully!!!";
                        return RedirectToAction("updateProduct", new { id = id });
                    }
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in product.Files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        product.FileNames.Add(uniqueFileName);
                    }
                }
                if (_product.UpdateProduct(id, product))
                {
                    TempData["SuccessMessage"] = "Product Update Succefully!!!";
                    return RedirectToAction("updateProduct", new { id = id });
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

        public IActionResult restore(int id)
        {
            if (_product.IsRestore(id))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult ProductDetails(int id)
        {
            var productDetails = _product.GetProductById(id);
            ProductVM vm = new ProductVM();
            return View(productDetails);
        }
        public IActionResult AddToCart()
        {
            return View();
        }
        public IActionResult GetCartDetails([FromBody] Dictionary<string, int> cart)
        {
            //int[] a = 21;
            var CartId = _product.GetCartItems(cart);
            return PartialView("CartPartial",CartId);
        }
        public IActionResult UpdateCartItem(int quantity, int totalPrice, int productId)
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult CheckOut(List<int> productID, List<int> CartPrice, List<int> CartQuantity, List<string> Cartname,List<int> quantity)
        {
            List<CartItems> cartItems = new List<CartItems>();
            for (int i = 0; i < productID.Count; i++)
            {
                CartItems cartItem = new CartItems(); // Instantiate a new CartItems object
                cartItem.ProductId = productID[i];
                cartItem.CartItemQuantity = quantity[i];
                cartItem.CartItemName = Cartname[i];
                cartItem.CartItemPrice = CartPrice[i];
                cartItem.CartFileName = _product.getProductPhoto(productID[i]).FirstOrDefault().PhotoName;

                cartItems.Add(cartItem);
            }
            return View(cartItems);
        }
        public IActionResult PlaceOrder(string FirstName, string LastName, string Email, string Address, int ZipCode, string City, List<int> CartQuantity, List<int> CartPrice, List<int> ProductId)
        {
            var customerId = _product.AddCustomerDetaile(FirstName, LastName, Email, Address, ZipCode, City);
            var uniqNumber = Guid.NewGuid().ToString();
            var orderDetails = _product.OrderDetails(CartQuantity, CartPrice, ProductId, customerId.CustomerId, uniqNumber);
            if (orderDetails)
            {
                var orderTrackingUrl = Url.Action("TrackOrder", "Order", new { id = uniqNumber }, protocol: HttpContext.Request.Scheme);

                var body = $@"
                             <h1>Thank you for your order!</h1>
                             <p>Your order has been successfully placed. You can track your order using the link below:</p>
                             <p><a href='{HtmlEncoder.Default.Encode(orderTrackingUrl)}'>Track your order</a></p>
                             <p>Thank you for shopping with us!</p>
                             <p>Best regards,</p>
                             <p>Your Company Name</p>";
                if (_emailService.IsSendEmail(customerId.Email, "Thank You For Order", body))
                {
                    return RedirectToAction("OrderSuccessful", "Order", new { OrderUniqId =uniqNumber});
                }
                else
                {
					TempData["Error"] = "Email Send UnSuccessFull";
				}
				TempData["SuccessMessage"] = "Order Send SuccessFully";
			}

			return RedirectToAction("Index");
        }

        public IActionResult CartViewBag()
        {
            var currentCount = HttpContext.Session.GetInt32("CurrentCart");
            ViewBag.CartCount = currentCount + 1;
            HttpContext.Session.SetInt32("CurrentCart", (int)(currentCount + 1));
            return Json(new { newCount = ViewBag.CartCount });
        }

    }
}