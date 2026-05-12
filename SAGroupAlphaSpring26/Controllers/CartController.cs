using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.ApiServices;
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
        private readonly ILogger<CartController> _logger;

        public CartController(DataContext dataContext, ILogger<CartController> logger)
        {
            _dataService = new DataService(dataContext);
            _logger = logger;
        }

        [HttpGet("view")]
        public IActionResult ViewCart()
        {
            try
            {
                // Get the user ID (currently a string), parse it. 
                // Then get the cart items based on user ID.
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                User user = this._dataService.GetUser(userId);
                List<CartItem> userCartItems = _dataService.GetCartItemsNoTracking(userId);
                List<CartItemSet> userSets = _dataService.GetCartItemsSetNoTracking(userId);
                userCartItems.ForEach(ci =>
                {
                    if (ci.Piece != null)
                    {
                        ci.Piece.Price = CurrencyConverter.ConvertPriceToCurrency(user.Currency, ci.Piece.Price);
                    }
                });
                userSets.ForEach(cis =>
                {
                    if (cis.Set != null)
                    {
                        cis.Set.Price = CurrencyConverter.ConvertPriceToCurrency(user.Currency, cis.Set.Price);
                    }
                });

                var model = new CartViewModel();
                model.UserCurrency = user.Currency;
                model.Pieces.AddRange(userCartItems);
                model.Sets.AddRange(userSets);

                // Show view.
                return View(model);
            }
            catch (Exception ex)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    User user = this._dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to show cart for user: {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to show cart for user, could not retrieve user info. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("add-to-cart/{id:int}")]
        public IActionResult AddToCart(int id)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                _dataService.AddCartItem(userId, id);

                return RedirectToAction(nameof(ViewCart));
            }
            catch (Exception ex)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    User user = this._dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to add an item to cart for user: {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to add an item to cart for user, failed to load user information. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("add-set-to-cart/{id:int}")]
        public IActionResult AddSetToCart(int id)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                _dataService.AddSetCartItem(userId, id);

                return RedirectToAction(nameof(ViewCart));
            }
            catch (Exception ex)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    User user = this._dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to add set to cart for user: {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to add set to cart for user, failed to load user information. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("remove-from-cart/{id:int}")]
        public IActionResult RemoveFromCart(int id)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                _dataService.DeleteCartItem(userId, id);

                return RedirectToAction(nameof(ViewCart));
            }
            catch (Exception ex)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    User user = this._dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to remove from cart for user: {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to remove from cart for user, failed to load user info. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("remove-set-from-cart/{id:int}")]
        public IActionResult RemoveSetFromCart(int id)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                _dataService.DeleteSetCartItem(userId, id);

                return RedirectToAction(nameof(ViewCart));
            }
            catch (Exception ex)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    User user = this._dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to remove set from cart for user: {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to remove set from cart for user, failed to load user info. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("view-Sale")]
        public IActionResult ViewSale()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                User user = _dataService.GetUser(userId);
                Sale userSale = this.BuildSale();
                List<CartItemSet> userSets = _dataService.GetCartItemsSetNoTracking(userId);
                userSale.SaleLines.ForEach(sl => sl.Price = CurrencyConverter.ConvertPriceToCurrency(user.Currency, sl.Price));
                userSets.ForEach(s =>
                {
                    if (s.Set != null)
                    {
                        s.Set.Price = CurrencyConverter.ConvertPriceToCurrency(user.Currency, s.Set.Price);
                    }
                });

                // Dont want to keep making DB reads, I will pass in the list of sets right in the model.
                SaleViewModel model = new()
                {
                    Sale = userSale,
                    Sets = userSets
                };

                return View(model);
            }
            catch (Exception ex)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    User user = _dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to load view cart information for user {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to load view cart information for user, failed to load user info. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                Sale userSale = this.BuildSale();
                userSale.Date = DateTime.Now;
                _dataService.AddSale(userSale);
                _dataService.CheckoutCart(userId);

                // Redirect back to the cart or profile, could also add TempData message here.
                return RedirectToAction(nameof(ViewCart));
            }
            catch (Exception ex)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    User user = this._dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to check out cart for user: {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to check out cart for user, failed to load user info. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("salehistory")]
        public async Task<IActionResult> SaleHistory()
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                User user = this._dataService.GetUser(userId);

                List<Sale> UsersSales = this._dataService.GetUserSales(userId);
                UsersSales.ForEach(s => s.SaleLines.ForEach(async sl => sl.Price = await CurrencyConverter.GetHistoricalValueToUsd(user.Currency, sl.Price, s.Date)));
                UsersSales.ForEach(s => s.SaleLines.ForEach(async sl => sl.Tax = await CurrencyConverter.GetHistoricalValueToUsd(user.Currency, sl.Tax, s.Date)));
                UsersSales.ForEach(s => s.SaleLines.ForEach(async sl => sl.TotalCost = await CurrencyConverter.GetHistoricalValueToUsd(user.Currency, sl.TotalCost, s.Date)));

                return View(UsersSales);
            }
            catch (Exception ex)
            {
                try
                {
                    int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    User user = this._dataService.GetUser(userId);
                    _logger.LogInformation($"Failed to show sale history for user: {user.Email}. {ex.Message}");
                }
                catch
                {
                    _logger.LogInformation($"Failed to show sale history for user, failed to load user info. {ex.Message}");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        private Sale BuildSale()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            List<CartItem> userCartItems = _dataService.GetCartItemsNoTracking(userId);
            List<CartItemSet> userSets = _dataService.GetCartItemsSetNoTracking(userId);

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
                foreach (CartItemSet cartSet in userSets)
                {
                    if (cartSet.Set.PiecesList != null && cartSet.Set.PiecesList.Count > 0)
                    {
                        foreach (var piece in cartSet.Set.PiecesList)
                        {
                            SaleLine saleLine = new();

                            saleLine.PieceID = piece.PieceId;
                            Piece piece1 = this._dataService.GetPiece(piece.PieceId);

                            saleLine.Piece = piece1;
                            // saleLine.Price = piece1.Price;
                            saleLine.Price = piece.Set.Price / (decimal)piece.Set.PiecesList.Count();
                            saleLine.Tax = saleLine.Price * 0.05m;
                            saleLine.TotalCost = saleLine.Price + saleLine.Tax;
                            saleLine.SetID = cartSet.SetId;

                            userSale.SaleLines.Add(saleLine);
                        }
                    }
                }
            }

            return userSale;
        }
    }
}
