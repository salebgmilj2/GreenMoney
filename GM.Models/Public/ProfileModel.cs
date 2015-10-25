using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Models.Public
{

    public class ProfileModelBase
    {
        public UserModel User { get; set; }

        public string GravatarHash { get; set; }

        public long TotalEarnings { get; set; }

        public long TotalRedemptions { get; set; }

        public long CartTotal { get; set; }

        public long ChallengesTotal { get; set; }
                     
        public long PointsAvailable { get; set; }

        public int? Instance_Id { get; set; }
    }

    public class ProfileModel : ProfileModelBase
    {
        public AddressModel Address { get; set; }

        public string DolarPriceSum { get; set; }

        public IEnumerable<TransactionModel> Transactions { get; set; }

        public int Page { get; set; }

        public int NumPages { get; set; }
    }
}
