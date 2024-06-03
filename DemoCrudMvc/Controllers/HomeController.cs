using BAL.Interface;
using DAL.ViewModals;
using DemoCrudMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace DemoCrudMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProduct _product;


        public HomeController(ILogger<HomeController> logger, IProduct product)
        {
            _logger = logger;
            _product = product;
        }

        public IActionResult Index()
        {
            var product=_product.GetAllProducts();
            return View(product);
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

        public async Task<IActionResult> ProductUpdateAsync(int id, ProductVM product,string featurePhoto)
        {
            if (featurePhoto == null)
            {
                product.featurePhoto = product.Files.FirstOrDefault().FileName;
            }
            else
            {

            product.featurePhoto=featurePhoto;
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
            var productDetails=_product.GetProductById(id);
            ProductVM vm = new ProductVM();
            //vm.ProductId = id;
            //vm.ProductName=productDetails.ProductName;  
            //vm.featurePhoto=productDetails.FeaturePhoto;
            //vm.ProductDescription = productDetails.Description;
            //vm.CategoryName = productDetails.Category.CategoryName;
            //vm.FileNames = productDetails.ProductPhotos..ToList();
            return View(productDetails);
        }
        public IActionResult AddToCart()    
        {
            return View();
        }
        public IActionResult GetCartDetails(int[] id)
        {
            var CartId = _product.GetCartItems(id);
            return PartialView("CartPartial",CartId);
        }

    }
}