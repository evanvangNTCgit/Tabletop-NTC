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
        public ReportController(DataContext dc) 
        {
            this._dataService = new DataService(dc);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("MostPurchasedPieces")]
        public IActionResult MostPurchasedPieces() 
        {
            List<PurchaseStatsViewModel> saleLines = this._dataService.GetPiecePurchaseStats();
            saleLines = saleLines.OrderByDescending(sl => sl.PurchasedAmountTotal).ToList();

            return View(saleLines);
        }

        [HttpGet("MostPurchasedSets")]
        public IActionResult MostPurchasedSets()
        {
            List<SetStatsViewModel> saleLines = this._dataService.GetSetPurchaseStats();
            saleLines = saleLines.OrderByDescending(sl => sl.PurchasedAmountTotal).ToList();

            return View(saleLines);
        }
    }
}
