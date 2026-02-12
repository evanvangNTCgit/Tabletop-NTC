using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SAGroupAlphaSpring26.Models;
using SAGroupAlphaSpring26.Services;
using System.Diagnostics;

namespace SAGroupAlphaSpring26.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppConfig _appConfig;

        private readonly DataService _dataService;

        public HomeController(IOptions<AppConfig> appConfigWrapper)
        {
            _appConfig = appConfigWrapper.Value;
            _dataService = new DataService();
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.ApplicationName = "SA Group Alpha Spring 2026";

            return View();
        }

        [Route("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Store")]
        public IActionResult Store()
        {
            ViewBag.ApplicationName = "SA Group Alpha Spring 2026";

            return View();
        }

        // Gets a specific piece by ID.
        [Route("Piece/{id}")]
        public IActionResult Piece(int id)
        {
            var model = _dataService.GetPiece(id);
            return View(model);
        }

        [Route("Session/{id}")]
        public IActionResult Session(int id)
            {
            var model = _dataService.GetSession(id);
        return View(model);
        }

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
