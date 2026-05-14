using AspNetCoreGeneratedDocument;
using Azure;
using Serilog;
using System.Text.Json;

namespace SAGroupAlphaSpring26.ApiServices
{
    public static class CurrencyConverter
    {
        private static HttpClient _client = new();
        private const string UsdValueUrl = "https://latest.currency-api.pages.dev/v1/currencies/usd.json";

        // Fallback URL provided by API.
        private const string UsdValueURLFallback = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/usd.json";

        private static UsdCurrencyValueDTO UsdValues = null!;

        /// <summary>
        /// Gets historical value of item purchase at price back to usd.
        /// </summary>
        /// <param name="currency">Currency of purchase.</param>
        /// <param name="price">Price of the item purchased in other currency.</param>
        /// <param name="date">Date of purchase.</param>
        /// <returns>Old currency purchase in USD value.</returns>
        public async static Task<decimal> GetHistoricalValueToUsd(string currency, decimal price, DateTime date)
        {
            try
            {
                currency = currency.ToLower();
                if (currency == "usd")
                    return price;
                string formattedDate = date.ToString("yyyy-MM-dd"); // This is how the API likes the date.
                string MainURL = $"https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@{formattedDate}/v1/currencies/usd.json";
                string fallbackURL = $"https://{formattedDate}.currency-api.pages.dev/v1/currencies/usd.json";
                HttpResponseMessage response = await _client.GetAsync(MainURL);
                if (response == null)
                {
                    response = await _client.GetAsync(fallbackURL);
                    response.EnsureSuccessStatusCode();
                }
                var jsonReponse = await response.Content.ReadAsStringAsync();
                var currencies = JsonSerializer.Deserialize<UsdCurrencyValueDTO>(jsonReponse);
                foreach (var curr in currencies.Values)
                {
                    if (curr.Key == currency)
                    {
                        // When rounding currency its not always up, its standard .5 and above is rounded up. So like normal math.
                        var convertedValue = Math.Round((price * curr.Value), 2);
                        return convertedValue;
                    }
                }
                // Still nothing? Currency likely not supported...
                throw new Exception($"{currency} is not supported");
            }
            catch (Exception ex)
            {
                // Static serilog.
                Log.Information($"Failed to convert historical currency purchase to USD: Currency: {currency}, Price: {price}, Date: {date}. {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the value from USD to the users currency. 
        /// For example they buy a 5 USD piece in march 2024. And 1 USD is 100 pesos. it should return 500 pesos.
        /// </summary>
        /// <param name="currency">Currency.</param>
        /// <param name="usdPrice">Price in USD.</param>
        /// <param name="date">Date of sale.</param>
        /// <returns></returns>
        public async static Task<decimal> GetHistoricalValueFromUsd(string currency, decimal usdPrice, DateTime date)
        {
            try
            {
                currency = currency.ToLower();
                if (currency == "usd")
                    return usdPrice;

                string formattedDate = date.ToString("yyyy-MM-dd"); // This is how the API likes the date.
                string MainURL = $"https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@{formattedDate}/v1/currencies/usd.json";
                string fallbackURL = $"https://{formattedDate}.currency-api.pages.dev/v1/currencies/usd.json";

                HttpResponseMessage response = await _client.GetAsync(MainURL);
                if (response == null || !response.IsSuccessStatusCode)
                {
                    response = await _client.GetAsync(fallbackURL);
                    response.EnsureSuccessStatusCode();
                }

                var jsonReponse = await response.Content.ReadAsStringAsync();
                var currencies = JsonSerializer.Deserialize<UsdCurrencyValueDTO>(jsonReponse);
                foreach (var curr in currencies.Values)
                {
                    if (curr.Key == currency)
                    {
                        // When rounding currency its not always up, its standard .5 and above is rounded up. So like normal math.
                        var convertedValue = Math.Round((usdPrice * curr.Value), 2);
                        return convertedValue;
                    }
                }

                // Still nothing? Currency likely not supported...
                throw new Exception($"{currency} is not supported");
            }
            catch (Exception ex)
            {
                // Static serilog.
                Log.Information($"Failed to convert historical USD purchase to {currency}: USD Price: {usdPrice}, Date: {date}. {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Turns the value from USD currency to user currency.
        /// </summary>
        /// <param name="currency">ABBREVIATED CURRENCY (aud for example).</param>
        /// <param name="UsdPrice">Price in USD.</param>
        /// <returns>Price converted from Usd to currency.</returns>
        public  static decimal ConvertPriceToCurrency(string currency, decimal UsdPrice)
        {
            try
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
            catch (Exception ex)
            {
                Log.Information($"Failed to convert a price to currency: Currency: {currency}, UsdPrice: {UsdPrice}. {ex.Message}");
                throw;
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
                    if(response == null)
                    {
                        throw new CurrencyCallException("Failed to get USD values. API likely down.");
                    }
                }
                var jsonReponse = await response.Content.ReadAsStringAsync();
                var currencies = JsonSerializer.Deserialize<UsdCurrencyValueDTO>(jsonReponse);
                UsdValues = currencies;
            }
            catch (Exception ex)
            {
                Log.Information($"Failed to get USD values. {ex.Message}");
                throw;
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
                pricedItem.Price = ConvertPriceToCurrency(currency, pricedItem.Price);
            }

            return list;
        }
    }
}
