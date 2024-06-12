using BAL.Interface;
using DAL.ViewModals;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace DemoCrudMvc.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory _category;
        private readonly IProduct _product;
        public CategoryController(ICategory category, IProduct product)
        {
            _product = product;
            _category = category;
        }
        public IActionResult Index()
        {
            ViewBag.Categories = _product.GetAllCategories();
            var Email = HttpContext.Session.GetString("email");
            if (Email == null)
            {
                return RedirectToAction("index", "login");
            }
            return View();
        }

        public IActionResult GetCategory(string SearchValue, int CurrentPage, int PageSize, string Change, bool Boolvalue)
        {
            ViewBag.Categories = _product.GetAllCategories();

            var Data = _category.GetCategories(SearchValue, Change, Boolvalue);
            int TotalItems = Data.Count();
            //Count TotalPage
            int TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
            List<CategoryVM> paginatedData = Data.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            ViewBag.totalPages = TotalPages;

            ViewBag.CurrentPage = CurrentPage;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalEntries = TotalItems;

            return PartialView("CategoryPartial", paginatedData);
        }

        public IActionResult AddCategory()
        {
            var Email = HttpContext.Session.GetString("email");
            if (Email == null)
            {
                return RedirectToAction("index", "login");
            }
            return View();
        }

        public IActionResult IsCategoryUsedInProduct(int CategoryId)
        {
            ViewBag.CategoryId = CategoryId;
            if (_category.IsCategoryUsedInProduct(CategoryId))
            {
                return Ok(new { message = "good", categoryId = CategoryId, isUsed = true });
            }
            else
            {
                return Json(new { isUsed = false });
            }

        }

        [HttpPost]
        public IActionResult AddCategory(CategoryVM Category)
        {
            if (_category.AddCategory(Category))
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Please Something Want Wrong!!!";
                return RedirectToAction("AddTask");
            }
        }

        public IActionResult DeleteCategory(int CategoryId)
        {
            if (_category.RemoveCategory(CategoryId))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        public IActionResult SetNewCategory(int CategoryId,int NewCategoryId)
        {
            if (_category.SetNetCategory(CategoryId,NewCategoryId))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult RestoreCategory(int id)
        {
            if (_category.RestoreCategory(id))
            {
                return Ok();
            }   
            else
            {
                return BadRequest();
            }
        }

        public IActionResult UpdateCategory(int Id)
        {
            var Email = HttpContext.Session.GetString("email");
            if (Email == null)
            {
                return RedirectToAction("index", "login");
            }
            var GetCategoryFromId=_category.GetCategoryById(Id);
            return View(GetCategoryFromId);
        }

        [HttpPost]
        public IActionResult UpdateCategory(int Id,CategoryVM Category)
        {
            if (_category.UpdateCategory(Id, Category))
            {
                return RedirectToAction("index");
            }
                return RedirectToAction("index");

        }
    }
}
