using System;

namespace GM.ViewModels.Shared
{
    public class LayoutViewModel
    {
        public bool IsAuthenticated { get; set; }
        public bool IsSuplier { get; set; }
        
        //council
        public bool IsCouncil { get; set; }

        //auspost
        public bool IsAusPost { get; set; }      
  
        public int? Instance_Id { get; set; }

        //TODO check provider user key and user id
        public Guid CurrentAccountId { get; set; }
        public string ProviderUserKey { get; set; }
        public string CurrentFirstName { get; set; }
        public string CurrentLastName { get; set; }
        public string CurrentUserEmail { get; set; }
        public int? CurrentUserPhotoId { get; set; }
        public long CurrentUserTotalPoints { get; set; }
        public long CurrentUserTotalCart { get; set; }
        public long CurrentUserTotalChallenges { get; set; }
        
        //council
        public string CurrentUserInstance { get; set; }
        public int UserChallengesCount { get; set; }

        public string Title { get; set; }
        public bool HideTopWrapperMenu { get; set; }
        public Links ActiveLink { get; set; }
    }

    public enum Links
    {
        None,
        HomePage,
        Account, AccountMyProfile, AccountUpdateProfile, AccountInbox, AccountMyCart, AccountMyWallet, AccountAdditionalUsers,
        SupplierDashboard, SupplierUpdateProfile, SupplierRedemptions,
        Rewards,
        SupplierRewards, CreateReward,
        Challenges, CreateChallenge
    }
}