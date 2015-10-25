using System;

namespace GM.Models.Public
{
    public class VoucherModel
    {
        public Guid VoucherId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public DateTime Issued { get; set; }

        public RewardModel RewardModel { get; set; }

    }
}
