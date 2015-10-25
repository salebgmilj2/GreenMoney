using System.Collections.Generic;
using System.Web.Mvc;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels.Supplier
{
    public class RedemptionsViewModel: LayoutViewModel
    {
        public RedemptionsStatisticsListModel RedemptionStatistics { get; set; }

    }
}