using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.ViewModels
{
    public class PasswordResetViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^'&*-]).{8,}$", ErrorMessage = "Password must contain at least: one Uppercase letter, one lowercase letter, one digit and one special character")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^'&*-]).{8,}$", ErrorMessage = "Password must contain at least: one Uppercase letter, one lowercase letter, one digit and one special character")]
        [Required]
        [Compare("Password", ErrorMessage =
            "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email Address is Required")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Enter a valid email.")]
        public string Email { get; set; }

        public string ResetCode { get; set; }
    }
}
