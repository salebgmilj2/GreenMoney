using System.Collections.Generic;
using GM.Models.Public;

namespace GM.DataAccess.Interfaces
{
    public interface IRewardsRepository
    {
        RewardsListModel GetRewards(int? instanceId, int? excludeInstanceId, string sortBy, string category, string filter, int page, int pageSize);
        RewardModel GetRewardDetails(int id);

        List<RewardCategory> GetRewardCategories();
    }
}
