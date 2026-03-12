using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace SAGroupAlphaSpring26.ViewModels
{
    public class SessionCreationViewModelModel : PageModel
    {
        // Require a name for the session.
        [Required(ErrorMessage = "Please enter a name for your session.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        // Allow for no notes.
        [AllowNull]
        [Display(Name = "Notes")]
        public string Notes { get; set; } = string.Empty;

        // 
    }
}
