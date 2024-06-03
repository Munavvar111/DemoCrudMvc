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
            var category = _context.Categories.Select(item => new CategoryVM
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
                        category = category.OrderBy(p => p.CategoriesID).ToList();
                        break;
                    case "CategoryName":
                        category = category.OrderBy(p => p.CategoriesName).ToList();
                        break;

                    case "Description":
                        category = category.OrderBy(p => p.CategoriesDescription).ToList();
                        break;

                }
            }
            else
            {
                switch (change)
                {
                    case "CategoryId":
                        category = category.OrderByDescending(p => p.CategoriesID).ToList();
                        break;

                    case "CategoryName":
                        category = category.OrderByDescending(p => p.CategoriesName).ToList();
                        break;
                    case "Description   ":
                        category = category.OrderByDescending(p => p.CategoriesDescription).ToList();
                        break;


                }
            }
            return category;
        }


        public bool AddCategory(CategoryVM category)
        {
            Category category1 = new Category();
            category1.CategoryDesc = category.CategoriesDescription;
            category1.CategoryName=category.CategoriesName;
            category1.IsDeleted = false;
            _context.Categories.Add(category1);
            _context.SaveChanges();
            return true;
        }

        public bool IsCategoryUsedInProduct(int categoryId)
        {
            var isUsed = _context.Products.Any(item=>item.CategoryId==categoryId); return isUsed;   


        }
        public bool RemoveCategory(int categoryId)
        {
            var category = _context.Categories.Where(item=>item.CategoryId == categoryId).FirstOrDefault();
            category.IsDeleted = true;
            _context.Categories.Update(category);
            _context.SaveChanges();
            return true;    
        }

        public bool SetNetCategory(int CategoryId, int netCategoryId)
        {
            var products = _context.Products.Where(item => item.CategoryId == CategoryId).ToList();
            var category=_context.Categories.Where(item=>item.CategoryId==CategoryId).FirstOrDefault();
            category.IsDeleted = true;
            _context.Categories.Update(category);   
            if (products.Any())
            {
                foreach (var product in products)
                {
                    product.CategoryId = netCategoryId;
                    _context.Products.Update(product);  
                }

                _context.SaveChanges();
                return true;
            }

            return false;
        }
        public bool RestoreCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(item => item.CategoryId == id);
            category.IsDeleted = false;
            _context.Categories.Update(category);   
            _context.SaveChanges();
            return true;
        }
        public CategoryVM GetCategoryById(int id)
        {
            var category=_context.Categories.FirstOrDefault(item=>item.CategoryId == id);
            var CategoryVm = new CategoryVM();
            CategoryVm.CategoriesID = category.CategoryId;
            CategoryVm.CategoriesName = category.CategoryName;
            CategoryVm.CategoriesDescription = category.CategoryDesc;
            return CategoryVm;
        }

        public bool UpdateCategory(int id, CategoryVM category)
        {
            var updateCategory = _context.Categories.FirstOrDefault(item => item.CategoryId == id);
            updateCategory.CategoryDesc=category.CategoriesDescription;
            updateCategory.CategoryName = category.CategoriesName;
            _context.Categories.Update(updateCategory);
            _context.SaveChanges();
            return true;
        }
    }
}
