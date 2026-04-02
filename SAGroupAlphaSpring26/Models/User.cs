namespace SAGroupAlphaSpring26.Models
{
    // User class, used to store user information.
    public class User
    {
        // ID of the user.
        public int Id { get; set; }

        // First name of the user.
        [Required]
        public required string FirstName { get; set; }

        // Last name of the user.
        [Required]
        public required string LastName { get; set; }

        // Email address of the user.
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        // The password hash of a user
        // WE DO NOT STORE RAW PASSWORDS INTO THE DB
        // Instead a oneway hash so a user does not get account leaked by for example a XSS attack.
        [Required]
        public required string PasswordHash { get; set; }

        // A simple boolean to determine if a user is an admin or not. 
        // If not they cannot do stuff in the admin contoller, instead are redirected to a not authorized screen.
        [Required]
        public bool IsAdmin { get; set; }

        // The pieces currently owned by the user.
        // Many to many relationship since a user can own many pieces, and a piece can be owned by many users.
        public List<UserPieces>? OwnedPieces { get; set; } = new();

        // A user can be in many sessions. However since a session only has a DM a session CAN ONLY HAVE ONE USER - NO MANY TO MANY
        public List<Session> Sessions { get; set; } = new();

        public List<CartItem>? CartItems { get; set; }
    }
}
