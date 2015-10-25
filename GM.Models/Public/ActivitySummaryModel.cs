using GM.Models.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GM.Models.Public
{
    public class ActivitySummaryModel
    {
        // 7 days

        //public int VisitedRewardPage { get; set; }
        public int OveralPopularity { get; set; }
        public int VoucherRedeemed { get; set; }
        public double AverageAge { get; set; }
        public string Gender { get; set; }

    }
}