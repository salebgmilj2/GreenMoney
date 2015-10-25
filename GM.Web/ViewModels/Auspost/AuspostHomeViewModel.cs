using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GM.ViewModels.Shared;
using System.ComponentModel.DataAnnotations;
using GM.Utility.ComponentAttributes;

namespace GM.ViewModels
{
    public class AuspostHomeViewModel : LayoutViewModel
    {
        public LoginAuspostViewModel LoginViewModel { get; set; }
    }

    public class LoginAuspostViewModel : LayoutViewModel
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Enter a valid work email address")]
        [ValidateEmail(ErrorMessage = "Enter a valid work email address")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }


}