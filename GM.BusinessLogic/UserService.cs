using System.Collections.Generic;
using System.Linq;
using GM.BusinessLogic.Interfaces;
using GM.DataAccess;
using GM.Models.Public;
using System;

namespace GM.BusinessLogic
{
    public class UserService : IUserService
    {
        public List<string> GetAllUsers()
        {
            //var users = GM.DataAccess.Entities.Users.
            var test = new UserRepository().Users();

            return new List<string>();
        }

        public InstanceModel GetInstanceForPostCode(string postcode)
        {
            return new UserRepository().GetInstanceForPostCode(postcode);
        }

        public bool UserIsInInterestedPeople(string email)
        {
            return new UserRepository().UserIsInInterestedPeople(email);
        }

        public UserModel AddUserToInInterestedPeople(UserModel userModel)
        {
            return new UserRepository().AddUserToInInterestedPeople(userModel);
        }

        public List<InstanceModel> GetAllInstances()
        {
            return new UserRepository().GetAllInstances();
        }

        public List<string> GetAllSuburbs()
        {
            return new UserRepository().GetAllSuburbs();
        }

        public List<string> GetAllStreetTypes()
        {
            return new UserRepository().GetAllStreetTypes();
        }

        public List<string> GetAllStreetTypes(int instanceId, bool toExclude = false)
        {
            return new UserRepository().GetAllStreetTypes(instanceId, toExclude);
        }

        public List<string> GetAllStreetNames(int instanceId)
        {
            return new UserRepository().GetAllStreetNames(instanceId);
        }

        public List<string> GetAllSuburbs(int instanceId, bool toExclude = false)
        {
            return new UserRepository().GetAllSuburbs(instanceId, toExclude);
        }

        public List<string> GetAllStates(int instanceId)
        {
            return new UserRepository().GetAllStates(instanceId);
        }

        public AddressModel FindMatchingAdressModel(string unitNumber, string streetNumber, string streetName, string streetType, string suburb, string postcode)
        {
            return new UserRepository().FindMatchingAdressModel(unitNumber, streetNumber, streetName, streetType, suburb, postcode);
        }

        public int? FindMatchingAuspostAddressId(string streetName, string streetType, string suburb, string state)
        {
            return new UserRepository().FindMatchingAuspostAddressId(streetName, streetType, suburb, state);
        }

        public bool CheckAddressHasUsersRegistered(string unitNumber, string streetNumber, string streetName, string streetType, string suburb, string postcode)
        {
            return new UserRepository().CheckAddressHasUsersRegistered(unitNumber, streetNumber, streetName, streetType, suburb, postcode);

        }

        public UserModel CreateUser(UserModel model, int numPoints, string description, int? transactionTypeId)
        {
            return new UserRepository().CreateUser(model, numPoints, description, transactionTypeId);

        }

        public UserModel GetUserById(Guid id)
        {
            return new UserRepository().GetUserById(id);
        }

        public UserModel CreateSupplierUser(UserModel model, int numPoints, string description, int? transactionTypeId)
        {
            return new UserRepository().CreateSupplierUser(model, numPoints, description, transactionTypeId);
        }

        public UserModel UpdateSupplierUser(UserModel model)
        {
            return new UserRepository().UpdateSupplierUser(model);
        }

        public bool UpdateToCompleteSupplierUser(UserModel model)
        {
            return new UserRepository().UpdateToCompleteSupplierUser(model);
        }

        public bool FoundOriginalUserForAddressById(int addressIdInt)
        {
            return new UserRepository().FoundOriginalUserForAddressById(addressIdInt);

        }

        public UserModel GetFacebookUser(long userId)
        {
            return new UserRepository().GetFacebookUser(userId);
        }

        public bool DeleteUser(Guid userId, string connectionString)
        {
            return new UserRepository().DeleteUser(userId, connectionString);

        }

        public InviteUserModel InviteHouseholdMembers(InviteUserModel userModel, int numBonusPoints, string descBonusPoints, int? transactionTypeId)
        {
            return new UserRepository().InviteHouseholdMembers(userModel, numBonusPoints, descBonusPoints, transactionTypeId);
        }

        public AddressModel GetAddressById(int addressId)
        {
            return new UserRepository().GetAddressById(addressId);
        }

        public List<UserModel> GetAdditionalAccounts(string p)
        {
            return new UserRepository().GetAdditionalAccounts(p);
        }

        public List<UserModel> GetInvitedUsersAccounts(string p)
        {
            return new UserRepository().GetInvitedUsersAccounts(p);

        }

        public string GetUserNameHolderForAddress(int addressId)
        {
            return new UserRepository().GetUserNameHolderForAddress(addressId);
        }

        public bool InvitationAcceptedAddBonusPoints(string inviterEmail, int numBonusPoints, string descBonusPoints, int? transactionTypeId)
        {
            return new UserRepository().InvitationAcceptedAddBonusPoints(inviterEmail, numBonusPoints, descBonusPoints, transactionTypeId);
        }

        public bool AddRewards4Images(string connString)
        {
            return new UserRepository().AddRewards4Images(connString);
        }

        public List<string> GetEmailsForRoleInInstance(int instanceId, string role)
        {
            return new UserRepository().GetEmailsForRoleInInstance(instanceId, role);
        }

        public List<string> GetAllSuburbsForInstance(int instanceId)
        {
            return new UserRepository().GetAllSuburbsForInstance(instanceId);
        }
        public bool AlterChallenges(string connString)
        {
            return new UserRepository().AlterChallenges(connString);
        }
    }
}
