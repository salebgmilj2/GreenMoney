using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GM.Models.Public
{
    public class ChallengeListModel
    {
        public IList<ChallengeModel> ChallengesList { get; set; }
        public int Page { get; set; }
        public int NumPages { get; set; }
        public int NumRewards { get; set; }
    }

    public class ChallengesListModel
    {
        public IList<ChallengeModel> ChallengesList { get; set; }
        public int Page { get; set; }
        public int NumPages { get; set; }
        public int NumChallenges { get; set; }
    }

    public class ChallengeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EarnAmount { get; set; }
        public string About { get; set; }
        public int? Points { get; set; }
        public int? LogoImageId { get; set; }
        public int? ImageId1 { get; set; }
        public int? ImageId2 { get; set; }
        public int? ImageId3 { get; set; }
        public int? ImageId4 { get; set; }
        public int? ChallengeCategoryId { get; set; }
        public string PromoCode { get; set; }
        public string Article { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DurationInDays { get; set; }
        public int? AwardPointsOptionId { get; set; }
        public int? ParticipationFrequencyId { get; set; }
        public string Purpose { get; set; }
        public string Instructions { get; set; }
        public string TermsAndConditions { get; set; }
        public int? LocationOptionId { get; set; }
        public string StreetAddress { get; set; }
        public string Suburb { get; set; }
        public ChallengeState ChallengeState { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string DateHoursTime { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Web { get; set; }
        public bool SendEmailNotifications { get; set; }
        public string SendEmailTo { get; set; }
        public bool Display { get; set; }
        public int? Popularity { get; set; }
        public DateTime? DateAdded { get; set; }
        public int Instance_Id { get; set; }
        public System.Guid Owner_Id { get; set; }

        public bool IsUpdate { get; set; }
        public List<UploadModel> ProfileImages { get; set; }
        public int Participants;
        public int? Status { get; set; }
        public string ChallengeCategory;
        public string ChallengeStatus { get; set; }



        public bool ShowImage
        {
            get { return ImageId1.HasValue; }

        }

        public bool ShowAddedButton
        {
            get { return ChallengeState.Equals(ChallengeState.Completed) || ChallengeState.Equals(ChallengeState.Active); }
        }

        public string ChallengeCategoryShortName { get; set; }
    }

    public class ChallengeCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? ImageId { get; set; }

    }    

    public class ChallengeAwardPointsOption
    {
        public int Id { get; set; }
        public string Name { get; set; }

        bool IsSelected { get; set; }
    }

    public class ChallengeParticipationFrequency
    {
        public int Id { get; set; }
        public string Name { get; set; }

        bool IsSelected { get; set; }
    }

    public enum ChallengeState
    {
        [Description("NotAdded")]
        NotAdded = 0,
        [Description("Active")]
        Active = 1,
        [Description("Completed")]
        Completed = 2
    }

    public enum ChallengeCategoryEnum
    {
        Action,
        Pledge,
        Learn
    }

    public enum States
    {
        ACT,
        NSW,
        NT,
        QLD,
        SA,
        TAS,
        VIC,
        WA
    }
}
