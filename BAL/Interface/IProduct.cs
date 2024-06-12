using DAL.DataModels;
using DAL.ViewModals;
using Microsoft.AspNetCore.Http;
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

        public Product GetProductById(int Id);  

        public bool UpdateProduct(int Id, ProductVM Product);

        public bool DeleteProduct(int Id);

        public User GetUser(string Email);

        public void AddUser(RegistrationVM Registration);

        public bool IsLoginValid(loginVM Login);

        public List<ProductPhoto> GetProductPhoto(int Id);

        public bool DeletePhoto(string PhotoId);

        public bool IsRestore(int Id);

        public IEnumerable<Product> GetAllProducts(string SearchValue,int? MaxValue,int? MinValue);

        public List<CartItems> GetCartItems(Dictionary<string, int> Cart);

        public Customer AddCustomerDetaile(string FirstName, string LastName, string Email, string Address, int ZipCode, string City);

        public bool OrderDetails(List<int> CartQuantity, List<int> CartPrice, List<int> ProductId, int CustomerId,string UniqNumber, string? RazorPayPaymentId, string? RazorpaySignature,int PayType);

        public int ProductCountSevenDay(int Id);

        public List<OrderProduct> OrderLastTwoHors();

        public List<NotificationVM> GetNotificationTwoHours();

        //Task<MerchantOrder> ProcessMerchantOrder(CartItems cartItems);

        //Task<string> CompleteOrderProcess(HttpContextAccessor httpContextAccessor);
    }
}
