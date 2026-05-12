using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Services;

namespace SAGroupAlphaSpring26.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Report")]
    public class ReportController : Controller
    {
        private readonly DataService _dataService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(DataContext dc, ILogger<ReportController> logger) 
        {
            this._dataService = new DataService(dc);
            this._logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("MostPurchasedPieces")]
        public IActionResult MostPurchasedPieces() 
        {
            try
            {
                List<PurchaseStatsViewModel> saleLines = this._dataService.GetPiecePurchaseStats();
                saleLines = saleLines.OrderByDescending(sl => sl.PurchasedAmountTotal).ToList();

                return View(saleLines);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to show most purchased piece report. {ex.Message}");
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpGet("MostPurchasedSets")]
        public IActionResult MostPurchasedSets()
        {
            try
            {
                List<SetStatsViewModel> saleLines = this._dataService.GetSetPurchaseStats();
                saleLines = saleLines.OrderByDescending(sl => sl.PurchasedAmountTotal).ToList();

                return View(saleLines);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to show most purchased sets report. {ex.Message}");
                return RedirectToAction("Index", "Admin");
            }
        }
    }
}
