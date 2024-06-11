using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModals
{
    public class MerchantOrder
    {
        public string OrderId { get; set; } 

        public string Razorpaykey { get; set; } 

        public int Amount { get; set; } 

        public string FirstName { get; set; }    

        public string LastName { get; set; }    

        public List<int> CartQuantity { get;set; }  

        public List<int> CartPrice { get;set; }

        public List<int> ProductId { get; set; }


        public string Email { get; set; }   

        public string Address { get; set; } 

        public string Discription { get; set; } 
    }
}
