using System;
using System.Collections.Generic;
using GM.BusinessLogic.Interfaces;
using GM.DataAccess;
using GM.Models.Public;

namespace GM.BusinessLogic
{
    public class ChallengesAdminService
    {
        public List<ChallengeCategory> GetChallengeCategories(int instance_Id)
        {
            return new ChallengesAdminRepository().GetChallengeCategories(instance_Id);
        }

        public List<ChallengeAwardPointsOption> GetChallengeAwardPointsOptions()
        {
            return new ChallengesAdminRepository().GetChallengeAwardPointsOptions();
        }


        public List<ChallengeParticipationFrequency> GetChallengeParticipationFrequency()
        {
            return new ChallengesAdminRepository().GetChallengeParticipationFrequency();
        }

        public int? CreateChallenge(Guid providerUserKey, ChallengeModel model)
        {
            return new ChallengesAdminRepository().CreateChallenge(providerUserKey, model);
        }

        public int? UpdateChallenge(Guid providerUserKey, ChallengeModel model)
        {
            return new ChallengesAdminRepository().UpdateChallenge(providerUserKey, model);
        }

        public ChallengesListModel GetChallenges(Guid providerUserKey, int page, int pageSize, int orderBy, int? status, int? category, string search)
        {
            return new ChallengesAdminRepository().GetChallenges(providerUserKey, page, pageSize, orderBy, status, category, search);
        }

        public UserChallengeListModel GetUserChallenges(int id, int? orderBy, bool? pointsClaimed, string suburb, string search, int? timeRange, int page, int pageSize)
        {
            return new ChallengesAdminRepository().GetUserChallenges(id, orderBy, pointsClaimed, suburb, search, timeRange, page, pageSize);
        }

        public ChallengeModel GetChallengeDetails(int id)
        {
            return new ChallengesAdminRepository().GetChallengeDetails(id);
        }

        public ChallengeModel CopyChallenge(int id)
        {
            return new ChallengesAdminRepository().CopyChallenge(id);
        }

        public bool DeleteChallenge(int id)
        {
            return new ChallengesAdminRepository().DeleteChallenge(id);
        }

        public bool DeleteUserChallenge(int id)
        {
            return new ChallengesAdminRepository().DeleteUserChallenge(id);
        }

        public bool AddPoints(int userChallengeId)
        {
            return new ChallengesAdminRepository().AddPoints(userChallengeId);
        }

        public DateTime GetLastActivityDate(Guid id)
        {
            return new ChallengesAdminRepository().GetLastActivityDate(id);
        }

        public bool CheckInInstance(int challengeId, Guid userId)
        {
            return new ChallengesAdminRepository().CheckInInstance(challengeId, userId);
        }

        public UserChallengeModel GetUserChallenge(int id)
        {
            return new ChallengesAdminRepository().GetUserChallenge(id);
        }
    }
}
