namespace SAGroupAlphaSpring26.Models
{
    public class Scene
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public int SessionId { get; set; }
        
        public Session? Session { get; set; }

        public List<Token> Tokens { get; set; } = new List<Token>();
    }
}
