//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GM.DataAccess.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Challenges
    {
        public Challenges()
        {
            this.UserChallenges = new HashSet<UserChallenges>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string EarnAmount { get; set; }
        public string About { get; set; }
        public int? Points { get; set; }
        public Nullable<int> LogoImageId { get; set; }
        public Nullable<int> ImageId1 { get; set; }
        public Nullable<int> ImageId2 { get; set; }
        public Nullable<int> ImageId3 { get; set; }
        public Nullable<int> ImageId4 { get; set; }
        public Nullable<int> ChallengeCategoryId { get; set; }
        public string PromoCode { get; set; }
        public string Article { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> DurationInDays { get; set; }
        public Nullable<int> AwardPointsOptionId { get; set; }
        public Nullable<int> ParticipationFrequencyId { get; set; }
        public string Purpose { get; set; }
        public string Instructions { get; set; }
        public string TermsAndConditions { get; set; }
        public Nullable<int> LocationOptionId { get; set; }
        public string StreetAddress { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string DateHoursTime { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Web { get; set; }
        public bool SendEmailNotifications { get; set; }
        public string SendEmailTo { get; set; }
        public bool Display { get; set; }
        public int Popularity { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public int Instance_Id { get; set; }
        public System.Guid Owner_Id { get; set; }
       
    
        public virtual AwardPointsOptions AwardPointsOptions { get; set; }
        public virtual ChallengeCategories ChallengeCategories { get; set; }
        public virtual Instance Instance { get; set; }
        public virtual LocationOptions LocationOptions { get; set; }
        public virtual ParticipationFrequency ParticipationFrequency { get; set; }
        public virtual ICollection<UserChallenges> UserChallenges { get; set; }
        public virtual Users1 Users1 { get; set; }
    }
}