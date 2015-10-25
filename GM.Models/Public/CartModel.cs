using System;
using System.Collections.Generic;

namespace GM.Models.Public
{

    public class CartModel
    {
        public IList<CartItemSummary> Items { get; set; }

        public long CartTotal { get; set; }

        public decimal CartDollarTotal { get; set; }

        public long PointsAvailable { get; set; }

        public bool HasVouchers { get; set; }
    }

    public class CartItemSummary
    {
        public int Id { get; set; }

        public int RewardId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public long Price { get; set; }

        public decimal DollarPrice { get; set; }

        public int? ImageId { get; set; }

        public DateTime? DateAdded { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

    }

    public class WalletItemSummary
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public long Price { get; set; }

        public decimal DollarPrice { get; set; }

        public int? CategoryImageId { get; set; }
        public int? ImageId { get; set; }

        public DateTime? DateAdded { get; set; }

        //For rewards it is always DateAdded + 30 days
        //For challenges it can have it's own value
        public DateTime? DateExpired { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public bool? Mobile { get; set; }
        public Guid VoucherId { get; set; }


        // For my challenges
        public bool PointsClaimed { get; set; }
        public string ChallengeCategoryShortName { get; set; }

    }
    
}
