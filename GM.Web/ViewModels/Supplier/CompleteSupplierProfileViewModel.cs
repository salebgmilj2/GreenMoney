using System.ComponentModel.DataAnnotations;
using System.Web;
using GM.ViewModels.Shared;

namespace GM.ViewModels.Supplier
{
    public class CompleteSupplierProfileViewModel : LayoutViewModel
    {
 
        [Display(Name = "ABN")]
        public string BusinessNumberABN { get; set; }

        [Required]
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }

        [Required]
        [Display(Name = "Bussines WebSite")]
        public string BussinesWebSite { get; set; }

    }
}