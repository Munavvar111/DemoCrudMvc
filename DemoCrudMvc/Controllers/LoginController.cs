using BAL.Interface;
using DAL.ViewModals;
using Microsoft.AspNetCore.Mvc;

namespace DemoCrudMvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IProduct _product;

        public LoginController(IProduct product)
        {
            _product = product;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registration(RegistrationVM registration)
        {
            var user = _product.GetUser(registration.Email);
            if (user == null)
            {
                _product.AddUser(registration);
                TempData["SuccessMessage"] = "Create Succefully";
            }
            else
            {
                TempData["Error"] = "Its Already In Exists ";
            }
            return RedirectToAction("Registration", "Login");
        }

        public IActionResult Login(loginVM loginVM)
        {
            if(_product.IsLoginValid(loginVM))
            {
                TempData["SuccessMessage"] = "Login Succefully";
                HttpContext.Session.SetString("email",loginVM.Email);
                return RedirectToAction("Product", "Home");
            }
            else
            {
                TempData["Error"] = "Login Unsuccefully!!";
                return RedirectToAction("Index", "Login");

            }
        }
        public IActionResult LogOut() {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");   
        }
    }
}
