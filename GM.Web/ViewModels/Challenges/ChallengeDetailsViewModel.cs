using System.Collections.Generic;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels.Challenges
{
    public class ChallengeDetailsViewModel : LayoutViewModel
    {
        public ChallengeModel Challenge { get; set; }

        public bool ShowSlider { get; set; }

        public string SortBy { get; set; }

        public string SortByText { get; set; }

        public ChallengeCategoryEnum Category { get; set; }

        public IEnumerable<ChallengeCategory> Categories { get; set; }

        public ChallengeCategory SelectedCategory { get; set; }

        public string Filter { get; set; }

        public bool PointsClaimed { get; set; }

        public bool Participating { get; set; }
    }


}