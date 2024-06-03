using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModals
{
    public class RegistrationVM
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address")]

        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, ErrorMessage = "Password should be between {2} and {1} characters.", MinimumLength = 6)]
        public string? Passwordhash { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "confirmPassword is required")]
        [Compare("Passwordhash", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPasswordhash { get; set; }
    }
}
