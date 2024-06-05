using BAL.Interface;
using DAL.ViewModals;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DemoCrudMvc.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrder _order;

        public OrderController(IOrder order)
        {
            _order=order;   
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetOrderDetail(string searchValue,int currentPage,int pageSize,string Change,bool boolvalue)
        {
            var getOrderDetails=_order.GetOrderDetails(searchValue,Change,boolvalue);
            int totalItems = getOrderDetails.Count();
            //Count TotalPage
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<OrderVM> paginatedData = getOrderDetails.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;

            ViewBag.CurrentPage = currentPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalEntries = totalItems;
            return PartialView("OrderPartial",paginatedData);
        }

        public IActionResult TrackOrder()
        {
            return View();  
        }

        public IActionResult OrderDetails(string id)
        {
            ViewBag.Status = _order.GetAllStatus();
            var a =_order.OrderDetailsById(id);
            return View(a);  
        }
    }
}
