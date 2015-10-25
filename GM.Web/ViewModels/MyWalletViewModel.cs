using GM.Models.Public;
using GM.ViewModels.Shared;

namespace GM.ViewModels
{
    public class MyWalletViewModel : LayoutViewModel
    {
        public CheckoutModel CartModel { get; set; }
        public bool ShowChallengesView { get; set; }
    }
}