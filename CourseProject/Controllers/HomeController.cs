using System.Diagnostics;
using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            //var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            //if (result.Succeeded)
            //{
            //    var user = await _userManager.FindByNameAsync(username);
            //    var roles = await _userManager.GetRolesAsync(user);

            //    if (roles.Contains("User"))
            //        return RedirectToAction("Index", "Assets");

            return RedirectToAction("Index", "Home"); // or redirect based on role
            //}

            //ModelState.AddModelError("", "Invalid login attempt.");
            //return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Logout()
        //{
        //    //await _signInManager.SignOutAsync();
        //    //return RedirectToAction("Login");
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
