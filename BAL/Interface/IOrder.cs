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
        public List<OrderVM> GetOrderDetails(string SearchValue,string Change,bool BoolValue,int StatusTrack);

        public List<OrderVM> OrderDetailsById(string OrderId);     

        public List<OrderStatus> GetAllStatus();

        public bool IsUpdateOrderStatus(string OrderUniqId, int OrderStatus);

        public bool IsOrderIdExsits(string OrderUniqId);

        public void ReadOrderNotification(string OrderUniqId);

        public void ReadAllNotification();
    }
}
