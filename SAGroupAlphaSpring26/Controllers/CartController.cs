using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Services;
using System.Security.Claims;

namespace SAGroupAlphaSpring26.Controllers
{
    [Authorize]
    [Route("Cart")]
    public class CartController : Controller
    {
        private readonly DataService _dataService;

        public CartController(DataContext dataContext)
        {
            _dataService = new DataService(dataContext);
        }

        [HttpGet("view")]
        public IActionResult ViewCart() 
        {
            // Get the user ID (currently a string), parse it. 
            // Then get the cart items based on user ID.
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            List<CartItem> userCartItems = _dataService.GetCartItems(userId);
            List<CartItemSet> userSets = _dataService.GetCartItemSet(userId);

            var model = new CartViewModel();

            model.Pieces.AddRange(userCartItems);
            model.Sets.AddRange(userSets);

            // Show view.
            return View(model);
        }

        [HttpPost("add-to-cart/{id:int}")]
        public IActionResult AddToCart(int id)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            _dataService.AddCartItem(userId, id);

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpPost("add-set-to-cart/{id:int}")]
        public IActionResult AddSetToCart(int id)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            _dataService.AddSetCartItem(userId, id);

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpGet("remove-from-cart/{id:int}")]
        public IActionResult RemoveFromCart(int id)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            _dataService.DeleteCartItem(userId, id);

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpGet("remove-set-from-cart/{id:int}")]
        public IActionResult RemoveSetFromCart(int id)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            _dataService.DeleteSetCartItem(userId, id);

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            _dataService.CheckoutCart(userId);

            // Redirect back to the cart or profile, could also add TempData message here.
            return RedirectToAction(nameof(ViewCart));
        }
    }
}
