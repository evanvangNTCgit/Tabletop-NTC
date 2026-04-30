using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.Data;
using SAGroupAlphaSpring26.Services;
using System.Security.Claims;

namespace SAGroupAlphaSpring26.Controllers
{
    // [Authorize] Do we need the authorize tag, since most of the routes should be allowed anonymous? We can add Authorize for specific routes if needed right?.
    [Route("Account")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly DataService _dataService;

        public AccountController(DataService dataService)
        {
            _dataService = dataService;
        }

        // The routing user goes to when registering.
        [AllowAnonymous]
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterViewModel rvm)
        {
            // If it is not valid user should be sent back to view for corrections...
            if (!ModelState.IsValid)
            {
                return View();
            }

            // See if the user already has an account...
            User existingUser = _dataService.GetUser(rvm.EmailAddress);
            if (existingUser != null)
            {
                ModelState.AddModelError("Error", "An account already exists with that email address.");
                // Send them back for correction
                return View();
            }

            PasswordHasher<string> passwordHasher = new();

            User user = new User()
            {
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                Email = rvm.EmailAddress,

                // This stores the password as a hash 
                // Hash being one-way encryption so it cannot be decrypted to original password. (Unless they really really try)
                // When the user tries to log in, the password they enter will be hashed and compared to the stored hash. If they match, the password is correct.
                PasswordHash = passwordHasher.HashPassword(null!, rvm.Password)
            };

            _dataService.AddUser(user);

            // Redirect the user back to the homepage
            // Changed to redirect to the Login page instead, since they currently will need to login again after creating an account.
            return RedirectToAction("sign-in", "Account");
        }

        // People that have not been authenticated yet can log in.
        // (They need to login for authentication.
        [AllowAnonymous]
        [HttpGet("sign-in")]
        public IActionResult Login()
        {
            return View();
        }

        // This is an asynctask because we are calling to a DB 
        // So we dont want to hold up the main thread. Handle other requests while we verify login.
        [HttpPost("sign-in")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl)
        {
            // If not valid send user back for correction.
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Get the user based on client input...
            User user = _dataService.GetUser(loginViewModel.EmailAddress);
            if (user == null)
            {
                // Set email address not registered error message.
                ModelState.AddModelError("Error", "An account does not exist with that email address.");

                return View();
            }

            // Check if password is correct
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            PasswordVerificationResult passwordVerificationResult =
                passwordHasher.VerifyHashedPassword(null!, user.PasswordHash, loginViewModel.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                // Set invalid password error message.
                ModelState.AddModelError("Error", "Invalid password.");

                return View();
            }

            // Add the user's ID (NameIdentifier), first name and role
            // to the claims that will be put in the cookie.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Can configure certain authentication properties if desired.
            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationproperties?view=aspnetcore-10.0
            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties {
            IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(returnUrl);
            }
            // The following can be used to find this cookie: 
            // User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        }

        [HttpGet("user-pieces-inventory")]
        public IActionResult UserPieceInventory()
        {
            int userId = 0;
            try
            {
                userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
            catch
            {
                // User likely does not have a cookie, account, or is not logged in yet...
            }

            if (userId > 0)
            {
                List<Piece> model = this._dataService.GetUserPieces(userId);
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // User of course must be already authorized to log out...
// User of course must be already authorized to log out...
        [HttpGet("sign-out")]
        [Route("sign-out"), Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        // Edit Profile - requires authorization
        [HttpGet("edit")]
        [Authorize]
        public IActionResult Edit()
        {
            // Get the current user's information
            int userId = _dataService.GetUserId(User);
            var user = _dataService.GetUser(userId);

            var editViewModel = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.Email
            };

            return View(editViewModel);
        }

        // Edit Profile POST - requires authorization
        [HttpPost("edit")]
        [Authorize]
        public IActionResult Edit(EditProfileViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }

            // Get the current user
            int userId = _dataService.GetUserId(User);
            var user = _dataService.GetUser(userId);

            // Check if email is being changed to an existing email
            if (user.Email != editViewModel.EmailAddress)
            {
                var existingUser = _dataService.GetUser(editViewModel.EmailAddress);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Error", "An account already exists with that email address.");
                    return View(editViewModel);
                }
            }

            // Update user information
            user.FirstName = editViewModel.FirstName;
            user.LastName = editViewModel.LastName;
            user.Email = editViewModel.EmailAddress;

            _dataService.UpdateUser(user);

            // Update the claim for the user's name
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity != null)
            {
                var nameClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    claimsIdentity.RemoveClaim(nameClaim);
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
                }
            }

TempData["SuccessMessage"] = "Your profile has been updated successfully.";
            return RedirectToAction("Edit");
        }

        // Change Password - requires authorization
        [HttpGet("change-password")]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // Change Password POST - requires authorization
        [HttpPost("change-password")]
        [Authorize]
        public IActionResult ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Get the current user
            int userId = _dataService.GetUserId(User);
            var user = _dataService.GetUser(userId);

            // Verify current password
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            PasswordVerificationResult passwordVerificationResult =
                passwordHasher.VerifyHashedPassword(null!, user.PasswordHash, viewModel.CurrentPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View(viewModel);
            }

            // Hash and update the new password
            string newPasswordHash = passwordHasher.HashPassword(null!, viewModel.NewPassword);
            _dataService.UpdateUserPassword(user, newPasswordHash);

            TempData["SuccessMessage"] = "Your password has been changed successfully.";
            return RedirectToAction("ChangePassword");
        }

        [HttpGet("access-denied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
