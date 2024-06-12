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
    public class OrderRepo : IOrder
    {
        private readonly ApplicationDbContext _context;

        public OrderRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<OrderVM> GetOrderDetails(string SearchValue, string Change, bool BoolValue,int StatusTrack)
            {

            var ordersDetail = _context.OrderProducts.Include(item => item.Customer).Select(item => new OrderVM
            {
                CustomeId = item.CustomerId,
                UniqOrderId = item.OrderUniqId,
                Address = item.Customer.Address,
                CustomerName = item.Customer.FirstName + "-" + item.Customer.LastName,
                OrderDate = _context.Orders.FirstOrDefault(items => items.UniqOrderId == item.OrderUniqId).OrderDate,
                Status = _context.Orders.FirstOrDefault(items => items.UniqOrderId == item.OrderUniqId).Status,
            }).OrderBy(item => item.Status).Where(item=>(string.IsNullOrEmpty(SearchValue) || item.CustomerName.ToLower().Contains(SearchValue) 
            || item.Address.ToLower().Contains(SearchValue) || item.UniqOrderId.ToLower().Contains(SearchValue)) &&( StatusTrack==0 || item.Status==StatusTrack) ).ToList();
            if (BoolValue)
            {

                switch (Change)
                {
                    case "OrderId":
                        ordersDetail = ordersDetail.OrderBy(p => p.UniqOrderId).ToList();
                        break;

                    case "OrderDate":
                        ordersDetail = ordersDetail.OrderBy(p => p.OrderDate).ToList();
                        break;

                    case "CustomerName":
                        ordersDetail = ordersDetail.OrderBy(p => p.CustomerName).ToList();
                        break;
                    case "Address":
                        ordersDetail = ordersDetail.OrderBy(p => p.Address).ToList();
                        break;
                    case "Status":
                        ordersDetail = ordersDetail.OrderBy(p => p.Status).ToList();
                        break;
                

                }
            }
            else
            {
                switch (Change)
                {
                    case "OrderId":
                        ordersDetail = ordersDetail.OrderByDescending(p => p.OrderId).ToList();
                        break;

                    case "OrderDate":
                        ordersDetail = ordersDetail.OrderByDescending(p => p.OrderDate).ToList();
                        break;

                    case "CustomerName":
                        ordersDetail = ordersDetail.OrderByDescending(p => p.CustomerName).ToList();
                        break;

                    case "Address":
                        ordersDetail = ordersDetail.OrderByDescending(p => p.Address).ToList();
                        break;
                    case "Status":
                        ordersDetail = ordersDetail.OrderByDescending(p => p.Status).ToList();
                        break;
                   
                }
            }
            foreach (var Order in ordersDetail)
            {
                if (Order.Status != null)
                {
                    var status = _context.OrderStatuses.FirstOrDefault(s => s.StatusId == Order.Status);
                    if (status != null)
                    {
                        Order.CureentStatus = status.StatusName;
                    }
                }
            }
            _context.Dispose();

            return ordersDetail;
        }

        public List<OrderVM> OrderDetailsById(string OrderId)
        {
            var OrderDetails = _context.Orders.Include(item => item.Product).Include(item => item.Customer).Where(item => item.UniqOrderId == OrderId).Select(item => new OrderVM
            {
                OrderId = item.OrderId,
                OrderDate = item.OrderDate,
                Address = item.Customer.Address,
                CustomerName = item.Customer.FirstName + "-" + item.Customer.LastName,
                ProductName = item.Product.ProductName,
                ProductId = item.Product.ProductId,
                FileName = item.Product.FeaturePhoto,
                UniqOrderId = item.UniqOrderId,
                Price = item.OrderPrice,
                Quantity = item.OrderQuantity,
                Status = item.Status,
                CureentStatus = _context.OrderStatuses.FirstOrDefault(items => items.StatusId == item.Status).StatusName,
                CustomeId = item.CustomerId,
                CustomerCity = item.Customer.City,
                StatusLog = _context.OrderStatusLogs.Where(item => item.UniqOrderId == OrderId).ToList()
            }).ToList();
            _context.Dispose();

            return OrderDetails;
        }

        public List<OrderStatus> GetAllStatus()
        {
            return _context.OrderStatuses.ToList();
        }

        public bool IsUpdateOrderStatus(string OrderUniqId, int OrderStatus)
        {
            var orders = _context.Orders.Where(o => o.UniqOrderId == OrderUniqId).ToList();
            if (orders.First().Status != OrderStatus)
            {
                if (!orders.Any())
                {
                    // No orders found
                    return false;
                }

                // Update the OrderStatus for each order
                foreach (var order in orders)
                {

                    order.Status = OrderStatus;
                    _context.Orders.Update(order);
                    _context.SaveChanges();
                }

                OrderStatusLog orderStatusLog = new OrderStatusLog();
                orderStatusLog.OrderStatus = OrderStatus;
                orderStatusLog.UniqOrderId = OrderUniqId;
                orderStatusLog.UpDatedDate = DateTime.Now;
                _context.OrderStatusLogs.Add(orderStatusLog);
                _context.SaveChanges();
                _context.Dispose();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsOrderIdExsits(string OrderUniqId) { 
        return _context.Orders.Any(item=>item.UniqOrderId == OrderUniqId);   
        }

        public void ReadOrderNotification(string OrderUniqId)
        {
            var OrderProduct = _context.OrderProducts.FirstOrDefault(item => item.OrderUniqId == OrderUniqId);
            OrderProduct.NotificationBool= false;
            _context.OrderProducts.Update(OrderProduct);
            _context.SaveChanges();
            _context.Dispose();

        }

        public void ReadAllNotification()
        {
            var UnReadNotification = _context.OrderProducts.Where(item => item.OrderDate > DateTime.Now.AddHours(-2) && item.NotificationBool);
            foreach(var Item in UnReadNotification)
            {
                Item.NotificationBool= false;
                _context.OrderProducts.Update(Item); 
            }
            _context.SaveChanges();
            _context.Dispose();

        }
    }
}
