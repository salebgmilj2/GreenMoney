using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class DashboardViewModel : LayoutViewModel
    {
        public ProfileModel ProfileModel { get; set; }
        public ActivitySummaryModel ActivitySummaryModel { get; set; }
        public RewardsSummaryListModel RewardsSummaryListModel { get; set; }
        public HouseholdMemberModel HouseholdMemberModel { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", this.ProfileModel.User.FirstName, this.ProfileModel.User.LastName); }
        }

        public string Address { get; set; }

        public bool ShowNextPage
        { 
            get 
            {
                return this.ProfileModel.NumPages > this.ProfileModel.Page;
            }
        }

        public bool ShowPrevPage
        {
            get
            {
                return this.ProfileModel.Page > 1;
            }
        }
    }
}