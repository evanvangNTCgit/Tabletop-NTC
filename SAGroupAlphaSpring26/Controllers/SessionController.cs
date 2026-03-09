using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Models;
using SAGroupAlphaSpring26.Services;

namespace SAGroupAlphaSpring26.Controllers
{
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly DataService _dataService;

        public SessionController(DataService dataService)
        {
            _dataService = dataService;
        }

        [Route("index/{userId}")]
        public IActionResult Index(int userId)
        {
            var sessions = _dataService.GetSessions(userId);
            return View(sessions);
        }

        // [Route("details/{id?}")]
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
    }
}