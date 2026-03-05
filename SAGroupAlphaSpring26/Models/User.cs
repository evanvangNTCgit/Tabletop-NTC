namespace SAGroupAlphaSpring26.Models
{
    // User class, used to store user information.
    public class User
    {
        // ID of the user.
        public int UserId { get; set; }

        // Username of the user
        public string Username { get; set; } = string.Empty;

        // Email address of the user.
        public string Email { get; set; } = string.Empty;

        // The pieces currently owned by the user.
        // Many to many relationship since a user can own many pieces, and a piece can be owned by many users.
        public List<Piece> OwnedPieces { get; set; } = new List<Piece>();

        // A user can be in many sessions. However since a session only has a DM a session CAN ONLY HAVE ONE USER - NO MANY TO MANY
        public List<Session> Sessions { get; set; } = new();
    }
}
