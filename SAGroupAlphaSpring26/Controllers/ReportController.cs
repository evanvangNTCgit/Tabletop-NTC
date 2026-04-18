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

        [HttpGet("MostPurchased")]
        public IActionResult MostPurchased() 
        {
            List<PurchaseStatsViewModel> saleLines = this._dataService.GetPiecePurchaseStats();

            return View(saleLines);
        }
    }
}
