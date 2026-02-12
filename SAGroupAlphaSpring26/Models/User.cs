namespace SAGroupAlphaSpring26.Models
{
    // User class, used to store user information.
    public class User
    {
        //
        public int Id { get; set; }

        //
        public string Username { get; set; } = string.Empty;

        //
        public string Email { get; set; } = string.Empty;

        //
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
