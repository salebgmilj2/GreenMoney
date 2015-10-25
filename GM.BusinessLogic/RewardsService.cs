using System;
using System.Collections.Generic;
using GM.BusinessLogic.Interfaces;
using GM.DataAccess;
using GM.Models.Public;

namespace GM.BusinessLogic
{
    public class RewardsService : IRewardsService
    {

        public RewardsListModel GetRewards(int? instanceId, int? excludeInstanceId, string sortBy, string category, string filter, int page, int pageSize)
        {
            return new RewardsRepository().GetRewards(instanceId, excludeInstanceId, sortBy, category, filter, page, pageSize);
        }

        public List<RewardCategory> GetRewardCategories()
        {
            return new RewardsRepository().GetRewardCategories();
        }

        public RewardModel GetRewardDetails(int id)
        {
            return new RewardsRepository().GetRewardDetails(id);
        }

        public VoucherModel GetVoucher(Guid id)
        {
            return new RewardsRepository().GetVoucher(id);
        }

        public RewardsListModel GetSupplierRewards(Guid providerUserKey, int page, int pageSize)
        {
            return new RewardsRepository().GetSupplierRewards(providerUserKey, page, pageSize);
        }

        public int? CreateReward(Guid providerUserKey, RewardModel model)
        {
            return new RewardsRepository().CreateReward(providerUserKey, model);
        }

        public int? UpdateReward(Guid providerUserKey, RewardModel model)
        {
            return new RewardsRepository().UpdateReward(providerUserKey, model);
        }

        public Tuple<int, List<RedemptionModel>> GetRedemptions(int rewardId, DateTime startDate, DateTime endDate, int page, int pageSize, string filter)
        {
            return new RewardsRepository().GetRedemptions(rewardId, startDate, endDate, page, pageSize, filter);
        }

        public RewardsSummaryListModel GetSupplierRewardsSummary(Guid providerUserKey, int page, int pageSize)
        {
            return new RewardsRepository().GetSupplierRewardsSummary(providerUserKey, page, pageSize);

        }

        public RedemptionsStatisticsListModel GetRedemptionsStatistics(string providerUserKey, int page, int pageSize)
        {
            return new RewardsRepository().GetRedemptionsStatistics(providerUserKey, page, pageSize);
        }

        public ActivitySummaryModel GetActivitySummaryModelModel(string providerUserKey)
        {
            return new RewardsRepository().GetActivitySummaryModelModel(providerUserKey);
        }

        public bool DeleteReward(int id)
        {
            return new RewardsRepository().DeleteReward(id);
        }
    }
}
