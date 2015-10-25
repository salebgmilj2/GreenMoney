using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GM.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Compare("Email", ErrorMessage = "Email and confirmation email do not match.")]
        [Display(Name = "Confirm Email")]
        public string ConfirmEmail { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Display(Name = "Postcode")]
        [Required, StringLength(4)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter a valid Australian postcode")]
        //[StringLength(4, ErrorMessage = "Enter a valid Australian postcode")]
        public string Postcode { get; set; }

        [Display(Name = "Send me latest GreenMoney bonus offers and updates via email")]
        public bool SendEmailOffers { get; set; }

    }
}