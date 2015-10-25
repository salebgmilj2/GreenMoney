using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using GM.DataAccess.Entities;
using GM.Models;
using GM.Models.Public;

namespace GM.DataAccess
{
    public class UserRepository
    {

        public bool Users()
        {
            using (var context = new greenMoneyEntities())
            {
                var users = context.Users;
            }

            return true;
        }

        public UserModel GetUserById(Guid id)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.FirstOrDefault(i => i.Id == id);

                if (user == null)
                {
                    return null;
                }

                var model = new UserModel
                {
                    AddressModel = new AddressModel()
                };

                Utils.CopyProperties(user, model);
                Utils.CopyProperties(user.Addresses, model.AddressModel);

                return model;
            }
        }

        public UserModel CreateUser(UserModel model, int numPoints, string description, int? transactionTypeId)
        {
            using (var context = new greenMoneyEntities())
            {
                var address = context.Addresses.First(i => i.Id == model.AddressId);
                var user = new Users1
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Addresses = address,
                    SendEmailOffers = model.SendEmailOffers,
                    SendEmailUpdates = model.SendEmailUpdates,
                    IsFBAccount = model.IsFBAccount,
                    FBUserId = model.FBUserId,
                    IsAdditionalAccountHolder = model.IsAdditionalAccountHolder,
                    Sex = model.Sex,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber
                };

                if (address.Instance_Id != null)
                    user.Instance_Id = (int)address.Instance_Id;

                // store it
                context.Users1.Add(user);

                AddPoints(context, user, numPoints, description, transactionTypeId);

                context.SaveChanges();

                return GetUserById(user.Id);
            }
        }

        public UserModel CreateSupplierUser(UserModel model, int numPoints, string description, int? transactionTypeId)
        {
            using (var context = new greenMoneyEntities())
            {
                var address = context.Addresses.First(i => i.Id == model.AddressId);
                var user = new Users1
                {
                    Id = model.Id,
                    Instance_Id = model.Instance_Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Addresses = address,
                    SendEmailUpdates = model.SendEmailUpdates,
                    IsAdditionalAccountHolder = model.IsAdditionalAccountHolder,
                    BusinessName = model.BusinessName,
                    BusinessNumberABN = model.BusinessNumberABN,
                    BusinessType = model.BusinessType,
                    BussinesWebSite = model.BussinesWebSite,
                    BussinesEmail = model.BussinesEmail,
                    BussinesPhone = model.BussinesPhone,
                    BussinesPhoneArea = model.BussinesPhoneArea,
                    BussinesPhoneMobile = model.BussinesPhoneMobile,
                    BussinesLocation = model.BussinesLocation,
                    EmailBussinesOnVoucherRedeem = model.EmailBussinesOnVoucherRedeem
                };

                // store it
                context.Users1.Add(user);

                AddPoints(context, user, numPoints, description, transactionTypeId);

                context.SaveChanges();

                return GetUserById(user.Id);
            }
        }

        public UserModel UpdateSupplierUser(UserModel model)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.Find(model.Id);


                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.SendEmailOffers = model.SendEmailOffers;
                user.SendEmailUpdates = model.SendEmailUpdates;
                user.IsFBAccount = model.IsFBAccount;
                user.BusinessName = model.BusinessName;
                user.BusinessNumberABN = model.BusinessNumberABN;
                user.BusinessType = model.BusinessType;
                user.BussinesWebSite = model.BussinesWebSite;
                user.BussinesEmail = model.BussinesEmail;
                user.BussinesPhone = model.BussinesPhone;
                user.BussinesPhoneArea = model.BussinesPhoneArea;
                user.BussinesPhoneMobile = model.BussinesPhoneMobile;
                user.BussinesLocation = model.BussinesLocation;
                user.EmailBussinesOnVoucherRedeem = model.EmailBussinesOnVoucherRedeem;

                context.SaveChanges();

                return GetUserById(user.Id);
            }
        }

        public bool UpdateToCompleteSupplierUser(UserModel model)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.Find(model.Id);

                user.BusinessNumberABN = model.BusinessNumberABN;
                user.BusinessType = model.BusinessType;
                user.BussinesWebSite = model.BussinesWebSite;

                context.SaveChanges();

                return true;
            }
        }
        private void AddPoints(greenMoneyEntities context, Users1 user, int numPoints, string description, int? transactionType)
        {

            var transaction = new Transactions
            {
                Addresses = user.Addresses,
                Users1 = user,
                Time = DateTime.Now,
                Description = description,
                Points = numPoints,
                TransactionTypeID = transactionType
            };

            context.Transactions.Add(transaction);
        }

        public InstanceModel GetInstanceForPostCode(string postcode)
        {
            InstanceModel model = new InstanceModel();
            using (var context = new greenMoneyEntities())
            {
                var postcodes = context.Addresses.Select(i => i.Postcode).Distinct();
                if (postcodes.Contains(postcode))
                {
                    Addresses addressFirst = context.Addresses.FirstOrDefault(x => x.Postcode == postcode);
                    if (addressFirst != null)
                    {
                        var instance = addressFirst.Instance;
                        Utils.CopyProperties(instance, model);

                        return model;
                    }
                }
            }
            return null;
        }

        public bool UserIsInInterestedPeople(string email)
        {
            using (var context = new greenMoneyEntities())
            {
                var isInInterestedPeople = context.InterestedPeople.Any(p => p.Email.ToLower() == email);
                return isInInterestedPeople;
            }
        }

        public UserModel AddUserToInInterestedPeople(UserModel userModel)
        {
            using (var context = new greenMoneyEntities())
            {
                var ip = new InterestedPeople
                {
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    SendEmailOffers = false,//registrationViewModel.SendEmailOffers, //Redesing
                    SendEmailUpdates = userModel.SendEmailOffers,
                    Email = userModel.Email,
                    Postcode = userModel.Postcode,
                    Gender = null, //model.Sex, //Redesing
                    DateOfBirth = userModel.DateOfBirth,
                    FreeStandingHome = true,
                    HasRfidTag = false,
                    StreetName = "N/A",
                    StreetNumber = "N/A",
                    StreetType = "N/A",
                    State = "N/A",
                    Suburb = "N/A",
                    UnitNumber = "N/A"
                };

                context.InterestedPeople.Add(ip);
                context.SaveChanges();

                return userModel;
            }
        }

        public List<InstanceModel> GetAllInstances()
        {
            var list = new List<InstanceModel>();
            using (var context = new greenMoneyEntities())
            {
                var instances = context.Instance.ToList();
                list.AddRange(instances.Select(instance => new InstanceModel { Id = instance.Id, Name = instance.Name }));

                return list;
            }
        }

        public List<string> GetAllSuburbs()
        {
            using (var context = new greenMoneyEntities())
            {
                var list = context.Addresses.Select(a => a.Suburb).Distinct().ToList();

                IEnumerable<string> sortAscendingQuery =
                   from x in list
                   orderby x
                   select x;

                return sortAscendingQuery.ToList();
            }
        }

        public List<string> GetAllStreetTypes()
        {
            using (var context = new greenMoneyEntities())
            {
                var list = context.Addresses.Select(a => a.StreetType).Distinct().ToList();
                IEnumerable<string> sortAscendingQuery =
                   from x in list
                   orderby x
                   select x;

                return sortAscendingQuery.ToList();
            }
        }

        public List<string> GetAllStreetTypes(int instanceId, bool toExclude = false)
        {
            using (var context = new greenMoneyEntities())
            {
                List<string> list = new List<string>();

                if (toExclude)
                {
                    list = context.Addresses.Where(a => a.Instance_Id != instanceId).Select(a => a.StreetType).Distinct().ToList();
                }
                else
                {
                    list = context.Addresses.Where(a => a.Instance_Id == instanceId).Select(a => a.StreetType).Distinct().ToList();
                }

                IEnumerable<string> sortAscendingQuery =
                   from x in list
                   orderby x
                   select x;

                return sortAscendingQuery.ToList();
            }
        }

        public List<string> GetAllStreetNames(int instanceId)
        {
            using (var context = new greenMoneyEntities())
            {
                var list = context.Addresses.Where(a => a.Instance_Id == instanceId)
                    .Select(a => a.StreetName).Distinct().ToList();

                return list;
            }
        }

        public List<string> GetAllSuburbs(int instanceId, bool toExclude = false)
        {
            using (var context = new greenMoneyEntities())
            {
                List<string> list = new List<string>();

                if (toExclude)
                {
                    list = context.Addresses.Where(a => a.Instance_Id != instanceId).Select(a => a.Suburb).Distinct().ToList();
                }
                else
                {
                    list = context.Addresses.Where(a => a.Instance_Id == instanceId).Select(a => a.Suburb).Distinct().ToList();
                }

                IEnumerable<string> sortAscendingQuery =
                   from x in list
                   orderby x
                   select x;

                return sortAscendingQuery.ToList();
            }
        }

        public List<string> GetAllStates(int instanceId)
        {
            using (var context = new greenMoneyEntities())
            {
                var list = context.Addresses.Where(a => a.Instance_Id == instanceId)
                    .Select(a => a.State).Distinct().ToList();

                return list;
            }
        }

        public AddressModel GetAddressById(int addressId)
        {
            using (var context = new greenMoneyEntities())
            {
                var address = context.Addresses.FirstOrDefault(x => x.Id == addressId);
                if (address != null)
                {
                    var model = new AddressModel();
                    Utils.CopyProperties(address, model);

                    return model;
                }
            }
            return null;
        }

        public bool CheckAddressHasUsersRegistered(string unitNumber, string streetNumber, string streetName, string streetType, string suburb, string postcode)
        {
            var address = FindMatchingAdress(unitNumber, streetNumber, streetName, streetType, suburb, postcode);

            using (var context = new greenMoneyEntities())
            {
                var addressFound = context.Addresses.FirstOrDefault(a => a.Id == address.Id);
                var hasUsers = addressFound != null && addressFound.Users1.Any();

                return hasUsers;
            }
        }

        public AddressModel FindMatchingAdressModel(string unitNumber, string streetNumber, string streetName,
            string streetType, string suburb, string postcode = null)
        {
            var address = FindMatchingAdress(unitNumber, streetNumber, streetName, streetType, suburb, postcode);

            if (address != null)
            {
                AddressModel model = new AddressModel();
                Utils.CopyProperties(address, model);

                return model;
            }
            return null;
        }

        public int? FindMatchingAuspostAddressId(string streetName, string streetType, string suburb, string state)
        {
            using (var context = new greenMoneyEntities())
            {
                var addresses = context.Addresses;
                if (streetName == null)
                    throw new ArgumentNullException("streetName");
                if (suburb == null)
                    throw new ArgumentNullException("suburb");
                if (state  == null)
                    throw new ArgumentNullException("state");

                var address = addresses
                    .Where(a => streetName.Equals(a.StreetName, StringComparison.OrdinalIgnoreCase))
                    .Where(a => suburb.Equals(a.Suburb, StringComparison.OrdinalIgnoreCase))
                    .Where(a => state.Equals(a.State, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault(a => streetType.Equals(a.StreetType, StringComparison.OrdinalIgnoreCase));

                if (address != null)
                {
                    return address.Id;
                }
                else
                {
                    return null;
                }
            }
        }

        private Addresses FindMatchingAdress(string unitNumber, string streetNumber, string streetName,
            string streetType, string suburb, string postcode = null)
        {
            using (var context = new greenMoneyEntities())
            {
                var addresses = context.Addresses;
                if (streetNumber == null)
                    throw new ArgumentNullException("streetNumber");
                if (streetName == null)
                    throw new ArgumentNullException("streetName");
                if (suburb == null)
                    throw new ArgumentNullException("suburb");

                var candidates = addresses
                    .Where(a => streetName.Equals(a.StreetName, StringComparison.OrdinalIgnoreCase))
                    .Where(a => suburb.Equals(a.Suburb, StringComparison.OrdinalIgnoreCase));

                if (unitNumber != null)
                    candidates =
                        candidates.Where(a => unitNumber.Equals(a.UnitNumber, StringComparison.OrdinalIgnoreCase));
                else
                    candidates = candidates.Where(a => a.UnitNumber == null);

                if (streetType != null)
                    candidates =
                        candidates.Where(a => streetType.Equals(a.StreetType, StringComparison.OrdinalIgnoreCase));
                else
                    candidates = candidates.Where(a => a.StreetType == null);

                if (postcode != null)
                    candidates = candidates.Where(a => postcode.Equals(a.Postcode, StringComparison.OrdinalIgnoreCase));

                foreach (var candidate in candidates)
                {
                    var x = StreetNumberRange(streetNumber);
                    var y = StreetNumberRange(candidate.StreetNumber);

                    if (x != null && y != null)
                    {
                        if (!x.Except(y).Any()) // ie. x is not a subset of y
                        {

                            return candidate; // select first suitable candidate

                        }
                    }
                    else
                    {
                        if (string.Equals(streetNumber, candidate.StreetNumber, StringComparison.OrdinalIgnoreCase))
                        {

                            return candidate;
                        }
                    }
                }

                return null;
            }
        }

        private static IEnumerable<int> StreetNumberRange(string streetNumber)
        {
            var match = Regex.Match(streetNumber, @"^(\d+)-+(\d+)$");
            if (match.Success)
            {
                var start = int.Parse(match.Groups[1].Value);
                var end = int.Parse(match.Groups[2].Value);
                return Enumerable.Range(start, end - start);
            }
            else
            {
                return null;
            }
        }


        public bool FoundOriginalUserForAddressById(int addressIdInt)
        {
            using (var context = new greenMoneyEntities())
            {
                var inviterUser = context.Users1.FirstOrDefault(
                    x => x.Addresses.Id == addressIdInt && x.IsAdditionalAccountHolder == false);

                return inviterUser != null;
            }
        }

        public UserModel GetFacebookUser(long userId)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.SingleOrDefault(a => a.IsFBAccount && a.FBUserId == userId);

                if (user == null)
                {
                    return null;
                }

                UserModel model = new UserModel();
                Utils.CopyProperties(user, model);

                model.Email = user.Users.UserName;

                return model;
            }
        }

        public bool DeleteUser(Guid userId, string connectionString)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.SingleOrDefault(a => a.Id == userId);

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
						delete  
						FROM gm.Transactions
						WHERE User_Id = @newUsername
					";

                        command.Parameters.AddWithValue("@newUsername", userId.ToString());

                        var t = command.ExecuteNonQuery();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
						delete  
						FROM dbo.Memberships
						WHERE UserId = @newUsername
					";

                        command.Parameters.AddWithValue("@newUsername", userId.ToString());

                        var t = command.ExecuteNonQuery();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
						delete  
						FROM dbo.Users
						WHERE UserId = @newUsername
					";

                        command.Parameters.AddWithValue("@newUsername", userId.ToString());

                        var t = command.ExecuteNonQuery();
                    }

                    connection.Close();

                }

                return true;
            }
        }

        public InviteUserModel InviteHouseholdMembers(InviteUserModel userModel, int numBonusPoints, string descBonusPoints, int? transactionTypeId)
        {
            using (var context = new greenMoneyEntities())
            {
                Users1 inviterUser = context.Users1.FirstOrDefault(x => x.Id == userModel.User_Id);
                var user = new InvitedUsers
                {
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    Email = userModel.Email,
                    User_Id = userModel.User_Id
                };

                context.InvitedUsers.Add(user);
                AddPoints(context, inviterUser, numBonusPoints, descBonusPoints, transactionTypeId);

                context.SaveChanges();

                return userModel;
            }
        }

        public List<UserModel> GetAdditionalAccounts(string p)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.FirstOrDefault(x => x.Id == new Guid(p));
                var additionalAccountHolders =
                   context.Users1.Where(x => x.Addresses.Id == user.Addresses.Id && x.IsAdditionalAccountHolder == true)
                        .ToList();

                List<UserModel> users = new List<UserModel>();
                foreach (var item in additionalAccountHolders)
                {
                    UserModel m = new UserModel();
                    Utils.CopyProperties(item, m);

                    m.Email = item.Users.UserName;
                    users.Add(m);
                }

                return users;

                //What is this??
                //model.IsAdditionalThemselves = user.IsAdditionalAccountHolder;
            }
        }

        public List<UserModel> GetInvitedUsersAccounts(string inviterId)
        {
            using (var context = new greenMoneyEntities())
            {
                var invitedAccounts = context.InvitedUsers.Where(x => x.User_Id == new Guid(inviterId)).ToList();

                var users = new List<UserModel>();
                foreach (var item in invitedAccounts)
                {
                    var m = new UserModel
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = item.Email
                    };

                    users.Add(m);
                }

                return users;
            }
        }

        public string GetUserNameHolderForAddress(int addressId)
        {
            using (var context = new greenMoneyEntities())
            {
                var inviterUser = context.Users1.FirstOrDefault(
                    x => x.Addresses.Id == addressId && x.IsAdditionalAccountHolder == false);

                if (inviterUser != null)
                    return inviterUser.Users.UserName;
            }

            return null;
        }

        public bool InvitationAcceptedAddBonusPoints(string inviterEmail, int numPoints, string description, int? transactionTypeId)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users.FirstOrDefault(i => i.UserName == inviterEmail);

                if (user != null)
                {
                    AddPoints(context, user.Users1, numPoints, description, transactionTypeId);
                    context.SaveChanges();
                    return true;

                }

                return false;
            }
        }

        public bool AddRewards4Images(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
						ALTER TABLE [greenMoney2].[gm].[Rewards]
                        ADD  
                        Image1Id int NULL,
                         Image2Id int NULL,
                         Image3Id int NULL,
                         Image4Id int NULL
					";

                    var t = command.ExecuteNonQuery();
                }

                connection.Close();

            }

            return true;
        }

        public List<string> GetEmailsForRoleInInstance(int instanceId, string role)
        {
            using (var context = new greenMoneyEntities())
            {
                var userEmails = from usr in context.Users
                                 from rol in usr.Roles
                                 from usr1 in context.Users1
                                 where usr.UserId == usr1.Id
                                 where rol.RoleName == role
                                 where usr1.Instance_Id == instanceId
                                 select new
                                 {
                                     Email = usr.UserName
                                 };

                List<string> tmpStrings = new List<string>();
                foreach (var userEmail in userEmails)
                {
                    tmpStrings.Add(userEmail.Email);
                }

                return tmpStrings;
            }
        }

        public List<string> GetAllSuburbsForInstance(int instanceId)
        {
            using (var context = new greenMoneyEntities())
            {
                var list = from a in context.Addresses
                           where a.Instance_Id == instanceId
                           select a.Suburb;

                return list.Distinct().ToList();
            }
        }

        public bool AlterChallenges(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
						 ALTER TABLE [GreenMoney2].[gm].[Challenges]
                         ALTER COLUMN  Article nvarchar(max) NULL;
                         ALTER TABLE [GreenMoney2].[gm].[Challenges]
                         ALTER COLUMN Purpose nvarchar(max) NULL;
					";

                    var t = command.ExecuteNonQuery();
                }

                connection.Close();

            }

            return true;
        }

    }
}
