using MC.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MC.Website.Controllers
{
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
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Hello([FromRoute] int? id, [FromQuery] string name)
        {
            ViewBag.Id = id;
            ViewBag.Name = name;
            return View();
        }

        public IActionResult Calculator([FromQuery] double? a, [FromQuery] double? b)
        {
            double? perimeter, aria;
            perimeter = 2 * a + 2 * b;
            aria = a * b;

            ViewBag.A = a;
            ViewBag.B = b;
            ViewBag.Perimeter = perimeter;
            ViewBag.Aria = aria;

            return View();
        }
    }
}