using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Models;
using SAGroupAlphaSpring26.Services;

namespace SAGroupAlphaSpring26.Controllers
{
    // For now allow anonymous change if needed or wanted.
    [Authorize]//[AllowAnonymous]
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly DataService _dataService;

        public SessionController(DataService dataService)
        {
            _dataService = dataService;
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
            catch
            {
                return NotFound();
            }
        }

        // Takes us to the Session Creation Page.
        [Authorize]
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new Session());
        }

        // Posts the created sessions information.
        [Authorize]
        [HttpPost("create")]
        public IActionResult Create(Session session)
        {
            // Gets user id using the get user id data service.
            session.UserId = _dataService.GetUserId(User);

            // time of creation.
            session.LastUpdated = DateTime.Now;

            // adds the session to the database.
            _dataService.AddSession(session);

            return RedirectToAction("Details", new { id = session.Id });
        }

        [HttpPost("edit")]
        public IActionResult Edit(Session session)
        {
            var existingSession = _dataService.GetSession(session.Id);

            if(existingSession == null)
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

        public class NoteUpdateDTO
        {
            public int Id { get; set; }
            public string Notes { get; set; } = string.Empty;
        }
    }
}