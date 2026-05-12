using System.Text.Json;

namespace SAGroupAlphaSpring26.ApiServices
{
    public static class GetCurrencies
    {
        private static HttpClient _client = new();
        private const string currenciesUrl = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies.json";

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
    }
}
