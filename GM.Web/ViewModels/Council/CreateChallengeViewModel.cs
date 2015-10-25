using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GM.Models.Public;
using GM.ViewModels.Shared;
using System.Web.Mvc;

namespace GM.ViewModels.Council
{
    public class CreateChallengeViewModel : LayoutViewModel
    {
        public int Id { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsInitialRegistrationStep { get; set; }

        [StringLength(35, ErrorMessage = "This field is limited to 35 characters.")]
        [Required(ErrorMessage = "The Challenge/campaign name field is required")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be up to 100 characters long.")]
        // [Required(ErrorMessage = "The Earn description field is required")]
        public string EarnAmount { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be up to 200 characters long.")]
        //  [Required(ErrorMessage = "The About field is required")]
        public string About { get; set; }
        // [Required(ErrorMessage = "The Points field is required")]
        public int? Points { get; set; }

        //Images
        public int? LogoImageId { get; set; }
        public HttpPostedFileBase LogoPhoto { get; set; }


        public int? ImageId1 { get; set; }
        public int? ImageId2 { get; set; }
        public int? ImageId3 { get; set; }
        public int? ImageId4 { get; set; }
        public List<HttpPostedFileBase> ProfileImage { get; set; }

        // [Required(ErrorMessage = "The challenge category field is required")]
        public int? ChallengeCategoryId { get; set; }

        [StringLength(20, ErrorMessage = "The {0} must be up to 20 characters long.")]
        public string PromoCode { get; set; }

        //[StringLength(1000, ErrorMessage = "The {0} must be up to 1000 characters long.")]

        [AllowHtml]
        public string Article { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Prompt = "Start date")]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EndDate { get; set; }
        public int? DurationInDays { get; set; }

        // [Required(ErrorMessage = "You must select when to award points.")]
        public int? AwardPointsOptionId { get; set; }
        // [Required(ErrorMessage = "You must select participation frequency.")]
        public int? ParticipationFrequencyId { get; set; }

        // [StringLength(300, ErrorMessage = "The {0} must be up to 300 characters long.")]

        [AllowHtml]
        public string Purpose { get; set; }
       // [StringLength(300, ErrorMessage = "300 character limit")]
        [AllowHtml]
        public string Instructions { get; set; }
        //[StringLength(300, ErrorMessage = "300 character limit")]
        [AllowHtml]
        public string TermsAndConditions { get; set; }

        public int? LocationOptionId { get; set; }

        public string StreetAddress { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        [AllowHtml]
        public string DateHoursTime { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", ErrorMessage = "Not a valid email"),]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Web { get; set; }
        public bool SendEmailNotifications { get; set; }
        public string SendEmailTo { get; set; }
        public bool Display { get; set; }

        public int? Popularity { get; set; }
        public int? Status { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }

        public SelectList EmailsSelectList { get; set; }
        public SelectList StatesSelectList { get; set; }
        public SelectList CategorySelectList { get; set; }
        public SelectList AwardPointsSelectList { get; set; }
        public SelectList FrequencySelectList { get; set; }
        public SelectList SendEmailsToSelectList { get; set; }


        // public int? Instance_Id { get; set; }

    }
}