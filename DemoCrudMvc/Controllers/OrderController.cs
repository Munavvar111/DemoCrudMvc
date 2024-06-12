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
            var Email = HttpContext.Session.GetString("email");
            if (Email == null)
            {
                return RedirectToAction("index", "login");
            }
            return View();
        }
        public IActionResult GetOrderDetail(string searchValue,int CurrentPage, int PageSize, string Change,bool BoolValue, int StatusTrack)
        {
            var GetOrderDetails=_order.GetOrderDetails(searchValue,Change, BoolValue, StatusTrack);
            int TotalItems = GetOrderDetails.Count();
            //Count TotalPage
            int TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
            List<OrderVM> PaginatedData = GetOrderDetails.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            ViewBag.totalPages = TotalPages;

            ViewBag.CurrentPage = CurrentPage;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalEntries = TotalItems;
            return PartialView("OrderPartial",PaginatedData);
        }

        public IActionResult TrackOrder(string Id)
        {
            if (Id == null)
            {
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index","Home");
            }

            ViewBag.Status = _order.GetAllStatus();
            if(!_order.IsOrderIdExsits(Id)) { 
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index", "Home");

            }
            var Order = _order.OrderDetailsById(Id);
            if (Order == null)
            {
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index","Home");
            }
            return View(Order);
        }

        public IActionResult OrderDetails(string Id)
        {
            if (Id == null)
            {
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index");
            }
            var Email = HttpContext.Session.GetString("email");
            if (Email == null)
            {
                return RedirectToAction("index", "login");
            }
            ViewBag.Status = _order.GetAllStatus();
            var OrderDetails =_order.OrderDetailsById(Id);
            if (OrderDetails == null)
            {
                TempData["Error"] = "Something Is Wrong";
                return RedirectToAction("Index");
            }
            return View(OrderDetails);  
        }
        [HttpPost]
        public IActionResult UpdateStatus(int OrderStatus,string UniqId)
        {
            if (_order.IsUpdateOrderStatus(UniqId, OrderStatus))
            {
                TempData["SuccessMessage"] = "Your Order Shipment Has Been Updated";
                return RedirectToAction("OrderDetails", new {Id =UniqId});
            }
            else
            {
				TempData["SuccessMessage"] = "Your Order Shipment Has Not Been Updated";
				return RedirectToAction("OrderDetails", new {id=UniqId});

            }
        }

        public IActionResult DownloadInvoic(string OrderUniqId)
        {
            var OrderDetails = _order.OrderDetailsById(OrderUniqId);
            return new ViewAsPdf("InvoiceDetails", OrderDetails)
            {
                FileName = "Invoice.pdf"
            };
        }
        public IActionResult OrderSuccessful(string OrderUniqId)
        {
            var OrderDetails = _order.OrderDetailsById(OrderUniqId);
            return View(OrderDetails);  
        }

        public IActionResult ReadOrderNotification(string OrderId)
        {
            if (OrderId != null)
            {
                _order.ReadOrderNotification(OrderId);
            }
            return RedirectToAction("OrderDetails", new { Id = OrderId });
        }
        public IActionResult ReadAllNotification()
        {
            _order.ReadAllNotification();
            return RedirectToAction("Index", "Home");
        }
    }
}
