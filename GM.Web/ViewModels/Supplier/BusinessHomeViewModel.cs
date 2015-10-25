using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class BusinessHomeViewModel : LayoutViewModel
    {
        public LoginViewModel LoginViewModel { get; set; }
        public RegistrationSupplierViewModel RegistrationSupplierViewModel { get; set; }
        public bool SupplierRegistrationIsNotValid { get; set; }
    }
}