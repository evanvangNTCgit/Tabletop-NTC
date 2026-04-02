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
            List<CartItem> model = _dataService.GetCartItems(userId);

            var test = _dataService.GetAllPieces();

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

        [HttpGet("remove-from-cart/{id:int}")]
        public IActionResult RemoveFromCart(int id)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            _dataService.DeleteCartItem(userId, id);

            return RedirectToAction(nameof(ViewCart));
        }
    }
}
