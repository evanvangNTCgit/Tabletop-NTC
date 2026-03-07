namespace SAGroupAlphaSpring26.Models
{
    // used to store session info.
    public class Session
    {
        // Session ID, used to link sessions to users and pieces.
        public int Id { get; set; }

        // session notes, for user to store their own notes.
        public string Notes { get; set; } = string.Empty;

        // stores the last time the session was updated. Allows users to sort by most recent.
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // User ID, used to link sessions to users.
        [Required]
        public int UserId { get; set; }

        // The navigation property for user in session.
        public User? User { get; set; }

        // Stores tokens for the session.
        public List<Token> Tokens { get; set; } = new List<Token>();
    }
}
