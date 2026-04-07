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
            var model = new CartViewModel();
            List<CartItem> cartItems = new List<CartItem>();
            List<CartItemSet> setItems = new();

            // auth check.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Get and parse the User ID just like your teammate did
                string? userIdClaim = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {

                    cartItems = _dataService.GetCartItems(userId);
                    if (cartItems != null && cartItems.Count > 0)
                    {
                        model.Pieces.AddRange(cartItems);
                    }

                    setItems = _dataService.GetCartItemSet(userId);
                    if (setItems != null && setItems.Count > 0)
                    {
                        model.Sets.AddRange(setItems);
                    }
                }
            }

            return View(model);
        }
    }
}