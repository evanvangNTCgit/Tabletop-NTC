using Microsoft.AspNetCore.Mvc;
using SAGroupAlphaSpring26.ApiServices;

namespace SAGroupAlphaSpring26.ViewComponents
{
    public class CurrencySelection : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new CurrencyViewModel();
            model.Currencies = await CurrencyConverter.GetCurrenciesSupported();
            model.CurrentChoice = Request.Cookies["UserCurrencyValue"] ?? "usd";

            return View(model);
        }
    }
}
