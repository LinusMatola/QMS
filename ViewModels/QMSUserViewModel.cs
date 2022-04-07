using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.ViewModels
{
    public class QMSUserViewModel
    {
        [Required(ErrorMessage = "Please key in your first name")]
        [Display(Name = "Name")]
        [DataType(DataType.Text)]
        public string FullName { get; set; }


        [Required(ErrorMessage = "Email Address is Required")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Enter a valid email.")]
        public string Email { get; set; }
    }
}
