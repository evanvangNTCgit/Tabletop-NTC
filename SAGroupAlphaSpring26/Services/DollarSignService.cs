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
            if (currency == null)
            {
                return "$";
            }
            switch (currency.ToUpper())
            {
                case ("EUR"): // Euros
                    return "€";
                case ("MXN"): // Mexican Pesos
                    return "MEX$";
                case ("USD"): // US Dollars
                    return "$";
                case ("AUD"): // Australian Dollars
                    return "AU$";
                case ("CAD"): // Canadian Dollars
                    return "CAD$";
                default:
                    return "$";
            }
        }
    }
}
