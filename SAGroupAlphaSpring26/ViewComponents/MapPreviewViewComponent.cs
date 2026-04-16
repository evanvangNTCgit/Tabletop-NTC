using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAGroupAlphaSpring26.Data;

namespace SAGroupAlphaSpring26.ViewComponents
{
    public class MapPreviewViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public MapPreviewViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int sessionId)
        {
            // Get all tokens for this session, includes piece type and image path.
            var tokens = await _context.Tokens
                .Include(t => t.Piece)
                .ThenInclude(p => p.PieceType)
                .Where(t => t.SessionId == sessionId)
                .ToListAsync();

            return View(tokens);
        }
    }
}
