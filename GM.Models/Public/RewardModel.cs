using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GM.Models.Public
{
    public class RewardsListModel
    {
        public IList<RewardModel> RewardsList { get; set; }

        public int Page { get; set; }

        public int NumPages { get; set; }

        public int NumRewards { get; set; }
    }

    public enum RewardState
    {
        [Description("NotModerated")]
        NotModerated = 0,
        [Description("Incomplete")]
        Incomplete = 1,
        [Description("Waiting approval")]
        WaitingApproval = 2,
        [Description("Not displayed")]
        NotDisplayed = 3,
        [Description("Active")] //Approved
        Approved = 4
    }


    public class RewardModel
    {
        public int Id { get; set; }
        public Guid? Owner_Id { get; set; }
        public bool Display { get; set; }

        /// <summary>
        /// Name of the reward.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Description of the reward.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Number of points the reward costs.
        /// </summary>
        public virtual long Price { get; set; }

        /// <summary>
        /// Number of points the reward costs.
        /// </summary>
        public virtual decimal DollarPrice { get; set; }
 
        //Profile images
        public List<UploadModel> ProfileImages { get; set; } //TODO
        public int? ImageId { get; set; }
        public int? Image2Id { get; set; }
        public int? Image3Id { get; set; }
        public int? Image4Id { get; set; }

        /// <summary>
        /// Logo image!!!
        /// ID of the small image for this offer. Shown when the reward is in a list of rewards.
        /// </summary>
        public virtual int? ImageSmallId { get; set; }

        public int CategoryId { get; set; }

        public string PartnerName { get; set; }
        public string PartnerLocation { get; set; }
        public string PartnerHours { get; set; }
        public string PartnerPhone { get; set; }
        public string PartnerEmail { get; set; }
        public string PartnerUrl { get; set; }
        public string PartnerDescription { get; set; }
        public string TermsAndConditions { get; set; }
        
        public string HowToRedeem { get; set; }
        public virtual int? VoucherBarcodeId { get; set; }
        public virtual DateTime? ValidFrom { get; set; }
        public virtual DateTime? ValidTo { get; set; }

        //Supplier
        /// <summary>
        /// Moderation state, must be approved (or null for admin created ones) to show
        /// </summary>
        public virtual int? State { get; set; }
        public bool NotifyOnRedeem { get; set; }

        // New fields
        public DateTime? DateAdded { get; set; }
        public bool? Mobile { get; set; }
        public string Instructions { get; set; }
        public int? Instance_Id { get; set; }

    }

    public class RewardCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        // New fields
        public int? ParentId { get; set; }
        public string Photo { get; set; }

        //Display
        public bool IsSelected { get; set; }

    }
}
