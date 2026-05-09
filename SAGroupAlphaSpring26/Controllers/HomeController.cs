using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SAGroupAlphaSpring26.ApiServices;
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
        public async Task<IActionResult> Store(string sortOrder, string filter, string storeitemtype, int pageNumber)
        public IActionResult Store(string sortOrder, string filter, string storeitemtype, int pageNumber, bool? showOwned)
        {
            // Prevent stacking like: showOwned=false,false,true,... by normalizing to a single value.
            // Query binding to bool? will still take the last value; this is just a guard.
            ViewData["NameSort"] = (sortOrder == "name_asc") ? "name_desc" : "name_asc";
            ViewData["PriceSort"] = (sortOrder == "price_asc") ? "price_desc" : "price_asc";

            ViewBag.ApplicationName = "SA Group Alpha Spring 2026";

            StoreViewModel model = new StoreViewModel();

            // Base lists
            var sets = _dataService.GetAllSets();
            var pieces = new List<Piece>();

            int currentUserId = _dataService.GetUserId(User);
            bool includeOwned = showOwned.HasValue && showOwned.Value;

            // Build pieces list (purchase-ranked, then owned/unowned based on inventory).
            // Do this independently of which store toggle is currently selected.
            if (currentUserId > 0)
            {
                var topUnownedPiecesStats = _dataService.GetTopUnownedPiecePurchaseStats(currentUserId);

                if (includeOwned)
                {
                    var ownedPieces = _dataService.GetUserPieces(currentUserId);

                    // Purchase counts for owned pieces
                    var allPurchaseStats = _dataService.GetPiecePurchaseStats();
                    var purchaseDict = allPurchaseStats.ToDictionary(x => x.Piece.Id, x => x.TotalPurchased);

                    var ownedStats = ownedPieces
                        .Select(p => new PurchaseStatsViewModel
                        {
                            Piece = p,
                            TotalPurchased = purchaseDict.ContainsKey(p.Id) ? purchaseDict[p.Id] : 0,
                            PurchasedAmountTotal = 0m
                        })
                        .OrderByDescending(x => x.TotalPurchased)
                        .ToList();

                    var combined = ownedStats.Concat(topUnownedPiecesStats)
                        .GroupBy(x => x.Piece.Id)
                        .Select(g => g.First())
                        .OrderByDescending(x => x.TotalPurchased)
                        .ToList();

                    pieces = combined.Select(x => x.Piece).ToList();
                    ViewData["PiecePurchaseCounts"] = combined.ToDictionary(x => x.Piece.Id, x => x.TotalPurchased);
                }
                else
                {
                    pieces = topUnownedPiecesStats.Select(x => x.Piece).ToList();
                    ViewData["PiecePurchaseCounts"] = topUnownedPiecesStats.ToDictionary(x => x.Piece.Id, x => x.TotalPurchased);
                }
            }
            else
            {
                // Not logged in or cannot determine user: fallback to all pieces.
                pieces = _dataService.GetPieces();
            }

            // Apply text filter to both lists all the time.
            if (!string.IsNullOrEmpty(filter))
            {
                pieces = pieces.Where(p => p.Name.ToUpper().Contains(filter.ToUpper())).ToList();
                sets = sets.Where(s => s.Name.ToUpper().Contains(filter.ToUpper())).ToList();
            }

            // Apply sorting independently to both lists.
            switch (sortOrder)
            {
                case "name_asc":
                    model.Pieces = pieces.OrderBy(p => p.Name).ToList();
                    model.Sets = sets.OrderBy(s => s.Name).ToList();
                    break;
                case "name_desc":
                    model.Pieces = pieces.OrderByDescending(p => p.Name).ToList();
                    model.Sets = sets.OrderByDescending(s => s.Name).ToList();
                    break;
                case "price_desc":
                    model.Pieces = pieces.OrderByDescending(p => p.Price).ToList();
                    model.Sets = sets.OrderByDescending(s => s.Price).ToList();
                    break;
                case "price_asc":
                    model.Pieces = pieces.OrderBy(p => p.Price).ToList();
                    model.Sets = sets.OrderBy(s => s.Price).ToList();
                    break;
                default:
                    model.Pieces = pieces;
                    model.Sets = sets;
                    break;
            }


            switch (storeitemtype)
            {
                case "sets":
                    model.Pieces.Clear();
                    break;
                case "pieces":
                    model.Sets.Clear();
                    break;
                case "all":
                    model.Sets = model.Sets;
                    model.Pieces = model.Pieces;
                    break;
                default:
                    break;
            }

            (sets, pieces) = await ConvertPrices(sets, pieces);

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            int pageSize = 4;
            var PaginatedStoreModel = new PaginatedStoreViewModel(PaginatedSetList.Create(model.Sets, pageNumber, pageSize), PaginatedPieceList.Create(model.Pieces, pageNumber, pageSize));
            PaginatedStoreModel.UserCurrency = Request.Cookies["UserCurrencyValue"] ?? "usd";
            return View(PaginatedStoreModel);
        }

        private async Task<(List<Set>, List<Piece>)> ConvertPrices(List<Set> sets, List<Piece> pieces)
        {
            try
            {
                List<Set> convertedSets = sets;
                List<Piece> convertedPieces = pieces;
                string cookieValue = Request.Cookies["UserCurrencyValue"] ?? "usd";
                if (sets.Count > 0 && sets != null)
                {
                    convertedSets = await CurrencyConverter.GetStoreItemsPriceConverted(sets, cookieValue);
                }

                if (pieces.Count > 0 && pieces != null)
                {
                    convertedPieces = await CurrencyConverter.GetStoreItemsPriceConverted(pieces, cookieValue);
                }

                return (convertedSets, convertedPieces);
            }
            catch (CurrencyCallException)
            {
                return (sets, pieces); // API not working just return the original list.
            }
            catch
            {
                throw;
            }
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
