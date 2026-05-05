namespace SAGroupAlphaSpring26.Services
{
    public static class DollarSignService
    {
        /// <summary>
        /// Returns proper dollar sign based on parameter
        /// </summary>
        /// <param name="currency">Currency abbreviated 'aud' for example</param>
        /// <returns>Dollar sign for user currency.</returns>
        public static string DollarSignCurrency(string currency)
        {
            switch (currency.ToUpper())
            {
                case ("EUR"): // Euros
                    return "€";
                case ("MXN"): // Mexican Pesos
                case ("USD"): // US Dollars
                case ("AUD"): // Australian Dollars
                    return "$";
                case ("CAN"): // Canadian Dollars
                    return "CAD$";
                default:
                    return "$";
            }
        }
    }
}
