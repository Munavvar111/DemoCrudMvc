using DAL.DataModels;
using DAL.ViewModals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IOrder
    {
        public List<OrderVM> GetOrderDetails(string SearchValue,string change,bool boolValue);

        public List<OrderVM> OrderDetailsById(string id);     

        public List<OrderStatus> GetAllStatus();

        public bool IsUpdateOrderStatus(string OrderUniqId, int OrderStatus);

        public bool IsOrderIdExsits(string OrderUniqId);
    }
}
