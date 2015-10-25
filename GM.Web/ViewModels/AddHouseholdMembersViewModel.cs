using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GM.Utility.ComponentAttributes;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class AddHouseholdMembersViewModel : LayoutViewModel
    {
        public List<HouseholdMemberModel> HouseholdMembers { get; set; }
    }

    public class HouseholdMemberModel : LayoutViewModel
    {

        public int AddressId { get; set; }
        public string InviterName { get; set; }
        public string InviterId { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm email")]
        public string ConfirmEmail { get; set; }


        [Required, DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Send them the latest GreenMoney bonus offers and updates via email")]
        public bool SendEmailOffers { get; set; }

        [SendEmailUpdates]
        public bool SendEmailUpdates { get; set; }

    }

}