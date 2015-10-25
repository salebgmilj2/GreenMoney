using GM.Models.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GM.Models.Public
{
    public class RedemptionStatisticModel
    {
        public int RewardId { get; set; }

        public string PartnerName { get; set; }
        public string RewardName { get; set; }
        public int? ImageSmallId { get; set; }
        public int? ImageId { get; set; }

        public int RedeemsForLast7Days { get; set; }
        public int RedeemsForThisMonth { get; set; }
        public int RedeemsForLastMonth { get; set; }

    }
}