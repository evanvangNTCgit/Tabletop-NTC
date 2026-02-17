using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Models;
using System.Linq;

namespace SAGroupAlphaSpring26.Controllers
{

    public class MapController : Controller
    {
        private readonly DataContext _context;

        public MapController(DataContext context)
        {
            _context = context;
        }

        // Stores positions of Tokens on the map.
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
                    token.zIndex = update.zIndex;
                }
            }
            _context.SaveChanges();
            return Ok();
        }

        // Helper class for token JSON data.
        public class TokenUpdateModel
        {
            public int Id { get; set; }
            public double X { get; set; }
            public double Y { get; set; }

            public int zIndex { get; set; }

            public int SessionID { get; set; }

            public string Name { get; set; } = string.Empty;
        }

        public IActionResult Map(int sessionId)
        {
            // gets the session from the database using the session ID.
            var session = _context.Sessions.Find(sessionId);

            // Creates the map token for displaying the map on the screen.
            var mapToken = _context.Tokens
                .Include(t => t.Piece)
                .ThenInclude(p => p.PieceType)
                .FirstOrDefault(t => t.SessionID == sessionId && t.Piece.PieceType.Name == "Map");

            var viewModel = new MapScreenViewModel()
            {
                CurrentSession = session,
                MapImagePath = mapToken != null ? mapToken.Piece.ImagePath : "/images/testMap.png",

                // For now we are getting all tokens that are not maps, but later on we may want to include maps for quick switching of maps during a session.
                Tokens = _context.Tokens
                    .Include(t => t.Piece)
                    .Where(t => t.SessionID == sessionId && t.Piece.PieceType.Name != "Map")
                    .ToList()
            };

            return View("~/Views/Session/Map.cshtml", viewModel);
        }
    }
}
