using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GM.Models.Public;
using GM.ViewModels.Shared;
using System.Web.Mvc;

namespace GM.ViewModels.Supplier
{
    public class CreateRewardViewModel : LayoutViewModel
    {
        public int Id { get; set; } 
        public bool IsUpdate { get; set; }
        public bool IsInitialRegistrationStep { get; set; }
        public bool IsSentForApprove { get; set; }

        //public RewardModel RewardModel { get; set; }
        public string PartnerLocationCurrent { get; set; }
        public string PartnerLocationNewAddress { get; set; }
        public string PartnerLocationNewState { get; set; }
        public string PartnerLocationNewPostCode { get; set; }

        //[Required(ErrorMessage = "The Partner hours field is required")]
        [AllowHtml]
        public string PartnerHours { get; set; }
        public string PartnerUrl { get; set; }

        [Required(ErrorMessage = "The Partner description field is required")]
        [AllowHtml]
        public string PartnerDescription { get; set; }

        [Required]
        public string PartnerName { get; set; }

        [StringLength(35, ErrorMessage = "The {0} must be up to 35 characters long.")]
        [Required]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be up to 200 characters long.")]
        [Required]
        [AllowHtml]
        public string Description { get; set; }

        [Required]
        [RegularExpression(@"[0-9\-\(\)\+#\*,;]+", ErrorMessage = "Number is invalid.")]
        public long Price { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public int CategoryId { get; set; }

        public int? ImageSmallId { get; set; }
        public HttpPostedFileBase LogoPhoto { get; set; }


        //Images
        public int? ImageId { get; set; }
        public int? Image2Id { get; set; }
        public int? Image3Id { get; set; }
        public int? Image4Id { get; set; }

        public List<HttpPostedFileBase> ProfileImage { get; set; } //TODO

        public int? VoucherBarcodeId { get; set; }
        public HttpPostedFileBase BarCodeImage { get; set; }

        [AllowHtml]
        public string HowToRedeem { get; set; }
        public string Instructions { get; set; }
        [AllowHtml]
        public string TermsAndConditions { get; set; }

        public bool Mobile { get; set; }


        public IEnumerable<SelectListItem> OwnersEmailsList { get; set; }
        public string PartnerEmail { get; set; }

        public bool NotifyOnRedeem { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }


        public bool EnableDisplayRewardOption { get; set; }
        public bool Display { get; set; }
    }
}