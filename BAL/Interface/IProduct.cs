using DAL.DataModels;
using DAL.ViewModals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IProduct
    {
        public List<ProductVM> GetProductList(string productName,string change,bool boolvalue);

        public List<Category> GetAllCategories();

        public bool IsAddTheProduct(ProductVM    product); 

        public Product GetProductById(int id);  

        public bool UpdateProduct(int id, ProductVM product);

        public bool DeleteProduct(int id);

        public User GetUser(string email);

        public void AddUser(RegistrationVM registration);

        public bool IsLoginValid(loginVM login);

        public List<ProductPhoto> getProductPhoto(int id);

        public bool DeletePhoto(string photoId);

        public bool IsRestore(int id);

        public List<Product> GetAllProducts();
    }
}
