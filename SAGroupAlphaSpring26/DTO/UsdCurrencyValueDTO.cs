using System.Text.Json.Serialization;

namespace SAGroupAlphaSpring26.DTO
{
    public class UsdCurrencyValueDTO
    {
        [JsonPropertyName("usd")]
        public Dictionary<string, decimal> Values { get; set; }
    }
}
