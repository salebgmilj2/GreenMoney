using System;
using System.Collections.Generic;
using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class AdditionalAccountsViewModel : LayoutViewModel
    {
        public List<UserModel> AdditionalAccounts { get; set; }
        public List<UserModel> InvitedUsersAccounts { get; set; }

        public HouseholdMemberModel HouseholdMemberModel { get; set; }

    }
 
}