using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Services;
using System.Security.Claims;


namespace SAGroupAlphaSpring26.Controllers
{
    // Fow now allow anonymous change if needed or desired.
    [Authorize]//[AllowAnonymous]
    public class MapController : Controller
    {
        private readonly DataService _dataService;
        private readonly DataContext _context;

        public MapController(DataService dataService, DataContext context)
        {
            _dataService = dataService;
            _context = context;
        }

        // Stores positions of Tokens on the map.
        [HttpPost]
        public IActionResult SavePositions([FromBody] List<TokenUpdateModel> updates)
        {
            if (updates == null || !updates.Any()) return BadRequest("No token updates provided.");

            foreach (var update in updates)
            {
                var token = _context.Tokens.Find(update.Id);
                if (token != null)
                {
                    token.X = update.X;
                    token.Y = update.Y;
                    token.ZIndex = update.zIndex;
                    
                    var session = _dataService.GetSession(token.SessionId);
                    session.LastUpdated = DateTime.Now;
                }
            }
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateToken([FromBody] TokenCreateModel model)
        {
            if (model == null || model.PieceId == 0 || model.SessionId == 0)
                return BadRequest("Invalid model.");

            var piece = _context.Pieces.Find(model.PieceId);
            if (piece == null)
                return NotFound("Piece not found.");

            var maxZ = _context.Tokens
                .Where(t => t.SessionId == model.SessionId)
                .Max(t => (int?)t.ZIndex) ?? 0;
            var token = new Token
            {
                SessionId = model.SessionId,
                PieceID = model.PieceId,
                Name = piece.Name,
                X = model.X,
                Y = model.Y,
                ZIndex = maxZ + 1
            };
            _context.Tokens.Add(token);
            _context.SaveChanges();

            return Json(new { id = token.Id });
        }

        // Added id to routing.
        [Route("Map/MapTest/{id}")]
        public IActionResult MapTest(int id) 
        {
            // now uses data service to get session data, this should get the tokens/pieces associated.
            var session = _dataService.GetSession(id);

            if (session == null)
            {
                return NotFound();
            }

            // Creates the map token for displaying the map on the screen.
            // The nullability of this is already handles by just setting it to a test map...
            var mapToken = session.Tokens
                .FirstOrDefault(t => t.Piece?.PieceType?.Name == "Map");

            var viewModel = new MapScreenViewModel()
            {
                CurrentSession = session,
                MapImagePath = mapToken != null ? mapToken.Piece!.ImagePath : "/images/testMap.png",

                Tokens = session.Tokens
                .Where(t => t.Piece?.PieceType?.Name != "Map")
                .ToList()
            };

            var UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!int.TryParse(UserId, out int UserIdParse)) 
            {
                // implement some sort of error handling...
            }

            viewModel.PlayablePieces = this._dataService.GetUserPieces(UserIdParse);

            return View("~/Views/Map/Map.cshtml", viewModel);
        }

        public IActionResult PlayerView() 
        {
            // return View(this._mapscreenViewModel);
            // Not implemented yet!!
            return View();
        }


        // Added id to routing.
        [Route("Map/Map/{id}")]
        public IActionResult Map(int sessionId)
        {
            // gets the session from the database using the session ID.
            var session = _context.Sessions.FirstOrDefault(s => s.Id == sessionId);

            // Creates the map token for displaying the map on the screen.
            // The nullability of this is already handles by just setting it to a test map...
            var mapToken = _context.Tokens
                .Include(t => t.Piece)
                .ThenInclude(p => p!.PieceType)
                .FirstOrDefault(t => t.SessionId == sessionId && t.Piece!.PieceType!.Name == "Map");

            var viewModel = new MapScreenViewModel()
            {
                CurrentSession = session,
                MapImagePath = mapToken != null ? mapToken.Piece!.ImagePath : "/images/testMap.png",

                Tokens = _context.Tokens
                    .Include(t => t.Piece)
                    .Where(t => t.SessionId == sessionId)
                    .ToList()
            };

            var UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(int.TryParse(UserId, out int UserIdParse)) 
            {
                viewModel.PlayablePieces = this._dataService.GetUserPieces(UserIdParse);
            }

            return View(viewModel);
        }
    }
}
