using DAL.ViewModals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface ICategory
    {
        public List<CategoryVM> GetCategories(string productName, string change, bool boolvalue);

        public bool IsCategoryUsedInProduct(int categoryId);
        public bool AddCategory(CategoryVM category);

        public bool RemoveCategory(int categoryId);
        public bool SetNetCategory(int CategoryId,int  netCategoryId);

        public bool RestoreCategory(int id);

        public CategoryVM GetCategoryById(int id);

        public bool UpdateCategory(int id,CategoryVM category); 
    }
}
