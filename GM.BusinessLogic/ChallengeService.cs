using System;
using System.Collections.Generic;
using System.Linq;
using GM.DataAccess;
using GM.Models.Public;

namespace GM.BusinessLogic
{
    public class ChallengesService
    {
        private readonly ChallengeRepository _challengeRepository;

        public ChallengesService()
        {
            _challengeRepository = new ChallengeRepository();
        }

        public ChallengeListModel GetChallenges(string userId, int instanceId, string sortBy, string category, string filter, int page, int pageSize)
        {
            var challenges = _challengeRepository.GetChallenges(userId, instanceId);

            if (pageSize == 0)
            {
                pageSize = 10;
            }

            //Filter all by text query
            if (!string.IsNullOrEmpty(filter))
                challenges =
                    challenges.Where(
                        r =>
                            (!string.IsNullOrEmpty(r.Name) && r.Name.ToLower().Contains(filter.ToLower()))|| (!string.IsNullOrEmpty(r.EarnAmount) && r.EarnAmount.ToLower().Contains(filter.ToLower()))
                            || (!string.IsNullOrEmpty(r.About) && r.About.ToLower().Contains(filter.ToLower()))
                            ||(!string.IsNullOrEmpty(r.Purpose) &&r.Purpose.ToLower().Contains(filter.ToLower()))).ToList();


            var selected = new List<ChallengeModel>();
            //Filter all by selected category
            if (category != null)
            {
                var selectedCategory = _challengeRepository.GetChallengeCategoryByName(category);
                if (selectedCategory == null)
                {
                    throw new Exception("challenge category not found");
                }

                selected = challenges.Where(r => r.ChallengeCategoryId.Equals(selectedCategory.Id)).ToList();
            }
            else
            {
                selected = challenges.ToList();
            }

            switch (sortBy)
            {
                case "newest":
                    selected = selected.OrderByDescending(r => r.DateAdded).ThenByDescending(r => r.Id).ToList();
                    break;
                case "oldest":
                    selected = selected.OrderBy(r => r.DateAdded).ThenBy(r => r.Id).ToList();
                    break;
                case "cheapest":
                    selected = selected.OrderBy(r => r.Points).ToList();
                    break;
                case "expensive":
                    selected = selected.OrderByDescending(r => r.Points).ToList();
                    break;
                case "popular":
                    selected = selected.OrderByDescending(r => r.Popularity).ToList();
                    break;
                default:
                    selected = selected.OrderByDescending(r => r.Popularity).ToList();
                    break;
            }

            //Include pagination
            var list = selected.Skip((page - 1) * pageSize).Take(pageSize).ToList();


            var model = new ChallengeListModel
            {
                ChallengesList = list,
                NumPages = (int)Math.Ceiling((double)selected.Count() / pageSize),
                NumRewards = selected.Count(),
                Page = page
            };

            return model;
        }

        public List<ChallengeCategory> GetChallengeCategories()
        {
            return _challengeRepository.GetChallengeCategories();
        }

        public ChallengeModel GetChallengeDetails(int id)
        {
            return _challengeRepository.GetChallengeDetails(id);
        }

        public CheckoutModel GetMyChallenges(string providerUserKey)
        {
            return _challengeRepository.GetMyChallengesWallet(providerUserKey);
        }

        public bool CheckIfInMyChallenges(string providerUserKey, int id)
        {
            return _challengeRepository.CheckIfInMyChallenges(providerUserKey, id);
        }

        public int GetNumberOfMyChallenges(string providerUserKey, DateTime startDate)
        {
            return _challengeRepository.GetNumberOfMyChallenges(providerUserKey, startDate);
        }

        public bool ClaimedPoint(string providerUserKey, int id)
        {
            return _challengeRepository.ClaimedPoints(providerUserKey, id);
        }

        public bool ClaimPoints(string providerUserKey, int id, bool claim = false)
        {
            return _challengeRepository.ClaimPoints(providerUserKey, id, claim);
        }

        //if the end date is null than the points should be automatically added
        public bool ClaimPointsForPledge(string providerUserKey, int id, bool claim = false)
        {
            var challenge = _challengeRepository.GetChallengeDetails(id);
            if (challenge != null)
            {
                if (challenge.EndDate == null)
                {
                    var claimed = _challengeRepository.ClaimPoints(providerUserKey, id, true);
                    if (claimed)
                        return _challengeRepository.AddPoints(providerUserKey, id);
                }
                return _challengeRepository.ClaimPoints(providerUserKey, id, claim);
            }
            return false;
        }

        public bool AddPoints(string providerUserKey, int id, string promoCode="")
        {
            return _challengeRepository.AddPoints(providerUserKey, id, promoCode);
        }

    }
}
