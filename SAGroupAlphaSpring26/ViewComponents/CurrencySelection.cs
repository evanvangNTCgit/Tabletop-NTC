using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.ApiServices;
using System.Security.Claims;

namespace SAGroupAlphaSpring26.ViewComponents
{
    // If needed somewhere to change a user currency can simply add this view component.
    public class CurrencySelection : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var model = new CurrencyViewModel();
                model.Currencies = await CurrencyConverter.GetCurrenciesSupported();
                var userEmail = UserClaimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;

                if (userEmail == null || userEmail == string.Empty)
                    throw new Exception("No user email... Likely not logged in.");

                model.UserEmail = userEmail ?? string.Empty;

                return View(model);
            }
            catch
            {
                return View(null);
            }
        }
    }
}
