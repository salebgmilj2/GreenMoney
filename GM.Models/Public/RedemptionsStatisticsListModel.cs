using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Models.Public
{
    public class RedemptionsStatisticsListModel
    {
        public IList<RedemptionStatisticModel> RewardsList { get; set; }

        public int Page { get; set; }

        public int NumPages { get; set; }

        public int NumRewards { get; set; }
    }

}
