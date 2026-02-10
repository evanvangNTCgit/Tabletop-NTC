using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Models;

namespace SAGroupAlphaSpring26.Controllers
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
            ViewBag.ApplicationName = "SA Group Alpha Spring 2026";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Store() 
        {
            ViewBag.ApplicationName = "SA Group Alpha Spring 2026";

            Piece Evan = new Piece
            {
                Id = 1,
                Name = "Evan",
                Description = "A piece of Evan.",
                Price = 19.99m
            };

            return View(Evan);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
