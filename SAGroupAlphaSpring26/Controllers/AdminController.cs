using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Services;

namespace SAGroupAlphaSpring26.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly DataService _dataService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(DataContext dc, IWebHostEnvironment webHostEnvironment)
        {
            // This controller needs a dataservice since this is the 
            // Controller ADMIN is using to use some CRUD operations.
            this._dataService = new DataService(dc);

            this._webHostEnvironment = webHostEnvironment;
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

            string pathForImages = Path.Combine(this._webHostEnvironment.WebRootPath, "images/");
            pvm.ImagePaths = Directory.EnumerateFiles(pathForImages)
                .Select(fn => Path.GetFileName(fn))
                .ToList();

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

                string pathForImages = Path.Combine(this._webHostEnvironment.WebRootPath, "images/");
                pvm.ImagePaths = Directory.EnumerateFiles(pathForImages)
                    .Select(fn => Path.GetFileName(fn))
                    .ToList();

                return View(p);
            }

            try
            {
                // I do this because it just reads the file name like Cleric.png
                // However for it to work on the JS it needs to add /images/ to the beginning of it.
                pvm.Piece!.ImagePath = $"/images/{pvm.Piece.ImagePath}";

                this._dataService.AddPiece(pvm.Piece!);
            }
            catch 
            {
                // Dont add it and simply redirect.
                // Goes to index action in home controller.
                return RedirectToAction("Index", "Home");
            }

            // And then redirect the user back to the products
            // However not here currently so just index
            return RedirectToAction("Index", "Home");
        }
    }
}
