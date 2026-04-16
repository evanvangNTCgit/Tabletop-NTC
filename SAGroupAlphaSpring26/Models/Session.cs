namespace SAGroupAlphaSpring26.Models
{
    // used to store session info.
    public class Session
    {
        // Session ID, used to link sessions to users and pieces.
        public int Id { get; set; }

        // name of session.
        public string Name { get; set; } = string.Empty;

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

        // Instead of the user deleting a session entirely.
        // We can just archive it and have the user restore it, if desired
        [Required]
        public bool IsArchived { get; set; } = false; // Default to false for the ones that did not get this column yet.
    }
}
