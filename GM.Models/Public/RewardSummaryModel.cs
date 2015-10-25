using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GM.Models.Public
{

    public class RewardsSummaryListModel
    {
        public IList<RewardSummaryModel> RewardsList { get; set; }

        public int Page { get; set; }
        public int NumPages { get; set; }
        public int NumRewards { get; set; }
    }

    public class RewardSummaryModel
    {
        public int Id { get; set; }

        /// <summary>
        /// Name of the reward.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Name of the bussines (partner).
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// ID of the small image for this offer. Shown when the reward is in a list of rewards.
        /// </summary>
        public virtual int? ImageSmallId { get; set; }

        public string City { get; set; }
        public int NumOfRedemptions { get; set; }


        public RewardState RewardState { get; set; }

        //TODO - consider removing this
        /// <summary>
        /// Moderation state, must be approved (or null for admin created ones) to show
        /// </summary>
        public virtual int? State { get; set; }

        public bool Displayed { get; set; }
    }

}