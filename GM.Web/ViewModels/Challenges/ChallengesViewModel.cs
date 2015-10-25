using System.Collections.Generic;
using System.ComponentModel;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels.Challenges
{
    public class ChallengesViewModel:LayoutViewModel
    {
        public IEnumerable<ChallengeModel> Challenges { get; set; }
        public int Page { get; set; }
        public int NumPages { get; set; }
        public int NumRewards { get; set; }
        public string SortBy { get; set; }
        public string SortByText { get; set; }
        public ChallengeCategory Category { get; set; }

        public bool HasCategorySelected
        {
            get { return Category != null; }
        }

        public List<ChallengeCategory> Categories { get; set; }

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