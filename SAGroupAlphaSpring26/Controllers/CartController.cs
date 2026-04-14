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

        [HttpGet("view-Sale")]
        public IActionResult ViewSale()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Sale userSale = this.BuildSale();
            List<CartItemSet> userSets = _dataService.GetCartItemSet(userId);

            // Dont want to keep making DB reads, I will pass in the list of sets right in the model.
            SaleViewModel model = new()
            {
                Sale = userSale,
                Sets = userSets
            };

            return View(model);
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            Sale userSale = this.BuildSale();
            _dataService.AddSale(userSale);
            _dataService.CheckoutCart(userId);

            // Redirect back to the cart or profile, could also add TempData message here.
            return RedirectToAction(nameof(ViewCart));
        }

        private Sale BuildSale() 
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            List<CartItem> userCartItems = _dataService.GetCartItems(userId);
            List<CartItemSet> userSets = _dataService.GetCartItemSet(userId);

            Sale userSale = new();
            userSale.UserID = userId;

            if (userCartItems != null && userCartItems.Count > 0)
            {
                foreach (CartItem cartItem in userCartItems)
                {
                    SaleLine saleLine = new();
                    saleLine.PieceID = cartItem.PieceId;
                    saleLine.Piece = cartItem.Piece;
                    saleLine.Price = cartItem.Piece.Price;
                    saleLine.Tax = cartItem.Piece.Price * 0.05m;
                    saleLine.TotalCost = saleLine.Price + saleLine.TotalTax;

                    userSale.SaleLines.Add(saleLine);
                }
            }

            if (userSets != null && userSets.Count > 0) 
            {
                // We agreed on making a set just a normal salline.
                // So just loop through all pieces in a set and make a saleline.
                foreach(CartItemSet cartSet in userSets) 
                {
                    foreach(var piece in cartSet.Set.PiecesList) 
                    {
                        SaleLine saleLine = new();

                        saleLine.PieceID = piece.PieceId;
                        Piece piece1 = this._dataService.GetPiece(piece.PieceId);

                        saleLine.Piece = piece1;
                        saleLine.Price = piece1.Price;
                        saleLine.Tax = piece1.Price * 0.05m;
                        saleLine.TotalCost = saleLine.Price + saleLine.TotalTax;
                        saleLine.SetID = cartSet.SetId;

                        userSale.SaleLines.Add(saleLine);
                    }
                }
            }

            return userSale;
        }
    }
}
