using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using GM.DataAccess.Entities;
using GM.DataAccess.Interfaces;
using GM.Models;
using GM.Models.Public;
using GM.Utility;
using System.Text.RegularExpressions;

namespace GM.DataAccess
{
    public class ProfileRepository : IProfileRepository
    {
        public ProfileModel GetMyProfile(string userName)
        {
            return new ProfileModel();
        }

        public ProfileModelBase GetMyProfileBase(string providerUserKey, string email)
        {
            using (var context = new greenMoneyEntities())
            {
                // find from gm users
                var user = context.Users1.Find(new Guid(providerUserKey));

                var model = new ProfileModelBase
                {
                    User = new UserModel(),
                    GravatarHash = MD5.Hash(email),
                    TotalEarnings = user.Addresses.Transactions.Where(t => t.Points > 0).Sum(t => t.Points),
                    TotalRedemptions = -user.Addresses.Transactions.Where(t => t.Points < 0).Sum(t => t.Points),
                    CartTotal = user.CartItems.Sum(t => t.Quantity),
                    PointsAvailable = user.PointsAvailable,
                    Instance_Id = user.Instance_Id
                };

                //Number of challenges in last month
                var monthBeforeDate = DateTime.Today.AddDays(-30);
                var numOfChallenges = context.UserChallenges.Where(c => c.UserId == new Guid(providerUserKey)
                    && c.Issued > monthBeforeDate && c.PointsClaimed == false).Count();
                
                model.ChallengesTotal = numOfChallenges;

                Utils.CopyProperties(user, model.User);

                return model;
            }
        }

        public ProfileModel GetMyProfile(string providerUserKey, string email, int page = 1)
        {
            using (var context = new greenMoneyEntities())
            {
                // find from gm users
                var user = context.Users1.Find(new Guid(providerUserKey));

                const int pageSize = 20;

                var model = new ProfileModel
                {
                    User = new UserModel(),
                    Address = new AddressModel(),
                    GravatarHash = MD5.Hash(email),
                    TotalEarnings = user.Addresses.Transactions.Where(t => t.Points > 0).Sum(t => t.Points),
                    TotalRedemptions = -user.Addresses.Transactions.Where(t => t.Points < 0).Sum(t => t.Points),
                    CartTotal = user.CartTotal,
                    PointsAvailable = user.PointsAvailable,// GetPointsAvailable(user),
                    DolarPriceSum = GetDolarPriceSum(user.Vouchers),
                    Page = page,
                    NumPages = (int)Math.Ceiling((double)user.Addresses.Transactions.Count() / pageSize)
                };


                Utils.CopyProperties(user, model.User);
                Utils.CopyProperties(user.Addresses, model.Address);

                List<TransactionModel> list = new List<TransactionModel>();
                IEnumerable<Transactions> transactions = user.Addresses.Transactions.OrderByDescending(t => t.Time).Skip((page - 1) * pageSize).Take(pageSize);
                foreach (var item in transactions)
                {
                    TransactionModel m = new TransactionModel();
                    Utils.CopyProperties(item, m);

                    if (item.Users1 != null)
                    {
                        m.User = new UserModel();
                        Utils.CopyProperties(item.Users1, m.User);
                    }

                    list.Add(m);
                }
                model.Transactions = list;

                return model;
            }

        }

        private string GetDolarPriceSum(ICollection<Vouchers> vouchers)
        {
            int total = 0;
            try
            {
                foreach (var x in vouchers)
                {
                    if (x.Rewards.Name.Contains("$"))
                    {
                        var price = x.Rewards.Name.Split('$')[1];
                        var resultNumber = Regex.Match(price, @"\d+").Value;

                        int num;
                        bool res = int.TryParse(resultNumber, out num);
                        if (res == true)
                        {
                            total = total + num;
                        }
                    }
                    else if (x.Rewards.Name.Contains("%"))
                    {
                        total = total + 10;
                    }
                }
                return string.Format("${0}", total);
            }
            catch (Exception e)
            {
                return "$0";
            }
        }

