using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Models;
using SAGroupAlphaSpring26.Services;
using System.Security.Claims;
using SAGroupAlphaSpring26.Data;

namespace SAGroupAlphaSpring26.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly DataService _dataService;

        public CartViewComponent(DataContext dataContext)
        {
            _dataService = new DataService(dataContext);
        }

        public IViewComponentResult Invoke()
        {
            List<CartItem> cartItems = new List<CartItem>();

            // auth check.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Get and parse the User ID just like your teammate did
                string? userIdClaim = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    cartItems = _dataService.GetCartItems(userId);
                }
            }

            return View(cartItems);
        }
    }
}