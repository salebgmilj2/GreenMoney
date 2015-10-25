using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GM.BusinessLogic;
using GM.Models.Public;
using GM.ViewModels.Supplier;
using GM.ViewModels.Council;

namespace GM.Utility
{
    public static class ViewModelHelper
    {
        public static void SetDefaultsForViewModel(CreateRewardViewModel model, MembershipUser membershipUser)
        {
            IEnumerable<RewardCategory> categoriesAll = GetRewardCategories();
            var rewardCategories1 = categoriesAll as IList<RewardCategory> ?? categoriesAll.ToList();
            model.Categories = new SelectList(rewardCategories1.Distinct(), "Id", "Name");

            List<string> ownerEmails = new List<string>();
            ownerEmails.Add(membershipUser.Email);
            model.OwnersEmailsList = new SelectList(ownerEmails.Distinct(), "Id", "Name");
        }

        public static IEnumerable<RewardCategory> GetRewardCategories()
        {
            List<RewardCategory> rewardsCategories = new RewardsService().GetRewardCategories();

            return rewardsCategories;
        }

        public static string GetUserAddress(AddressModel addressModel)
        {
            if(addressModel.UnitNumber != null)
            {
                return string.Format("{0}, {1} {2} {3}, {4}, {5} {6}", addressModel.UnitNumber, 
                    addressModel.StreetNumber, addressModel.StreetName, addressModel.StreetType,
                    addressModel.Suburb, addressModel.State,
                    addressModel.Postcode);
            }
            else
            {
                return string.Format("{1} {2} {3}, {4}, {5} {6}", addressModel.UnitNumber, 
                    addressModel.StreetNumber, addressModel.StreetName, addressModel.StreetType,
                    addressModel.Suburb, addressModel.State,
                    addressModel.Postcode);
            }
        }

        //council
        public static void SetDefaultsForChallengeViewModel(CreateChallengeViewModel model, MembershipUser membershipUser)
        {

            //IEnumerable<ChallengeCategory> categories = new ChallengesService().GetChallengeCategories();
            model.CategorySelectList = new SelectList(GetChallengeCategories(Convert.ToInt32(model.Instance_Id)), "Id", "Name");

            IEnumerable<ChallengeAwardPointsOption> awardOptions = new ChallengesAdminService().GetChallengeAwardPointsOptions();
            model.AwardPointsSelectList = new SelectList(awardOptions, "Id", "Name");

            IEnumerable<ChallengeParticipationFrequency> participationFrequency = new ChallengesAdminService().GetChallengeParticipationFrequency();
            model.FrequencySelectList = new SelectList(participationFrequency, "Id", "Name");

            List<string> emails = new UserService().GetEmailsForRoleInInstance(Convert.ToInt32(model.Instance_Id), "Council");
            model.EmailsSelectList = new SelectList(emails);

            model.StatesSelectList = new SelectList(Enum.GetValues(typeof(States)).Cast<States>());

        }

        public static List<ChallengeCategory> GetChallengeCategories(int instance_Id)
        {
            List<ChallengeCategory> categories = new ChallengesAdminService().GetChallengeCategories(instance_Id);

            return categories;
        }


        public static void SetDefaultsForIndexChallengesViewModel(ChallengesIndexViewModel model, MembershipUser membershipUser)
        {
            var OrderBySelectList = new SelectList(new[] 
            {
                new { ID = "0", Name = "Newest to oldest" },
                new { ID = "1", Name = "Oldest to newest" },
                new { ID = "2", Name = "Participation (high to low)" },
                new { ID = "3", Name = "Ending soon" },
                new { ID = "4", Name = "Most popular" },
            },
          "ID", "Name", 0);

            model.OrderBy = OrderBySelectList;

            var StatusSelectList = new SelectList(new[] 
            {
                new { ID = "0", Name = "Status" },
                new { ID = "1", Name = "Draft" },
                new { ID = "2", Name = "Displayed" },
                new { ID = "3", Name = "Not active" },
                new { ID = "4", Name = "Ending soon" }
            },
         "ID", "Name", 0);

            model.Status = StatusSelectList;


            List<ChallengeCategory> categories = GetChallengeCategories(Convert.ToInt32(model.Instance_Id));
            ChallengeCategory empty = new ChallengeCategory();
            empty.Id = 0;
            empty.Name = "Challenge category";
            categories.Insert(0, empty);
            model.ChallengeCategory = new SelectList(categories, "Id", "Name");
        }

        public static void SetDefaultsForChallengeReportViewModel(ChallengeReportViewModel model, MembershipUser membershipUser)
        {
            var user = new UserService().GetUserById((Guid)membershipUser.ProviderUserKey);

            var OrderBySelectList = new SelectList(new[] 
            {
                new { ID = "0", Name = "Newest to oldest" },
                new { ID = "1", Name = "Oldest to newest" },
            
            },
          "ID", "Name", 0);

            model.OrderBy = OrderBySelectList;

            var ChallengeStatusSelectList = new SelectList(new[] 
            {
                new { ID = "0", Name = "Status" },
                new { ID = "1", Name = "Completed" },
                new { ID = "2", Name = "Not completed" },
            
            },
         "ID", "Name", 0);

            model.ChallengeStatus = ChallengeStatusSelectList;

            List<string> suburbs = new UserService().GetAllSuburbsForInstance(user.Instance_Id);
            if (model.Challenge.Instance_Id == 5)
            {
                suburbs.Insert(0, "Workplace");
            }
            else
            {
                suburbs.Insert(0, "Suburb");
            }

            model.Suburbs = new SelectList(suburbs);


        }

    }
}