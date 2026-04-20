using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Models;
using SAGroupAlphaSpring26.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace SAGroupAlphaSpring26.Controllers
{
    [Route("")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppConfig _appConfig;

        private readonly DataService _dataService;

        public HomeController(IOptions<AppConfig> appConfigWrapper, ILogger<HomeController> logger, DataService dataService)
        {
            _appConfig = appConfigWrapper.Value;
            _dataService = dataService;
            _logger = logger;
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.ApplicationName = "SA Group Alpha Spring 2026";

            //-------------------------------------------------------------------------
            // Removed the hardcoded userId and replaced with dynamic userID retrieval.
            //int userId = 1;
            //-------------------------------------------------------------------------

            // Checks if user is authenticated, if not redirects them to sign in.
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("sign-in", "Account");
            }

            // Gets user ID from cookies as a string.
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Check if user id exists and parse to an integer.
            if (int.TryParse(userIdString, out int userId))
            {
                var model = _dataService.GetSessions(userId);
                return View(model);
            }

            // if User ID is not found redirect to the sign in page.
            return RedirectToAction("sign-in", "Account");
        }

        [Route("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Store")]
        public IActionResult Store(string sortOrder)
        {
            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "piece_desc" : "";

            ViewBag.ApplicationName = "SA Group Alpha Spring 2026";

            StoreViewModel model = new StoreViewModel();
            //{
            //    Pieces = this._dataService.GetPieces(),
            //    Sets = this._dataService.GetAllSets()
            //};

            // Get the sets and pieces once...
            var pieces = _dataService.GetPieces();
            var sets = _dataService.GetAllSets();

            switch(sortOrder)
                {
                case "piece_desc":
                    model.Pieces = pieces.OrderBy(p => p.Name).ToList();
                    break;
                case "piece_asc":
                    model.Pieces = pieces.OrderBy(p => p.Name).ToList();
                    break;
                default:
                    model.Pieces = pieces;
                    model.Sets = sets;
                break;
            }

            return View(model);
        }

        // Gets a specific piece by ID.
        [Route("Piece/{id}")]
        public IActionResult Piece(int id)
        {
            var model = _dataService.GetPiece(id);
            return View(model);
        }

        // Gets a specific set by ID.
        [Route("Set/{id}")]
        public IActionResult Set(int id)
        {
            var model = _dataService.GetSet(id);
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
