using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Models.Public
{
    public class OrderSummaryItemModel
    {
        // Reward name
        public string Name { get; set; }

        public long Points { get; set; }

        public long Quantity { get; set; }

        public decimal DollarCost { get; set; }
        public long Cost { get; set; }

        public int PayPalIndex { get; set; }       // has if DollarCost > 0

        public string VoucherUrl { get; set; }     // has if DollarCost = 0
        public Guid VoucherId { get; set; }     // has if DollarCost = 0

        public bool AttachVoucherUrl { get; set; } // true if DollarCost = 0

        public bool HasDollarCost { get; set; }

        public bool NotifyOnRedeem { get; set; }


        // For NotifyOnRedeem email if NotifyOnRedeem is true

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string PartnerName { get; set; }
        public string PartnerOwnerEmail { get; set; }
        public string Category { get; set; }

    }
}
