using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModals;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repositry
{
    public class OrderRepo:IOrder
    {
        private readonly ApplicationDbContext _context;

        public OrderRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<OrderVM> GetOrderDetails(string SearchValue,string change,bool boolValue)
        {

            var ordersDetail = _context.OrderProducts.Include(item=>item.Customer).Select(item => new OrderVM
            {
                CustomeId = item.CustomerId,
                UniqOrderId=item.OrderUniqId,
                Address=item.Customer.Address,
                CustomerName=item.Customer.FirstName+"-"+item.Customer.LastName,
                OrderDate=_context.Orders.FirstOrDefault(items=>items.UniqOrderId==item.OrderUniqId).OrderDate,
                Status=_context.Orders.FirstOrDefault(items => items.UniqOrderId == item.OrderUniqId).Status,

            }).ToList();
                              
                               
            return ordersDetail;
        }

        public List<OrderVM> OrderDetailsById(string OrderId)
        {
            var a = _context.Orders.Include(item => item.Product).Include(item => item.Customer).Where(item => item.UniqOrderId == OrderId).Select(item => new OrderVM
            {
                OrderId = item.OrderId,
                OrderDate = item.OrderDate,
                Address = item.Customer.Address,
                CustomerName = item.Customer.FirstName + "-" + item.Customer.LastName,
                ProductName = item.Product.ProductName,
                ProductId = item.Product.ProductId,
                FileName=item.Product.FeaturePhoto,
                Price= item.OrderPrice,   
                Quantity = item.OrderQuantity,
                Status = item.Status,
                CureentStatus = _context.OrderStatuses.FirstOrDefault(items => items.StatusId == item.Status).StatusName,
                CustomeId = item.CustomerId,
                CustomerCity=item.Customer.City,
                StatusLog=_context.OrderStatusLogs.Where(item=>item.UniqOrderId==OrderId).ToList()
            }).ToList();
            return a;
        }

        public List<OrderStatus> GetAllStatus()
        {
            return _context.OrderStatuses.ToList(); 
        }
    }
}
