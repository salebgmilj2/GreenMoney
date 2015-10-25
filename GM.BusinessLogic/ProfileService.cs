using System;
using System.Collections.Generic;
using GM.BusinessLogic.Interfaces;
using GM.DataAccess;
using GM.Models.Public;

namespace GM.BusinessLogic
{
    public class ProfileService : IProfileService
    {
        public ProfileModel GetMyProfile(string providerUserKey, string email, int page = 1)
        {
            return new ProfileRepository().GetMyProfile(providerUserKey, email, page);
        }

        public ProfileModelBase GetMyProfileBase(string providerUserKey, string email)
        {
            return new ProfileRepository().GetMyProfileBase(providerUserKey, email);
        }

        public bool UpdateProfile(string providerUserKey, UpdateProfileModel updateModel)
        {
            return new ProfileRepository().UpdateProfile(providerUserKey, updateModel);
        }

        public CartModel GetMyCart(string providerUserKey)
        {
            return new ProfileRepository().GetMyCart(providerUserKey);
        }

        public bool AddToMyCart(string providerUserKey, int rewardId, int quantity = 1)
        {
            return new ProfileRepository().AddToMyCart(providerUserKey, rewardId, quantity);
        }

        public bool UpdateMyCart(string providerUserKey, IList<CartItemSummary> cartItems)
        {
            return new ProfileRepository().UpdateMyCart(providerUserKey, cartItems);
        }

        public CheckoutModel GetMyCartCheckout(string providerUserKey)
        {
            return new ProfileRepository().GetMyCartCheckout(providerUserKey);
        }

        public CheckoutSubmitModel AddCartToMyWallet(string providerUserKey)
        {
            return new ProfileRepository().AddCartToMyWallet(providerUserKey);
        }

        public CheckoutModel GetMyWallet(string providerUserKey)
        {
            return new ProfileRepository().GetMyWallet(providerUserKey);
        }

        public bool ChangeUsername(string userName, string email, string connectionString)
        {
            return new ProfileRepository().ChangeUsername(userName, email, connectionString);
        }

        public CouncilProfileModel GetCouncilProfile(int instance_id)
        {
            return new ProfileRepository().GetCouncilProfile(instance_id);
        }

    }
}
