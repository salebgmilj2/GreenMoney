﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class LoginViewModel : LayoutViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
        public bool IsVerified { get; set; }
    }
}