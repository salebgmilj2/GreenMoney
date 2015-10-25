using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GM.Models.Public
{
    public class UserChallengeListModel
    {
        public IList<UserChallengeModel> UserChallengeList { get; set; }

        public int Page { get; set; }

        public int NumPages { get; set; }

        public int NumChallenges { get; set; }
    }


    public class UserChallengeModel
    {
        public int Id { get; set; }
        [DisplayName("Date/time")]
        public DateTime Issued { get; set; }
        public int ChallengeId { get; set; }
        public System.Guid UserId { get; set; }
        [DisplayName("Status")]
        public bool PointsClaimed { get; set; }
        [DisplayName("Participant")]
        public string UserName { get; set; }
        public string Suburb { get; set; }
        public string Workplace { get; set; }

        //  public virtual ChallengeModel Challenges { get; set; }
        // public virtual UserModel Users { get; set; }
    }
}

