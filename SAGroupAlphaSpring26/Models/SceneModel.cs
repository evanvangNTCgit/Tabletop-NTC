namespace SAGroupAlphaSpring26.Models
{
    public class SceneModel
    {
        public int SessionId { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public int? MapPieceId { get; set; }

        public List<int> TokenIdsToClone { get; set; } = new List<int>();
    }
}
