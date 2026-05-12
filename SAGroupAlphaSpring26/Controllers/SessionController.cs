using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Services;
using System.Security.Claims;

namespace SAGroupAlphaSpring26.Controllers
{
    // For now allow anonymous change if needed or wanted.
    [Authorize]//[AllowAnonymous]
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly DataService _dataService;
        private readonly ILogger<SessionController> _logger;

        public SessionController(DataService dataService, ILogger<SessionController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        [Route("/{userId}")]
        public IActionResult Index(int userId)
        {
            var sessions = _dataService.GetSessions(userId);
            return View(sessions);
        }

        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            try
            {
                var session = _dataService.GetSession(id);
                if (session == null)
                {
                    return NotFound($"Could not find a session with ID {id}");
                }

                return View(session);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to show session details for session: {id}. {ex.Message}");
                return NotFound();
            }
        }

        // Takes us to the Session Creation Page.
        [Authorize]
        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                // Gets all of the pieces the user owns, where the piece type is "Map"
                var userId = _dataService.GetUserId(User);
                ViewBag.AvailableMaps = _dataService.GetUserPieces(userId)
                    .Where(p => p.PieceType?.Name == "Map")
                    .ToList();

                return View(new Session());
            }
            catch (Exception ex)
            {
                try
                {
                    var userId = _dataService.GetUserId(User);
                    User user = _dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to show create session view for user: {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to show create session for user, failed to load user info. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        // Posts the created sessions information.
        [Authorize]
        [HttpPost("create")]
        public IActionResult Create(Session session, int? initialMapId)
        {
            try
            {
                // Gets user id using the get user id data service.
                session.UserId = _dataService.GetUserId(User);

                // time of creation.
                session.LastUpdated = DateTime.Now;

                // adds the session to the database.
                _dataService.AddSession(session);

                // Set the initial map if selected
                if (initialMapId.HasValue && initialMapId.Value > 0)
                {
                    try
                    {
                        _dataService.UpdateSessionMap(session.Id, initialMapId.Value);
                    }
                    catch (Exception)
                    {
                        // Fallback to default if there is an issue with the selected map
                    }
                }

                return RedirectToAction("Details", new { id = session.Id });
            }
            catch (Exception ex)
            {
                try
                {
                    // Gets user id using the get user id data service.
                    var userId = _dataService.GetUserId(User);
                    User user = _dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to create a session for user: {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to create a session for user, failed to load user info. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("edit")]
        public IActionResult Edit(Session session)
        {
            try
            {
                var existingSession = _dataService.GetSession(session.Id);

                if (existingSession == null)
                {
                    return NotFound($"Could not find a session with ID {session.Id}");
                }

                existingSession.Name = session.Name;

                existingSession.Notes = session.Notes;

                // updates timestamp.
                existingSession.LastUpdated = DateTime.Now;

                // Updates the existing session.
                _dataService.UpdateSession(existingSession);

                // Redirect back to Details to see the saved changes
                return RedirectToAction("Details", new { id = session.Id });
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to edit session: {session.Id}, {session.Name}. {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("editNotes")]
        public IActionResult EditNotes([FromBody] NoteUpdateDTO update)
        {
            var session = _dataService.GetSession(update.Id);

            if (session == null) return NotFound();

            session.Notes = update.Notes;

            session.LastUpdated = DateTime.Now;

            _dataService.UpdateSession(session);

            return Ok();
        }

        // Confirms the deletion of the session
        [HttpPost("delete/{id}"), ActionName("Delete")]
        public IActionResult DeleteSessionConfirmed(int id)
        {
            try
            {
                var session = _dataService.GetSession(id);
                if (session != null)
                {
                    // Checks if the user owns the session, just to be safe.
                    if (session.UserId == _dataService.GetUserId(User))
                    {
                        _dataService.DeleteSession(id);
                    }
                }

                // Redirect back to user index page.
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to delete session: {id}. {ex.Message}");
                // Redirect back to user index page.
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("RecoverSession")]
        public IActionResult RecoverSession()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                if (userId == null)
                {
                    // Redirect back to sign in
                    // Maybe not signed in?
                    return RedirectToAction("Account", "Login");
                }
                List<Session> archivedSessions = this._dataService.GetDeletedSessions(int.Parse(userId));
                return View(archivedSessions);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to show Recover Session. {ex.Message}");
                // Redirect back to user index page.
                return RedirectToAction("Index", "Home");

            }
        }

        [HttpGet("RecoverSessionConfirm")]
        public IActionResult RecoverSessionConfirm(int id)
        {
            try
            {
                this._dataService.RestoreSession(id);

                // Redirect back to user index page.
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to recover session: {id}. {ex.Message}");
                // Redirect back to user index page.
                return RedirectToAction("Index", "Home");
            }
        }
    }
}