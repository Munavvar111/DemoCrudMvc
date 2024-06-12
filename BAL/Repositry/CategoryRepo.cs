using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repositry
{
    public class CategoryRepo : ICategory
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<CategoryVM> GetCategories(string categorySearch, string change, bool boolvalue)
        {
            var Category = _context.Categories.Select(item => new CategoryVM
            {
                CategoriesName = item.CategoryName,
                CategoriesDescription = item.CategoryDesc,
                CategoriesID = item.CategoryId,
                IsCategoryDeleted = (bool)item.IsDeleted
            }).Where(item => string.IsNullOrEmpty(categorySearch) || item.CategoriesName.ToLower().Contains(categorySearch) || item.CategoriesDescription.ToLower().Contains(categorySearch)).ToList();

            if (boolvalue)
            {

                switch (change)
                {
                    case "CategoryId":
                        Category = Category.OrderBy(p => p.CategoriesID).ToList();
                        break;
                    case "CategoryName":
                        Category = Category.OrderBy(p => p.CategoriesName).ToList();
                        break;

                    case "Description":
                        Category = Category.OrderBy(p => p.CategoriesDescription).ToList();
                        break;

                }
            }
            else
            {
                switch (change)
                {
                    case "CategoryId":
                        Category = Category.OrderByDescending(p => p.CategoriesID).ToList();
                        break;

                    case "CategoryName":
                        Category = Category.OrderByDescending(p => p.CategoriesName).ToList();
                        break;
                    case "Description   ":
                        Category = Category.OrderByDescending(p => p.CategoriesDescription).ToList();
                        break;


                }
            }
            return Category;
        }


        public bool AddCategory(CategoryVM category)
        {
            Category CategoryData = new Category();
            CategoryData.CategoryDesc = category.CategoriesDescription;
            CategoryData.CategoryName=category.CategoriesName;
            CategoryData.IsDeleted = false;
            _context.Categories.Add(CategoryData);
            _context.SaveChanges();
            return true;
        }

        public bool IsCategoryUsedInProduct(int categoryId)
        {
            var IsUsed = _context.Products.Any(item=>item.CategoryId==categoryId); return IsUsed;   


        }
        public bool RemoveCategory(int categoryId)
        {
            var Categogy = _context.Categories.Where(item=>item.CategoryId == categoryId).FirstOrDefault();
            Categogy.IsDeleted = true;
            _context.Categories.Update(Categogy);
            _context.SaveChanges();
            return true;    
        }

        public bool SetNetCategory(int CategoryId, int netCategoryId)
        {
            var Products = _context.Products.Where(item => item.CategoryId == CategoryId).ToList();
            var Category=_context.Categories.Where(item=>item.CategoryId==CategoryId).FirstOrDefault();
            Category.IsDeleted = true;
            _context.Categories.Update(Category);   
            if (Products.Any())
            {
                foreach (var Product in Products)
                {
                    Product.CategoryId = netCategoryId;
                    _context.Products.Update(Product);  
                }

                _context.SaveChanges();
                return true;
            }

            return false;
        }
        public bool RestoreCategory(int id)
        {
            var Category = _context.Categories.FirstOrDefault(item => item.CategoryId == id);
            Category.IsDeleted = false;
            _context.Categories.Update(Category);   
            _context.SaveChanges();
            return true;
        }
        public CategoryVM GetCategoryById(int id)
        {
            var Category=_context.Categories.FirstOrDefault(item=>item.CategoryId == id);
            var CategoryVm = new CategoryVM();
            CategoryVm.CategoriesID = Category.CategoryId;
            CategoryVm.CategoriesName = Category.CategoryName;
            CategoryVm.CategoriesDescription = Category.CategoryDesc;
            return CategoryVm;
        }

        public bool UpdateCategory(int id, CategoryVM category)
        {
            var UpdateCategory = _context.Categories.FirstOrDefault(item => item.CategoryId == id);
            UpdateCategory.CategoryDesc=category.CategoriesDescription;
            UpdateCategory.CategoryName = category.CategoriesName;
            _context.Categories.Update(UpdateCategory);
            _context.SaveChanges();
            return true;
        }
    }
}
