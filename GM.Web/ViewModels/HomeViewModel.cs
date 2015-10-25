using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class HomeViewModel : LayoutViewModel
    {
        public RegistrationViewModel RegistrationViewModel { get; set; }
        public LoginViewModel LoginViewModel { get; set; }

        public RegistrationSupplierViewModel RegistrationSupplierViewModel { get; set; }

        //Simple user
        public bool RegistrationIsNotValid { get; set; }
        public bool PostCodeIsNotMatching { get; set; }
        public bool EnterFbRgstrPostCode { get; set; }

        //Supplier
        public bool SupplierRegistrationIsNotValid { get; set; }

    }
}