using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Models;
using SAGroupAlphaSpring26.Services;
using SAGroupAlphaSpring26.ViewModels;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;

namespace SAGroupAlphaSpring26.Controllers
{
    // Fow now allow anonymous change if needed or desired.
    [Authorize]//[AllowAnonymous]
    public class MapController : Controller
    {
        private readonly DataService _dataService;
        private readonly ILogger<MapController> _logger;

        public MapController(DataService dataService, ILogger<MapController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult SavePositions([FromBody] List<TokenUpdateModel> updates)
        {
            if (updates == null) return BadRequest("No token updates provided.");
            if (!updates.Any()) return Ok();

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
        [Route("Map/MapTest/{id}/{sceneId?}")]
        public IActionResult MapTest(int id, int? sceneId)
        {
            try
            {
                // now uses data service to get session data, this should get the tokens/pieces associated.
                var session = _dataService.GetSession(id);

                if (session == null)
                {
                    return NotFound();
                }

                // Ensure at least one scene exists
                if (!session.Scenes.Any())
                {
                    _dataService.AddScene(new Scene { SessionId = id, Name = "Default Scene" });
                    session = _dataService.GetSession(id);
                }

                var currentScene = sceneId.HasValue
                    ? session.Scenes.FirstOrDefault(s => s.Id == sceneId.Value)
                    : session.Scenes.FirstOrDefault();

                if (currentScene == null) currentScene = session.Scenes.First();

                // Creates the map token for displaying the map on the screen.
                // The map token should always be at ZIndex 0, and only one token should be the map per scene.
                var mapToken = session.Tokens
                    .FirstOrDefault(t => string.Equals(t.Piece?.PieceType?.Name, "Map", StringComparison.OrdinalIgnoreCase) && t.SceneId == currentScene.Id);

                var viewModel = new MapScreenViewModel()
                {
                    CurrentSession = session,
                    CurrentScene = currentScene,
                    Scenes = session.Scenes,
                    MapImagePath = mapToken != null ? mapToken.Piece!.ImagePath : "/images/BetaMap1.png",

                    Tokens = session.Tokens
                    .Where(t => t.Piece?.PieceType?.Name != "Map" && t.SceneId == currentScene.Id)
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
            catch (Exception ex)
            {
                try
                {
                    var session = _dataService.GetSession(id);
                    _logger.LogInformation($"Failed to load map for session {session.Id}, {session.Name}, {ex.Message}");
                } catch
                {
                    _logger.LogInformation($"Failed to load map for session, failed to load session data, {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        // Displays the map for the players, uses session ID and Scene ID to display the correct map.
        [Route("Map/PlayerView/{id}/{sceneId?}")]
        public IActionResult PlayerView(int id, int? sceneId)
        {
            try
            {
                var session = _dataService.GetSession(id);

                if (session == null)
                {
                    return NotFound("Session not found.");
                }

                var currentScene = sceneId.HasValue
                    ? session.Scenes.FirstOrDefault(s => s.Id == sceneId.Value)
                    : session.Scenes.FirstOrDefault();

                if (currentScene == null)
                {
                    if (!session.Scenes.Any()) return NotFound("No scenes found for this session.");
                    currentScene = session.Scenes.First();
                }

                // Compares strings and uses StringComparison.OrdinalIgnoreCase to ensure correct casing.
                var mapToken = session.Tokens
                    .FirstOrDefault(t => string.Equals(t.Piece?.PieceType?.Name, "Map", StringComparison.OrdinalIgnoreCase) && t.SceneId == currentScene.Id);

                var viewModel = new MapScreenViewModel()
                {
                    CurrentSession = session,
                    CurrentScene = currentScene,
                    Scenes = session.Scenes,
                    // Fallback to test map if no map token is found
                    MapImagePath = mapToken?.Piece?.ImagePath ?? "/images/BetaMap1.png",

                    // Filter out the map itself from the interactive tokens list
                    Tokens = session.Tokens
                        .Where(t => !string.Equals(t.Piece?.PieceType?.Name, "Map", StringComparison.OrdinalIgnoreCase) && t.SceneId == currentScene.Id)
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
            catch (Exception ex)
            {
                try
                {
                    var session = _dataService.GetSession(id);
                    _logger.LogInformation($"Failed to load player view for session: {session.Id}, {session.Name}. {ex.Message}");
                } catch
                {
                    _logger.LogInformation($"Failed to load player view for session, failed to load session info. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
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

            // StringComparison.OrdinalIgnoreCase used to ensure correct casing.
            var mapToken = session.Tokens
                .FirstOrDefault(t => string.Equals(t.Piece?.PieceType?.Name, "Map", StringComparison.OrdinalIgnoreCase));

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
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to delete tokens for a session. {ex.Message}");
                return BadRequest("Failed to delete tokens.");
            }
        }

        // Clears all tokens from the board via dataservice, doesn't save the changes made to the board, click save button to save changes. Uses SessionId and SceneId to delete only the tokens in that scene.
        [HttpPost("Map/ClearBoard/{sessionId}/{sceneId}")]
        public IActionResult ClearBoard(int sessionId, int sceneId)
        {
            try
            {
                _dataService.ClearSessionTokens(sessionId, sceneId);
                return Ok();
            }
            catch (Exception ex)
            {
                this._logger.LogInformation($"Failed to clear board for session: {sessionId}. {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        public class UpdateMapModel
        {
            public int SessionId { get; set; }
            public int PieceId { get; set; }
            public int SceneId { get; set; }
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
                string newMapImagePath = _dataService.UpdateSessionMap(model.SessionId, model.PieceId, model.SceneId);
                return Json(new { imagePath = newMapImagePath });
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to update the map background for session: {model.SessionId}. {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // Creates a new scene for the session.
        [HttpPost("Map/CreateScene")]
        public IActionResult CreateScene([FromBody] SceneModel model)
        {
            try
            {
                if (model == null || model.SessionId <= 0 || string.IsNullOrEmpty(model.Name))
                    return BadRequest("Invalid scene data.");

                var scene = new Scene
                {
                    SessionId = model.SessionId,
                    Name = model.Name
                };
                _dataService.AddScene(scene);

                // If a map was selected, set the selected map.
                if (model.MapPieceId.HasValue && model.MapPieceId.Value > 0)
                {
                    _dataService.UpdateSessionMap(model.SessionId, model.MapPieceId.Value, scene.Id);
                }

                // If tokens were selected to be carried over, clone them to the new scene.
                if (model.TokenIdsToClone != null && model.TokenIdsToClone.Any())
                {
                    _dataService.CloneTokensToScene(model.TokenIdsToClone, scene.Id);
                }

                return Json(new { id = scene.Id, name = scene.Name });
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to Create a new scene for session: {model.SessionId}. {ex.Message}");
                return BadRequest("Failed Scene Creation");
            }
        }

        // Deletes a scene from the session by ID. Automatically deletes all tokens associated with that scene.
        [HttpPost("Map/DeleteScene/{id}")]
        public IActionResult DeleteScene(int id)
        {
            try
            {
                _dataService.DeleteScene(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to delete scene: {id}. {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

    }
}
