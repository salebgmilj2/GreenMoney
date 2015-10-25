using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using GM.DataAccess.Entities;
using GM.Models.Public;
using GM.Models;

namespace GM.DataAccess
{
    public class ChallengeRepository
    {
        public List<ChallengeModel> GetChallenges(string userId, int instanceId)
        {
            using (var context = new greenMoneyEntities())
            {
                //used challenges that shouldnt be shown
                var myCompletedChallenges = (from uc in context.UserChallenges
                                             join c in context.Challenges
                                                 on uc.ChallengeId equals c.Id
                                             where uc.UserId == new Guid(userId)
                                                   && uc.PointsClaimed == true
                                                   &&
                                                   SqlFunctions.GetDate() <
                                                   (c.ParticipationFrequencyId == 1
                                                       ? SqlFunctions.DateAdd("YEAR", 100, uc.Issued)
                                                       : c.ParticipationFrequencyId == 2
                                                           ? SqlFunctions.DateAdd("DAY", 7, uc.Issued)
                                                           : c.ParticipationFrequencyId == 3
                                                               ? SqlFunctions.DateAdd("MONTH", 1, uc.Issued)
                                                               : SqlFunctions.DateAdd("DAY", -1, SqlFunctions.GetDate()))
                                             select c);

                // all challenges for instance id
                var query = (from c in context.Challenges
                             where (c.Instance.Id == instanceId)
                                   && (c.StartDate < SqlFunctions.GetDate() || c.StartDate == null)
                                   && (c.EndDate > SqlFunctions.GetDate() || c.EndDate == null)
                                   && (c.Display)
                             select c);

                var list = new List<ChallengeModel>();
                // iz rezultata drugog upita se izbacuje rezultat prvog i puni lista koja se dalje koristi za viewmodel. Ovde je sada kompletna lista challenga minus već korišćeni sa zabranom daljeg korišćenja.
                foreach (var challenge in query.Except(myCompletedChallenges).ToList())
                {
                    var model = new ChallengeModel();
                    Utils.CopyProperties(challenge, model);
                    var userChallenge =
                        context.UserChallenges.SingleOrDefault(
                            x => x.ChallengeId == challenge.Id && x.UserId == new Guid(userId));

                    var exists = CheckIfInMyChallenges(userId, challenge.Id);

                    model.ChallengeState = userChallenge != null && userChallenge.PointsClaimed.HasValue &&
                        userChallenge.PointsClaimed.Value ? ChallengeState.Completed :
                        exists ?
                        ChallengeState.Active : ChallengeState.NotAdded;
                    var firstOrDefault = context.ChallengeCategories.FirstOrDefault(x => x.Id == challenge.ChallengeCategoryId);
                    if (firstOrDefault !=
                        null)
                        model.ChallengeCategoryShortName =
                            firstOrDefault
                                .ShortName;
                    list.Add(model);
                }
                return list;
            }
        }

        public List<ChallengeCategory> GetChallengeCategories()
        {
            using (var context = new greenMoneyEntities())
            {
                var categories = context.ChallengeCategories
                    .OrderBy(c => c.Name);

                var list = new List<ChallengeCategory>();

                foreach (var category in categories)
                {
                    var model = new ChallengeCategory();
                    Utils.CopyProperties(category, model);
                    list.Add(model);
                }

                return list;
            }
        }

        public ChallengeCategory GetChallengeCategoryByName(string category)
        {
            using (var context = new greenMoneyEntities())
            {
                var selectedCategory = context.ChallengeCategories
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x.ShortName) && x.ShortName.ToLower().Equals(category.ToLower()));

                if (category != null)
                {
                    var model = new ChallengeCategory();
                    Utils.CopyProperties(selectedCategory, model);
                    return model;
                }

