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
                if (int.TryParse(update.Id, out int realTokenId))
                {
                    var token = _context.Tokens.Find(realTokenId);
                    if (token != null)
                    {
                        token.X = update.X;
                        token.Y = update.Y;
                        token.ZIndex = update.zIndex;
                        token.Visibility = update.Visibility;
                        token.Name = update.Name;
                        token.Notes = update.Notes;

                        var session = _dataService.GetSession(token.SessionId);
                        if (session != null) session.LastUpdated = DateTime.Now;
                    }
                }
                else if(update.Id != null && update.Id.StartsWith("temp-"))
                {
                    var piece = _dataService.GetPiece(update.PieceId);
                    if (piece != null)
                    {
                        _context.Tokens.Add(new Token
                        {
                            SessionId = update.SessionID,
                            PieceID = update.PieceId,
                            Name = piece.Name,
                            X = update.X,
                            Y = update.Y,
                            ZIndex = update.zIndex,
                            Visibility = update.Visibility,
                            Notes = update.Notes
                        });

                        var session = _dataService.GetSession(update.SessionID);
                        if (session != null) session.LastUpdated = DateTime.Now;
                    }
                }
            }
            _context.SaveChanges();
            return Ok();
        }

        //[HttpGet("Map/GetTokens/{sessionId}")]
        //public IActionResult GetTokens(int sessionId)
        //{
        //    var tokens = _context.Tokens
        //        .Where(t => t.SessionId == sessionId)
        //        .Select(t => new TokenUpdateModel
        //        {
        //            Id = t.Id,
        //            X = t.X,
        //            Y = t.Y,
        //            zIndex = t.ZIndex,
        //            SessionID = t.SessionId,
        //            Name = t.Name
        //        })
        //        .ToList();
        //    return Json(tokens);
        //}

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
            this._dataService.AddToken(token);

            return Json(new { id = token.Id, pieceImageID = token.PieceID });
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
                MapImagePath = mapToken != null ? mapToken.Piece!.ImagePath : "/images/BetaMap1.png",

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

            return View("~/Views/Map/VangMap.cshtml", viewModel);
        }

        [Route("Map/PlayerView/{sessionID}")]
        public IActionResult PlayerView(int sessionID)
        {
            var session = _dataService.GetSession(sessionID);

            if (session == null)
            {
                return NotFound("Session not found.");
            }

            // Use string comparison with OrdinalIgnoreCase to avoid casing bugs (e.g., "map" vs "Map")
            var mapToken = session.Tokens
                .FirstOrDefault(t => string.Equals(t.Piece?.PieceType?.Name, "Map", StringComparison.OrdinalIgnoreCase));

            var viewModel = new MapScreenViewModel()
            {
                CurrentSession = session,
                // Fallback to test map if no map token is found
                MapImagePath = mapToken?.Piece?.ImagePath ?? "/images/BetaMap1.png",

                // Filter out the map itself from the interactive tokens list
                Tokens = session.Tokens
                    .Where(t => !string.Equals(t.Piece?.PieceType?.Name, "Map", StringComparison.OrdinalIgnoreCase))
                    .ToList()
            };

            // Simplify User ID retrieval
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdString, out int userId))
            {
                viewModel.PlayablePieces = _dataService.GetUserPieces(userId);
            }
            else
            {
                // Handle guest or unauthenticated user - perhaps an empty list?
                viewModel.PlayablePieces = new List<Piece>();
            }

            return View(viewModel);
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

        // Deletes a list of tokens by ID. The Javascript saves each deleted token's ID, then sends it here to the mapController, then calls the data service function to delete them from the database.
        [HttpPost("Map/DeleteTokens")]
        public IActionResult DeleteTokens([FromBody] List<int> tokenIds)
        {
            if (tokenIds == null || !tokenIds.Any()) return Ok(); // if nothing to delete: return ok.

            // deletes the tokens
            try
            {
                this._dataService.DeleteTokens(tokenIds);
                return Ok();
            }
            catch
            {
                return BadRequest("Failed to delete tokens.");
            }
        }
    }
}
