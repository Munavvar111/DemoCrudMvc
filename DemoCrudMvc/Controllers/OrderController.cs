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
            ViewBag.Status = _order.GetAllStatus();
            var email = HttpContext.Session.GetString("email");
            if (email == null)
            {
                return RedirectToAction("index", "login");
            }
            return View();
        }
        public IActionResult GetOrderDetail(string searchValue,int currentPage,int pageSize,string Change,bool boolvalue,int StatusTrack)
        {
            var getOrderDetails=_order.GetOrderDetails(searchValue,Change,boolvalue,StatusTrack);
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
            if (id == null)
            {
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index","Home");
            }

            ViewBag.Status = _order.GetAllStatus();
            if(!_order.IsOrderIdExsits(id)) { 
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index", "Home");

            }
            var a = _order.OrderDetailsById(id);
            if (a == null)
            {
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index","Home");
            }
            return View(a);
        }

        public IActionResult OrderDetails(string id)
        {
            if (id == null)
            {
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index");
            }
            var email = HttpContext.Session.GetString("email");
            if (email == null)
            {
                return RedirectToAction("index", "login");
            }
            ViewBag.Status = _order.GetAllStatus();
            var a =_order.OrderDetailsById(id);
            if (a == null)
            {
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index");
            }
            return View(a);  
        }
        [HttpPost]
        public IActionResult UpdateStatus(int OrderStatus,string UniqId)
        {
            if (_order.IsUpdateOrderStatus(UniqId, OrderStatus))
            {
                TempData["SuccessMessage"] = "Your Order Shipment Has Been Updated";
                return RedirectToAction("OrderDetails", new {id=UniqId});
            }
            else
            {
				TempData["SuccessMessage"] = "Your Order Shipment Has Not Been Updated";
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
        public IActionResult OrderSuccessful(string OrderUniqId)
        {
            var a = _order.OrderDetailsById(OrderUniqId);
            return View(a);  
        }

        public IActionResult ReadOrderNotification(string orderId)
        {
            if (orderId != null)
            {
                _order.ReadOrderNotification(orderId);
            }
            return RedirectToAction("OrderDetails", new { id = orderId });
        }
        public IActionResult ReadAllNotification()
        {
            _order.ReadAllNotification();
            return RedirectToAction("Index", "Home");
        }
    }
}
