using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{

    public class ForgotPasswordViewModel : LayoutViewModel
    {
        [Required, DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }
    }
}