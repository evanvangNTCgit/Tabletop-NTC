using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.ApiServices;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Services;
using System.Security.Claims;

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
            List<CartItem> cartItems = new();
            List<CartItemSet> setItems = new();
            User user = null;

            // auth check.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Get and parse the User ID just like your teammate did
                string? userIdClaim = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    user = _dataService.GetUser(userId);
                    cartItems = _dataService.GetCartItemsNoTracking(userId);
                    if (cartItems != null && cartItems.Count > 0)
                    {
                        model.Pieces.AddRange(cartItems);
                    }

                    setItems = _dataService.GetCartItemsSetNoTracking(userId);
                    if (setItems != null && setItems.Count > 0)
                    {
                        model.Sets.AddRange(setItems);
                    }

                    model.Pieces.ForEach(p =>
                    {
                        if (p.Piece != null)
                        {
                            p.Piece.Price = CurrencyConverter.ConvertPriceToCurrency(user.Currency, p.Piece.Price);
                        }
                    });
                    model.Sets.ForEach(s =>
                    {
                        if (s.Set != null)
                        {
                            s.Set.Price = CurrencyConverter.ConvertPriceToCurrency(user.Currency, s.Set.Price);
                        }
                    });
                }
            }

            return View(model);
        }
    }
}