                return null;
            }
        }

        public ChallengeModel GetChallengeDetails(int id)
        {
            using (var context = new greenMoneyEntities())
            {
                var challenge = context.Challenges.SingleOrDefault(x => x.Id == id);
                var model = new ChallengeModel();
                if (challenge != null)
                    Utils.CopyProperties(challenge, model);

                return model;
            }
        }

        public CheckoutModel GetMyChallengesWallet(string providerUserKey)
        {
            using (var context = new greenMoneyEntities())
            {
                DateTime monthBeforeDate = DateTime.Today.AddDays(-30);

                var userChallenges = context.UserChallenges.Where(c => c.UserId == new Guid(providerUserKey)
                    && c.Issued > monthBeforeDate && c.PointsClaimed == false);


                List<WalletItemSummary> challenges = new List<WalletItemSummary>();
                foreach (var challenge in userChallenges)
                {
                    WalletItemSummary summaryModel = new WalletItemSummary
                    {
                        Id = challenge.ChallengeId.HasValue ? challenge.ChallengeId.Value : 0,
                        Name = challenge.Challenges.Name,
                        Description = challenge.Challenges.EarnAmount,
                        ChallengeCategoryShortName = challenge.Challenges.ChallengeCategories.ShortName,
                        CategoryImageId = challenge.Challenges.ChallengeCategories.ImageId,
                        ImageId = challenge.Challenges.LogoImageId,
                        DateAdded = challenge.Issued,
                        DateExpired = challenge.Challenges.EndDate,
                        PointsClaimed = challenge.PointsClaimed != null && (bool)challenge.PointsClaimed
                    };
                    challenges.Add(summaryModel);
                }

                var model = new CheckoutModel
                {
                    Items = challenges.OrderBy(c => c.DateExpired).ThenBy(c => c.DateAdded)
                };

                return model;
            }
        }

        public bool CheckIfInMyChallenges(string providerUserKey, int id)
        {
            try
            {
                using (var context = new greenMoneyEntities())
                {
                    var isInMyChallenges = context.UserChallenges.Any(c => c.UserId == new Guid(providerUserKey) && c.ChallengeId == id);
                    return isInMyChallenges;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool ClaimedPoints(string providerUserKey, int id)
        {
            try
            {
                using (var context = new greenMoneyEntities())
                {
                    var userChallenge = context.UserChallenges.SingleOrDefault(c => c.UserId == new Guid(providerUserKey) && c.ChallengeId == id);
                    if (userChallenge != null)
                    {
                        return userChallenge.PointsClaimed.HasValue && userChallenge.PointsClaimed.Value;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        //when user starts the process for getting points
        public bool ClaimPoints(string providerUserKey, int id, bool claim)
        {
            try
            {
                using (var context = new greenMoneyEntities())
                {
                    var userGuid = new Guid(providerUserKey);
                    var challenge =
                        context.UserChallenges.FirstOrDefault(
                            x => x.ChallengeId == id && x.UserId == userGuid);
                    if (challenge == null)
                    {
                        context.UserChallenges.Add(
                            new UserChallenges
                            {
                                ChallengeId = id,
                                UserId = userGuid,
                                PointsClaimed = claim,
                                Issued = DateTime.Now
                            });
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //add points to translactions
        public bool AddPoints(string providerUserKey, int id, string promoCode = "")
        {
            try
            {
                using (var context = new greenMoneyEntities())
                {
                    var user = context.Users1.Find(new Guid(providerUserKey));
                    var time = DateTime.Now;
                    var userChallenge =
                        context.UserChallenges.FirstOrDefault(
                            x => x.ChallengeId == id && x.UserId == user.Id);
                    if (userChallenge != null)
                    {
                        userChallenge.PointsClaimed = true;
                        var challenge = context.Challenges.SingleOrDefault(x => x.Id.Equals(id));
                        if (challenge != null)
                        {
                            //just for challenge action
                            if (string.IsNullOrEmpty(promoCode) || challenge.PromoCode.Equals(promoCode))
                            {
                                var addPoints = challenge.Points.HasValue ? Convert.ToInt64(challenge.Points.Value) : 0;
                                var transaction = new Transactions
                                {
                                    Addresses = user.Addresses,
                                    Users1 = user,
                                    Time = time,
                                    Description = "Challenge " + challenge.Name,
                                    Points = +addPoints,
                                    TransactionTypeID = 2
                                };
                                context.Transactions.Add(transaction);
                                context.SaveChanges();
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public int GetNumberOfMyChallenges(string providerUserKey, DateTime startDate)
        {
            using (var context = new greenMoneyEntities())
            {
                //TODO check this condition
                var numOfChallenges = context.UserChallenges.Where(c => c.UserId == new Guid(providerUserKey)
                    && c.Issued > startDate).Count();

                return numOfChallenges;
            }
        }

    }
}
