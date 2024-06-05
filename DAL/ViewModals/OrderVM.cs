using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModals
{
    public  class OrderVM
    {
        public int OrderId { get; set; }    

        public string UniqOrderId { get; set; }

        public int ProductId { get; set; }  

        public string ProductName { get; set; } = string.Empty; 

        public DateTime OrderDate { get; set; }   

        public string? Address { get; set; } 

        public int Quantity { get;set; }

        public int Status { get; set; } 

        public int CustomeId { get; set; }

        public string CustomerName { get; set; }    

        public string CureentStatus {  get; set; }

        public string FileName { get; set; } = null!;

        public int Price { get;set; }
       
        public string? CustomerCity { get; set; }   

        public List<OrderStatusLog> StatusLog { get; set; } 
    }
}
