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
        public IActionResult Registration(RegistrationVM Registration)
        {
            var User = _product.GetUser(Registration.Email);
            if (User == null)
            {
                _product.AddUser(Registration);
                TempData["SuccessMessage"] = "Create Succefully";
            }
            else
            {
                TempData["Error"] = "Its Already In Exists ";
            }
            return RedirectToAction("Registration", "Login");
        }

        public IActionResult Login(loginVM LoginVM)
        {
            if(_product.IsLoginValid(LoginVM))
            {
                TempData["SuccessMessage"] = "Login Succefully";
                HttpContext.Session.SetString("email",LoginVM.Email);
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
