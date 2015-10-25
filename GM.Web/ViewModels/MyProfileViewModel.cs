using GM.Models.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class MyProfileViewModel : LayoutViewModel
    {
        public ProfileModel ProfileModel { get; set; }
        public HouseholdMemberModel HouseholdMemberModel { get; set; }

        public string EmailAddress { get; set; }
        public string BirthDate { get; set; }
        public string NumberOfYears { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", this.ProfileModel.User.FirstName, this.ProfileModel.User.LastName); }
        }

        public string AddressFull { get; set; }

        public bool HasBirthDate
        {
            get { return !string.IsNullOrWhiteSpace(BirthDate); }
        }
        public bool HasGender
        {
            get { return !string.IsNullOrWhiteSpace(this.ProfileModel.User.Sex); }
        }
        public bool HasPhoneNumber
        {
            get { return !string.IsNullOrWhiteSpace(this.ProfileModel.User.PhoneNumber); }
        }


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


        //Auspost specific
        public int NumberOfChallenges { get; set; }
    }
}