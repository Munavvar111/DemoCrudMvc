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

            return View();
        }

        public IActionResult GetCategory(string searchValue, int currentPage, int pageSize, string change, bool boolvalue)
        {
            ViewBag.Categories = _product.GetAllCategories();

            var data = _category.GetCategories(searchValue, change, boolvalue);
            int totalItems = data.Count();
            //Count TotalPage
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<CategoryVM> paginatedData = data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;

            ViewBag.CurrentPage = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalEntries = totalItems;

            return PartialView("CategoryPartial", paginatedData);
        }

        public IActionResult AddCategory()
        {

            return View();
        }

        public IActionResult IsCategoryUsedInProduct(int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            if (_category.IsCategoryUsedInProduct(categoryId))
            {
                return Ok(new { message = "good", categoryId = categoryId, isUsed = true });
            }
            else
            {
                return Json(new { isUsed = false });
            }

        }

        [HttpPost]
        public IActionResult AddCategory(CategoryVM category)
        {
            if (_category.AddCategory(category))
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Please Something Want Wrong!!!";
                return RedirectToAction("AddTask");
            }
        }

        public IActionResult DeleteCategory(int categoryId)
        {
            if (_category.RemoveCategory(categoryId))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        public IActionResult SetNewCategory(int categoryId,int newCategoryId)
        {
            if (_category.SetNetCategory(categoryId,newCategoryId))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult restoreCategory(int id)
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

        public IActionResult UpdateCategory(int id)
        {
            var getCategoryFromId=_category.GetCategoryById(id);
            return View(getCategoryFromId);
        }

        [HttpPost]
        public IActionResult UpdateCategory(int id,CategoryVM category)
        {
            if (_category.UpdateCategory(id, category))
            {
                return RedirectToAction("index");
            }
                return RedirectToAction("index");

        }
    }
}
