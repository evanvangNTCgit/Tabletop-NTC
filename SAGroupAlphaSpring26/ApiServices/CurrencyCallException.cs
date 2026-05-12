namespace SAGroupAlphaSpring26.ApiServices
{
    [Serializable]
    public class CurrencyCallException : Exception
    {
        // Constructors
        public CurrencyCallException(string message)
            : base(message)
        { }
    }
}
