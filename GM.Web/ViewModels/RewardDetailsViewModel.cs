using System.Collections.Generic;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class RewardDetailsViewModel  : LayoutViewModel
    {
        public RewardModel Reward { get; set; }

        public bool ShowSlider { get; set; }

        public string SortBy { get; set; }

        public string SortByText { get; set; }

        public RewardCategory Category { get; set; }

        public bool HasCategorySelected
        {
            get { return Category != null; }
        }

        public IEnumerable<RewardCategory> Categories { get; set; }

        public RewardCategory SelectedCategory { get; set; }

        public string Filter { get; set; }

    }

}