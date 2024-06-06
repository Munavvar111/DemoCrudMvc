using BAL.Interface;
using DAL.ViewModals;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
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

        public IActionResult TrackOrder(string id)
        {
            ViewBag.Status = _order.GetAllStatus();
            var a = _order.OrderDetailsById(id);
            return View(a);  
        }

        public IActionResult OrderDetails(string id)
        {
            ViewBag.Status = _order.GetAllStatus();
            var a =_order.OrderDetailsById(id);
            return View(a);  
        }
        [HttpPost]
        public IActionResult UpdateStatus(int OrderStatus,string UniqId)
        {
            if (_order.IsUpdateOrderStatus(UniqId, OrderStatus))
            {
                return RedirectToAction("OrderDetails", new {id=UniqId});
            }
            else
            {
                return RedirectToAction("OrderDetails", new {id=UniqId});

            }
        }

        public IActionResult DownloadInvoic(string OrderUniqId)
        {
            var orderdetails = _order.OrderDetailsById(OrderUniqId);
            return new ViewAsPdf("InvoiceDetails", orderdetails)
            {
                FileName = "Invoice.pdf"
            };
        }
    }
}
