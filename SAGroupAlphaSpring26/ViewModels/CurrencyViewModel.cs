namespace SAGroupAlphaSpring26.ViewModels
{
    public class CurrencyViewModel
    {
        public Dictionary<string, string> Currencies { get; set; }
        public string CurrentChoice = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}
