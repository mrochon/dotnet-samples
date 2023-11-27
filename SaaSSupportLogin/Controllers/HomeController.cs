using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSSupportLogin.Models;
using System.Diagnostics;

namespace SaaSSupportLogin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            // Get the name of the tenant/domain to which you want to login to; here presuming it is meraridom.com
            // Pass it to middleware
            HttpContext.Items.Add("targetDomain", "meraridom.com");
            // Pass current user's email address as login_hint
            var props = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("Index", "Home"),
                Parameters =
                {
                    { "login_hint", User.Claims.First(c => c.Type == "preferred_username") }
                }
            };
            return Challenge(props);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}