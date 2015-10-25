using System;
using System.Collections.Generic;
using GM.Models.Public;

namespace GM.BusinessLogic.Interfaces
{
    public interface IProfileService
    {
        ProfileModel GetMyProfile(string providerUserKey, string email, int page = 1);

        bool UpdateProfile(string providerUserKey, UpdateProfileModel updateModel);
        bool ChangeUsername(string oldUsername, string newUsername, string connectionString);

        CartModel GetMyCart(string providerUserKey);
        bool AddToMyCart(string providerUserKey, int rewardId, int quantity = 1);
        bool UpdateMyCart(string providerUserKey, IList<CartItemSummary> cartItems);

        CheckoutModel GetMyCartCheckout(string providerUserKey);
        CheckoutSubmitModel AddCartToMyWallet(string providerUserKey);

        CheckoutModel GetMyWallet(string providerUserKey);
    }
}
