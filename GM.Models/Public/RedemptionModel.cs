using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Models.Public
{
    public class RedemptionModel
    {
        public string PartnerName { get; set; }
        public Guid VoucherId { get; set; }
        public string Code { get; set; }
        public DateTime Issued { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public Guid UserId { get; set; }
        public string RewardName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
    }
}
