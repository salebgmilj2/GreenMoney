using System.Collections.Generic;
using GM.Models.Public;

namespace GM.BusinessLogic.Interfaces
{
    public interface IRewardsService
    {
        RewardsListModel GetRewards(int? instanceId, int? excludeInstanceId, string sortBy, string category, string filter, int page, int pageSize);
        RewardModel GetRewardDetails(int id);

        List<RewardCategory> GetRewardCategories();
    }
}
