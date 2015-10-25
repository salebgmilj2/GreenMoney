using System.Collections.Generic;
using System.ComponentModel;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class RewardsViewModel : LayoutViewModel
    {
        public IEnumerable<RewardModel> Rewards { get; set; }

        public int? InstanceId { get; set; }

        public bool ShowDropdownCity { get; set; }

        public int Page { get; set; }

        public int NumPages { get; set; }

        public int NumRewards { get; set; }

        public string SortBy { get; set; }

        public string SortByText { get; set; }

        public RewardCategory Category { get; set; }
        public List<RewardCategory> SubCategories { get; set; }

        public bool HasCategorySelected
        {
            get { return Category != null; }
        }

        public IEnumerable<RewardCategory> Categories { get; set; }

        public string Filter { get; set; }

    }

    public enum SortByEnum
    {
        [Description("Popularity")]
        popular,
        [Description("Offer: Newest to Oldest")]
        newest,
        [Description("Offer: Oldest to Newest")]
        oldest,
        [Description("Points: Low to High")]
        cheapest,
        [Description("Points: High to Low")]
        expensive
    }
}