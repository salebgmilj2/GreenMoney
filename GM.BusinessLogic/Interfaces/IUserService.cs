using System.Collections.Generic;
using System.Linq;
using GM.Models.Public;

namespace GM.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        List<string> GetAllUsers();

        UserModel CreateUser(UserModel model, int numPoints, string description, int? transactionTypeId);

        UserModel GetFacebookUser(long userId);

        InstanceModel GetInstanceForPostCode(string postcode);

        bool UserIsInInterestedPeople(string email);

        List<string> GetAllStreetTypes();
        List<string> GetAllSuburbs();

        AddressModel FindMatchingAdressModel(string unitNumber, string streetNumber, string streetName, string streetType,
            string suburb, string postcode);

    }
}
