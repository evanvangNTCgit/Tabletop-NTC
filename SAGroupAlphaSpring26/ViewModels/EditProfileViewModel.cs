namespace SAGroupAlphaSpring26.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public required string EmailAddress { get; set; }


        public Dictionary<string, string>? Currencies { get; set; }

        [Required]
        [Display(Name = "Currency")]
        public string UserCurrentCurrency { get; set; }
    }
}
