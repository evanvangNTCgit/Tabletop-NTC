using Azure;
using System.Text.Json;

namespace SAGroupAlphaSpring26.ApiServices
{
    public static class CurrencyConverter
    {
        private static HttpClient _client = new();
        private const string currenciesUrl = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies.json";

        private const string UsdValueUrl = "https://latest.currency-api.pages.dev/v1/currencies/usd.json";

        // Fallback URL provided by API.
        private const string UsdValueURLFallback = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/usd.json";

        private static UsdCurrencyValueDTO UsdValues = null;

        /// <summary>
        /// Gets currencies that can be converted from USD.
        /// By conversion this does not mean you can put in 20 USD then get the 20 USD equivalent in Mexican Pesos
        /// Instead you get what 1 USD equals in Mexican Pesos, then we do the conversion.
        /// </summary>
        /// <returns>Dictionary of Currency Abbreviation, and currency unabbreviated. EX: eur, Euro</returns>
        public async static Task<Dictionary<string, string>> GetCurrenciesSupported()
        {
            HttpResponseMessage response = await _client.GetAsync(currenciesUrl);
            if (!response.IsSuccessStatusCode)
            {
                response = await _client.GetAsync(currenciesUrl);
            }
            var jsonReponse = await response.Content.ReadAsStringAsync();
            var currencies = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonReponse);
            if (currencies != null)
            {
                // Okay some of these currencies are weird lets really only use ones we are familiar with
                // about 300 currencies supported from API.
                // Add to this array if there is a currency you see supported we should add.
                currencies = currencies.Where(
                    c => c.Key == "usd" || // US dollars
                    c.Key == "aud" || // Australian Dollars
                    c.Key == "mxn" || // Mexican Pesos
                    c.Key == "eur" || // Euros
                    c.Key == "cad" // Candadian Dollars
                ).ToDictionary<string, string>();

                return currencies;
            }
            else
            {
                throw new CurrencyCallException("Failed to get currencies supported");
            }
        }

        /// <summary>
        /// Turns the value from USD currency to user currency.
        /// </summary>
        /// <param name="currency">ABBREVIATED CURRENCY (aud for example).</param>
        /// <param name="UsdPrice">Price in USD.</param>
        /// <returns>Price converted from Usd to currency.</returns>
        public  static decimal GetValueFrom(string currency, decimal UsdPrice)
        {
            currency = currency.ToLower(); // Incase someone capitalizes it.
            if (UsdValues != null)
            {
                // Check each currency conversion and see if it can find the currency passed in by the paramter.
                // If it cannot be found throw an exception.
                foreach (var curr in UsdValues.Values)
                {
                    if (curr.Key == currency)
                    {
                        // When rounding currency its not always up, its standard .5 and above is rounded up. So like normal math.
                        var convertedValue = Math.Round((UsdPrice * curr.Value), 2);
                        return convertedValue;
                    }
                }
                // Okay nothing found in foreach loop... throw an exception.
                throw new Exception($"Following currency not supported:{currency}");
            }
            else
            {
                throw new CurrencyCallException("Currency conversion failed");
            }
        }

        public async static void GetUsdValues()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(UsdValueUrl);
                if (!response.IsSuccessStatusCode)
                {
                    response = await _client.GetAsync(UsdValueURLFallback);
                }
                var jsonReponse = await response.Content.ReadAsStringAsync();
                var currencies = JsonSerializer.Deserialize<UsdCurrencyValueDTO>(jsonReponse);
                UsdValues = currencies;
            }
            catch
            {
                // Cant make fetch must. make sure to do null checks in other methods.
            }
        }

        /// <summary>
        /// Takes a list of store items that implemented the interface, IPricedItem.
        /// </summary>
        /// <typeparam name="T">Class that implements IpricedItem</typeparam>
        /// <param name="list">List of items implementing IPricedItem</param>
        /// <param name="currency">The currency of user 'aud' for example (australian dollars)</param>
        /// <returns>List of the store items converted to user price.</returns>
        public static List<T> GetStoreItemsPriceConverted<T>(List<T> list, string currency) where T : IPricedItem
        {
            foreach (T pricedItem in list)
            {
                pricedItem.Price = GetValueFrom(currency, pricedItem.Price);
            }

            return list;
        }
    }
}