        public bool UpdateProfile(string providerUserKey, UpdateProfileModel model)
        {
            try
            {
                using (var context = new greenMoneyEntities())
                {
                    // find from gm users
                    var user = context.Users1.Find(new Guid(providerUserKey));

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.PhoneNumber = model.PhoneNumber;
                    if (model.BirthDate != DateTime.MinValue)
                    {
                        user.DateOfBirth = model.BirthDate;
                    }
                    user.Sex = model.Sex;
                    user.SendEmailUpdates = model.SendEmailUpdates;
                    user.PushNotifications = model.PushNotifications;
                    user.PostToFacebook = model.PostToFacebook;

                    //Private company - auspost
                    user.EmploymentType = model.EmploymentType;
                    if (model.AddressId != null)
                    {
                        user.Address_Id = (int) model.AddressId;
                    }
                    // save changes
                    context.SaveChanges();

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


        public CartModel GetMyCart(string providerUserKey)
        {
             using (var context = new greenMoneyEntities())
                {
                    var user = context.Users1.Find(new Guid(providerUserKey));

                    var model = new CartModel
                    {
                        Items = user.CartItems.Select(i => new CartItemSummary
                        {
                            Id = i.Id,
                            RewardId = i.Rewards.Id,
                            Name = i.Rewards.PartnerName,
                            Description = i.Rewards.Name,
                            Quantity = i.Quantity,
                            Price = i.Rewards.Price,
                            DollarPrice = i.Rewards.DollarPrice,
                            ImageId = i.Rewards.ImageId
                        }).ToList(),
                        CartTotal = user.CartItems.Sum(i => i.Cost),
                        CartDollarTotal = user.CartItems.Sum(i => i.DollarCost),
                        //a reward is a voucher if it has a $0 price
                        HasVouchers = user.CartItems.Any(i => i.Rewards.DollarPrice == 0),
                        PointsAvailable = user.PointsAvailable
                    };

                    return model;
                }
 
        }

        public bool AddToMyCart(string providerUserKey, int rewardId, int quantity = 1)
        {
            try
            {
                using (var context = new greenMoneyEntities())
                {
                    var user = context.Users1.Find(new Guid(providerUserKey));

                    var cartItem = user.CartItems.SingleOrDefault(c => c.Rewards.Id == rewardId);

                    if (cartItem != null)
                    {
                        cartItem.Quantity += quantity;
                    }
                    else
                    {
                        cartItem = new CartItems
                        {
                            Rewards = context.Rewards.Single(r => r.Id == rewardId),
                            Users1 = user,
                            Quantity = quantity
                        };
                        context.CartItems.Add(cartItem);
                    }

                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool UpdateMyCart(string providerUserKey, IList<CartItemSummary> cartItems)
        {
            try
            {
                using (var context = new greenMoneyEntities())
                {
                    var user = context.Users1.Find(new Guid(providerUserKey));

                    foreach (var item in cartItems)
                    {
                        var cartItem = user.CartItems.Single(i => i.Id == item.Id);

                        cartItem.Quantity = item.Quantity;

                        if (cartItem.Quantity <= 0)
                        {
                            context.CartItems.Remove(cartItem);
                        }
                    }

                    context.SaveChanges();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public CheckoutModel GetMyCartCheckout(string providerUserKey)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.Find(new Guid(providerUserKey));

                var items = user.CartItems.Select(i => new WalletItemSummary
                {
                    Id = i.Id,
                    Name = i.Rewards.Name,
                    Description = i.Rewards.Description,
                    Quantity = i.Quantity,
                    Price = i.Rewards.Price,
                    DollarPrice = i.Rewards.DollarPrice,
                    ImageId = i.Rewards.ImageId,
                    DateAdded = i.Rewards.DateAdded,
                    ValidFrom = i.Rewards.AvailableFrom,
                    ValidTo = i.Rewards.AvailableTo
                });

                var model = new CheckoutModel
                {
                    Items = items.OrderByDescending(c=>c.DateExpired).ThenByDescending(c=>c.DateAdded),
                    CartTotal = user.CartItems.Sum(i => i.Cost),
                    CartDollarTotal = user.CartItems.Sum(i => i.DollarCost),
                    //a reward is a voucher if it has a $0 price
                    HasVouchers = user.CartItems.Any(i => i.Rewards.DollarPrice == 0)
                };

                return model;
            }
        }

        public CheckoutSubmitModel AddCartToMyWallet(string providerUserKey)
        {
            CheckoutSubmitModel model = new CheckoutSubmitModel();

            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.Find(new Guid(providerUserKey));


                var items = user.CartItems.ToList();
                long total = items.Sum(i => i.Cost);
                decimal dollartotal = items.Sum(i => i.DollarCost);


                // User don't have any items in cart
                if (!items.Any())
                {
                    model.CheckoutSubmitModelState = CheckoutSubmitModelState.NoItemsFound;
                    return model;
                }

                // User has enough points to purchase
                if (user.PointsTotal >= total)
                {
                    var time = DateTime.Now;

                    string description;
                    int? transactionType = null;

                    if (user.CartItems.Count == 1)
                    {
                        description = "Redeem " + user.CartItems.Single().Rewards.PartnerName 
                            + " " + user.CartItems.Single().Rewards.Name;
                        transactionType = 4;
                    }
                    else
                    {
                        description = "Voucher redemption x" + user.CartItems.Sum(i => i.Quantity);
                        transactionType = 1;
                    }

                    var purchases = new List<OrderSummaryItemModel>();

                    int costIndex = 1;

                    foreach (var item in items)
                    {
                        int itemidx;
                        if (item.DollarCost == 0)
                        {
                            for (int i = 0; i < item.Quantity; i++)
                            {
                                var voucher = new Vouchers
                                {
                                    Id = Guid.NewGuid(),
                                    Issued = time,
                                    Rewards = item.Rewards,
                                    Users1 = user
                                };

                                context.Vouchers.Add(voucher);

                                voucher.Rewards.Popularity += 1;

                                var ownerUser = context.Users.FirstOrDefault(x => x.UserId == item.Rewards.Owner_Id);

                                purchases.Add(new OrderSummaryItemModel
                                {
                                    VoucherId = voucher.Id,
                                    Name = item.Rewards.Name,
                                    Points = item.Cost,
                                    Quantity = item.Quantity,
                                    DollarCost = item.DollarCost,
                                    PayPalIndex = 0,
                                    AttachVoucherUrl = true,
                                    NotifyOnRedeem = item.Rewards.NotifyOnRedeem,
                                    UserFirstName = user.FirstName,
                                    UserLastName = user.LastName,
                                    PartnerName = item.Rewards.PartnerName,
                                    PartnerOwnerEmail = ownerUser != null ? ownerUser.UserName : null,
                                    Category = item.Rewards.RewardCategories.Name
                                });
                            }
                        }
                        else
                        {
                            itemidx = costIndex++;
                            var ownerUser = context.Users.FirstOrDefault(x => x.UserId == item.Rewards.Owner_Id);
                            
                            purchases.Add(new OrderSummaryItemModel
                            {
                                Name = item.Rewards.Name,
                                Points = item.Cost,
                                Quantity = item.Quantity,
                                DollarCost = item.DollarCost,
                                PayPalIndex = itemidx,
                                AttachVoucherUrl = false,
                                VoucherUrl = string.Empty,
                                NotifyOnRedeem = item.Rewards.NotifyOnRedeem,
                                UserFirstName = user.FirstName,
                                UserLastName = user.LastName,
                                PartnerName = item.Rewards.PartnerName,
                                PartnerOwnerEmail = ownerUser != null ? ownerUser.UserName : null,
                                Category = item.Rewards.RewardCategories.Name
                            });
                        }

                       // Remove from cart items after added to purchases
                       context.CartItems.Remove(item);
                    }

                    var transaction = new Transactions
                    {
                        Addresses = user.Addresses,
                        Users1 = user,
                        Time = time,
                        Description = description,
                        Points = -total,
                        TransactionTypeID = transactionType
                    };
                    context.Transactions.Add(transaction);
                    context.SaveChanges();


                    if (dollartotal > 0)
                    {
                        model.CheckoutSubmitModelState = CheckoutSubmitModelState.SuccessWithProductOrderConfirmation;
                    }
                    else
                    {
                        model.CheckoutSubmitModelState = CheckoutSubmitModelState.SuccessWithOrderConfirmation;
                    }

                    model.Purchases = purchases;

                    return model;

                }
                else // User does NOT have enough points to purchase
                {
                    model.CheckoutSubmitModelState = CheckoutSubmitModelState.NotEnoughPoints;
                    return model;
                }
            }
        }

        public CheckoutModel GetMyWallet(string providerUserKey)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.Find(new Guid(providerUserKey));

                var vaucerItems = user.Vouchers.Select(i => new WalletItemSummary
                {
                    VoucherId = i.Id,
                    Name = i.Rewards.PartnerName,
                    Description = i.Rewards.Name,
                    Price = i.Rewards.Price,
                    DollarPrice = i.Rewards.DollarPrice,
                    ImageId = i.Rewards.ImageId,
                    DateAdded = i.Issued,
                    DateExpired = i.Issued.AddDays(30),
                    ValidFrom = i.Rewards.ValidFrom,
                    ValidTo = i.Rewards.ValidTo,
                    Mobile = i.Rewards.Mobile
                }).ToList();

                //vaucerItems = vaucerItems.Where(v => v.ValidTo != null && v.DateAdded > DateTime.Now.AddDays(-30)).ToList();
                List<WalletItemSummary> vouchersToShow = vaucerItems.Where(v => v.DateExpired >= DateTime.Now).ToList();

                var model = new CheckoutModel
                {
                    Items = vouchersToShow.OrderBy(v=>v.DateExpired).ThenBy(v=>v.DateAdded),
                    CartTotal = user.CartItems.Sum(i => i.Cost),
                    CartDollarTotal = user.CartItems.Sum(i => i.DollarCost),
                    //a reward is a voucher if it has a $0 price
                    HasVouchers = user.CartItems.Any(i => i.Rewards.DollarPrice == 0)
                };

                return model;
            }
        }



        public bool ChangeUsername(string oldUsername, string newUsername, string connectionString)
        {
            if (oldUsername == null)
                throw new ArgumentNullException("oldUsername");
            if (newUsername == null)
                throw new ArgumentNullException("newUsername");

            if (oldUsername == newUsername)
                return false;


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
						SELECT COUNT(*)
						FROM Users
						WHERE UserName = @newUsername
					";

                    command.Parameters.AddWithValue("@newUsername", newUsername);

                    if ((int)command.ExecuteScalar() > 0)
                        return false;
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
						UPDATE Users
						SET UserName = @newUsername
						WHERE UserName = @oldUsername
					";

                    command.Parameters.AddWithValue("@oldUsername", oldUsername);
                    command.Parameters.AddWithValue("@newUsername", newUsername);

                    if (command.ExecuteNonQuery() <= 0)
                        throw new Exception("Unable to change username.");

                }
            }

            return true;

        }

        //for Council users, put in separate repository or...
        public CouncilProfileModel GetCouncilProfile(int instance_id)
        {
            using (var context = new greenMoneyEntities())
            {
                CouncilProfileModel councilModel = new CouncilProfileModel();

                var userChallengesCount = (from uc in context.UserChallenges.Include("Challenges")
                                           where uc.Challenges.Instance_Id == instance_id
                                           select uc).Count();
                councilModel.Instance_Id = instance_id;
                councilModel.UserChallengesCount = userChallengesCount;

                return councilModel;
            }
        }
    }
}