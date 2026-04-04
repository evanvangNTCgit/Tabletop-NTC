namespace SAGroupAlphaSpring26.ViewModels
{
    // A simple view model for logging in
    // We need a users email address and password
    // To then query for the user by their email.
    // Then get check if it truly is them via password (is hashsed).
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public required string EmailAddress { get; set; } // added required to get rid of warning.

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; } // added required to get rid of warning.
    }
}
