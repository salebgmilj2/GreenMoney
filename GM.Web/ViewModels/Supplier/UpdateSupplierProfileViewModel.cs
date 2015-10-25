using System.ComponentModel.DataAnnotations;
using System.Web;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels.Supplier
{
    public class UpdateSupplierProfileViewModel : LayoutViewModel
    {
        public bool Success { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }

        [Display(Name = "ABN")]
        public string BusinessNumberABN { get; set; }

        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }

        [Display(Name = "Bussines WebSite")]
        public string BussinesWebSite { get; set; }

        [Required]
        [Display(Name = "Bussines Email")]
        public string BussinesEmail { get; set; }

        [Display(Name = "Last name")]
        public string BussinesPhone { get; set; }

        [Display(Name = "Area")]
        public string BussinesPhoneArea { get; set; }

        [Display(Name = "Phone Mobile")]
        public string BussinesPhoneMobile { get; set; }

        [Display(Name = "Bussines Location")]
        public string BussinesLocation { get; set; }

        [Display(Name = "Email Bussines On Voucher Redeem")]
        public bool EmailBussinesOnVoucherRedeem { get; set; }

        public bool SendEmailUpdates { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string ConfirmNewPassword { get; set; }

        public int? PhotoID { get; set; }
        public HttpPostedFileBase Photo { get; set; }

        //TODO make better solution
        public string Address { get; set; }
        public bool IsDefaultSupplierAddress { get; set; }
    }
}