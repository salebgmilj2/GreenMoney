using System.Collections.Generic;
using System.Web.Mvc;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels.Supplier
{
    public class RedemptionDetailsViewModel: LayoutViewModel
    {
        public RewardModel RewardModel { get; set; }

        public List<RedemptionModel> RedemptionsModel { get; set; }
        public string Filter { get; set; }
        public int Page { get; set; }
        public int NumPages { get; set; }
        public int NumRedemptions { get; set; }

        public RedemptionsInterval ReedemptionsInterval { get; set; }

        public IEnumerable<SelectListItem> RewardsList { get; set; }

    }

    public enum RedemptionsInterval
    {
        Last7Days,
        ThisMonth,
        LastMonth
    }
}