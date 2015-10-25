using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class MyCartCheckoutViewModel : LayoutViewModel
    {
        public CartModel CartModel { get; set; }
        public string UserEmail { get; set; }
        public string TotalQuantity { get; set; }
        public string TotalPoints { get; set; }

        public bool CheckoutFailure { get; set; }
    }
}