using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Models;
using SAGroupAlphaSpring26.Services;
using SAGroupAlphaSpring26.ViewModels;

namespace SAGroupAlphaSpring26.Controllers
{
    // Fow now allow anonymous change if needed or desired.
    [Authorize]//[AllowAnonymous]
    public class MapController : Controller
    {
        private readonly DataService _dataService;

        public MapController(DataService dataService)
        {
            _dataService = dataService;
        }

        // Stores positions of Tokens on the map.
        [HttpPost]
        public IActionResult SavePositions([FromBody] List<TokenUpdateModel> updates)
        {
            if (updates == null || !updates.Any()) return BadRequest("No token updates provided.");

            _dataService.UpdateTokenPositions(updates);
            return Ok();
        }

        [HttpPost("Map/SaveSessionNotes")]
        public IActionResult SaveSessionNotes([FromBody] SessionNotesUpdateModel model)
        {
            if (model == null || model.SessionId <= 0) return BadRequest("Invalid session notes data.");

            var session = _dataService.GetSession(model.SessionId);
            if (session != null)
            {
                session.Notes = model.Notes ?? "";
                session.LastUpdated = DateTime.Now;
                _dataService.UpdateSession(session);
                return Ok();
            }

            return NotFound("Session not found.");
        }

        [HttpPost]
        public IActionResult CreateToken([FromBody] TokenCreateModel model)
        {
            if (model == null || model.PieceId == 0 || model.SessionId == 0)
                return BadRequest("Invalid model.");

            var piece = _dataService.GetPiece(model.PieceId);
            if (piece == null)
                return NotFound("Piece not found.");

            var maxZ = _dataService.GetMaxZIndexForSession(model.SessionId);
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
            if (!int.TryParse(UserId, out int UserIdParse))
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
        public IActionResult Map(int id)
        {
            // gets the session from the database using the session ID.
            var session = _dataService.GetSession(id);

            if (session == null)
            {
                return NotFound();
            }

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
            if (int.TryParse(UserId, out int UserIdParse))
            {
                viewModel.PlayablePieces = this._dataService.GetUserPieces(UserIdParse);
            }

            return View(viewModel);
        }

        // Deletes a list of tokens by ID. The Javascript saves each deleted token's ID, then sends it here to the mapController, then calls the data service function to delete them from the database.
        [HttpPost("Map/DeleteTokens")]
        public IActionResult DeleteTokens([FromBody] List<int> tokenIds)
        {
            // Checks if there are any tokens scheduled to delete.
            if (tokenIds == null || !tokenIds.Any()) return Ok();

            // Deletes the tokens.
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

        // Clears all tokens from the board via dataservice, doesn't save the changes made to the board, click save button to save changes.
        [HttpPost("Map/ClearBoard/{sessionId}")]
        public IActionResult ClearBoard(int sessionId)
        {
            try
            {
                _dataService.ClearSessionTokens(sessionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class UpdateMapModel
        {
            public int SessionId { get; set; }
            public int PieceId { get; set; }
        }

        // Updates the map background by updating the map token's piece ID to the new piece ID.
        [HttpPost("Map/UpdateMapBackground")]
        public IActionResult UpdateMapBackground([FromBody] UpdateMapModel model)
        {
            // Checks if the model is valid.
            if (model == null || model.SessionId <= 0 || model.PieceId <= 0) return BadRequest("Invalid model");

            try
            {
                // Calls the dataservice function to update the map background.
                string newMapImagePath = _dataService.UpdateSessionMap(model.SessionId, model.PieceId);
                return Json(new { imagePath = newMapImagePath });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
