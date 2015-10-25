using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GM.ViewModels
{
    public class RegistrationSupplierViewModel
    {

        [Required]
        public int InstanceId { get; set; }
        public IEnumerable<SelectListItem> Instances { get; set; }
            
        [Required]
        [Display(Name = "Business name")]
        public string BusinessName { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        [Display(Name = "Business email")]
        public string BusinessEmail { get; set; }

        [Required]
        [Display(Name = "Area")]
        public string BussinesPhoneArea { get; set; }

        [Required]
        [Display(Name = "Business phone")]
        public string BusinessPhone { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

    }
}