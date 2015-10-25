using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels.Supplier
{
    public class SupplierRewardsViewModel : LayoutViewModel
    {
        public RewardsListModel Rewards { get; set; }
        public List<UploadModel> Uploads { get; set; }
    }
}