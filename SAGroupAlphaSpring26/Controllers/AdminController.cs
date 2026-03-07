using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Services;
using SAGroupAlphaSpring26.ViewModels;

namespace SAGroupAlphaSpring26.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly DataService _dataService;

        public AdminController(DataContext dc) 
        {
            // This controller needs a dataservice since this is the 
            // Controller ADMIN is using to use some CRUD operations.
            this._dataService = new DataService(dc);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("AddPiece")]
        public IActionResult AddPiece() 
        {
            // This needs my piece view models so that it can see the different piece types.
            PieceViewModel pvm = new();
            pvm.PieceTypes = this._dataService.GetPieceTypes();

            return View(pvm);
        }

        [HttpPost("AddPiece")]
        public IActionResult AddPiece(PieceViewModel pvm) 
        {
            // If the model provided is not valid send them back to the view
            // HOWEVER REMEMBER it needs the view model to see the list of piece types.
            if (!ModelState.IsValid) 
            {
                PieceViewModel p = new();
                p.PieceTypes = this._dataService.GetPieceTypes();

                return View(p);
            }

            this._dataService.AddPiece(pvm.Piece!);

            // And then redirect the user back to the products
            // However not here currently so just index
            return RedirectToAction("~/Views/Home/Index.cshtml");
        }
    }
}
