using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Services;

namespace SAGroupAlphaSpring26.Controllers
{
    [Authorize(Roles = "Admin")]
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
            pvm.PieceTypes = _dataService.GetPieceTypes();

            string pathForImages = Path.Combine(this._webHostEnvironment.WebRootPath, "images/");
            pvm.ImagePaths = Directory.EnumerateFiles(pathForImages)
                .Select(fn => Path.GetFileName(fn))
                .ToList();

            return View(pvm);
        }

        [HttpPost("AddPiece")]
        public async Task<IActionResult> AddPiece(PieceViewModel pvm)
        {
            // If the model provided is not valid send them back to the view
            // HOWEVER REMEMBER it needs the view model to see the list of piece types.
            if (!ModelState.IsValid)
            {
                // PieceViewModel p = new(); Do we need to create a new pieceviewmodel here? Shouldn't we get the one from the user?
                pvm.PieceTypes = _dataService.GetPieceTypes();

                string pathForImages = Path.Combine(this._webHostEnvironment.WebRootPath, "images/");
                pvm.ImagePaths = Directory.EnumerateFiles(pathForImages)
                    .Select(fn => Path.GetFileName(fn))
                    .ToList();

                return View(pvm);
            }

            try
            {
                // https://stackoverflow.com/questions/52399086/image-file-upload-and-usage-asp-net-core-2-0

                // Okay so we check first if they did a customer image upload.
                if (pvm.UserImageUpload != null)
                {
                    // Get the file name and copy it to the wwwroot folder.
                    var fileName = Path.GetFileName(pvm.UserImageUpload.FileName);
                    var filePath = Path.Combine(this._webHostEnvironment.WebRootPath, "images/", fileName);

                    // Initializing a new file stream that directs to the wwwroot/image, then filemode.create says to essentially create a new file in it.
                    using (var filestream = new FileStream(filePath, FileMode.Create))
                    {
                        await pvm.UserImageUpload.CopyToAsync(filestream);
                    }

                    // Now set the piece image to that name...
                    pvm.Piece!.ImagePath = $"/images/{fileName}";
                }
                // Okay so they did not upload something... They chose an already provided image..
                // And if they did not select one its set to default in the model to use placeholder image.
                else
                {
                    // I do this because it just reads the file name like Cleric.png
                    // However for it to work on the JS it needs to add /images/ to the beginning of it.
                    pvm.Piece!.ImagePath = $"/images/{pvm.Piece.ImagePath}";
                }

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

        [HttpGet("AddPieceType")]
        public IActionResult AddPieceType()
        {
            return View();
        }

        [HttpPost("AddPieceType")]
        public IActionResult AddPieceType(PieceType pt)
        {
            // If what is posted does not meet validation...
            if (!ModelState.IsValid)
            {
                // return them back to the view...
                return View();
            }

            try
            {
                this._dataService.AddPieceType(pt);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to add piece type: {e.Message}");
            }
        }

        // Get for editing a piece.
        [HttpGet("EditPiece/{id:int}")]
        public IActionResult EditPiece(int id)
        {
            var existingPiece = _dataService.GetPiece(id);
            if (existingPiece == null) return NotFound();

            // Creates a view model and adds the piece so that the view can populate the form with the existing data.
            PieceViewModel pvm = new();
            pvm.Piece = existingPiece;
            pvm.PieceTypes = _dataService.GetPieceTypes();

            string pathForImages = Path.Combine(_webHostEnvironment.WebRootPath, "images/");
            pvm.ImagePaths = Directory.EnumerateFiles(pathForImages)
                .Select(fn => Path.GetFileName(fn)).ToList();

            return View(pvm);
        }

        // Post for editing a piece.
        [HttpPost("EditPiece/{id:int}")]
        public async Task<IActionResult> EditPiece(PieceViewModel pvm)
        {
            if (!ModelState.IsValid)
            {
                pvm.PieceTypes = _dataService.GetPieceTypes();
                string pathForImages = Path.Combine(this._webHostEnvironment.WebRootPath, "images/");
                pvm.ImagePaths = Directory.EnumerateFiles(pathForImages).Select(fn => Path.GetFileName(fn)).ToList();
                return View(pvm);
            }

            try
            {
                // Handle new image upload if they provided one
                if (pvm.UserImageUpload != null)
                {
                    var fileName = Path.GetFileName(pvm.UserImageUpload.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/", fileName);
                    using (var filestream = new FileStream(filePath, FileMode.Create))
                    {
                        await pvm.UserImageUpload.CopyToAsync(filestream);
                    }
                    pvm.Piece!.ImagePath = $"/images/{fileName}";
                }
                else if (!pvm.Piece!.ImagePath.StartsWith("/images/"))
                {
                    pvm.Piece.ImagePath = $"/images/{pvm.Piece.ImagePath}";
                }

                this._dataService.UpdatePiece(pvm.Piece!);
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // Creates a session for a user, this is for admin/testing purposes, users create sessions through the sessioncontroller.
        [HttpGet("AddSession")]
        public IActionResult AddSession()
        {
            return View();
        }

        // Posts the created session information.
        [HttpPost("AddSession")]
        public IActionResult AddSession(Session session)
        {
            if (ModelState.IsValid)
            {
                session.LastUpdated = DateTime.Now;
                this._dataService.AddSession(session);
                return RedirectToAction("Index", "Home");
            }
            return View(session);
        }
    }
}
