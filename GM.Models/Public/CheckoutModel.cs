using System.Collections.Generic;

namespace GM.Models.Public
{
    public class CheckoutModel
    {
        public IEnumerable<WalletItemSummary> Items { get; set; }

        public long CartTotal { get; set; }

        public decimal CartDollarTotal { get; set; }

        public bool HasVouchers { get; set; }

        public string Email { get; set; }
    }

    public class CheckoutSubmitModel
    {
        public CheckoutSubmitModelState CheckoutSubmitModelState { get; set; }
        public List<OrderSummaryItemModel> Purchases { get; set; }
    }

    public enum CheckoutSubmitModelState
    {
        NoItemsFound,
        NotEnoughPoints,
        SuccessWithProductOrderConfirmation, //DollaCost>0
        SuccessWithOrderConfirmation
    }
}
