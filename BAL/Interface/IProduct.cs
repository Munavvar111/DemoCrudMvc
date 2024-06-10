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

        public IEnumerable<Product> GetAllProducts(string searchValue,int? maxValue,int? minValue);

        public List<CartItems> GetCartItems(Dictionary<string, int> cart);

        public Customer AddCustomerDetaile(string FirstName, string LastName, string Email, string Address, int ZipCode, string City);

        public bool OrderDetails(List<int> CartQuantity, List<int> CartPrice, List<int> ProductId, int CustomerId,string uniqNumber);

        public int ProductCountSevenDay(int id);

        public List<OrderProduct> OrderLastTwoHors();

        public List<NotificationVM> GetNotificationTwoHours();
    }
}
