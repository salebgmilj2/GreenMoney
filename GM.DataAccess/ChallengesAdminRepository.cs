using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Data.Objects.SqlClient;
using System.Security.Cryptography;
using GM.DataAccess.Entities;
using GM.DataAccess.Interfaces;
using GM.Models;
using GM.Models.Public;



namespace GM.DataAccess
{
    public class ChallengesAdminRepository
    {
        public List<ChallengeCategory> GetChallengeCategories(int instance_Id)
        {
            using (var context = new greenMoneyEntities())
            {
                var categories = context.ChallengeCategories
                    .OrderBy(c => c.Name);


                List<ChallengeCategory> list = new List<ChallengeCategory>();
                foreach (var category in categories)
                {

                    ChallengeCategory model = new ChallengeCategory();
                    Utils.CopyProperties(category, model);

                    //if ((instance_Id != 5) || (instance_Id == 5 && category.Id != 4))
                    //for all instances Invite neighbour is not shown
                    if (category.Id != 4)
                    {
                        list.Add(model);
                    }
                }

                return list;
            }
        }

        public List<ChallengeAwardPointsOption> GetChallengeAwardPointsOptions()
        {
            using (var context = new greenMoneyEntities())
            {
                var options = context.AwardPointsOptions
                    .OrderBy(c => c.Name);


                List<ChallengeAwardPointsOption> list = new List<ChallengeAwardPointsOption>();
                foreach (var option in options)
                {
                    ChallengeAwardPointsOption model = new ChallengeAwardPointsOption();
                    Utils.CopyProperties(option, model);

                    list.Add(model);
                }

                return list;
            }
        }

        public List<ChallengeParticipationFrequency> GetChallengeParticipationFrequency()
        {
            using (var context = new greenMoneyEntities())
            {
                var frequencies = context.ParticipationFrequency
                    .OrderBy(c => c.Name);


                List<ChallengeParticipationFrequency> list = new List<ChallengeParticipationFrequency>();
                foreach (var frequency in frequencies)
                {
                    ChallengeParticipationFrequency model = new ChallengeParticipationFrequency();
                    Utils.CopyProperties(frequency, model);

                    list.Add(model);
                }

                return list;
            }
        }


        //challenge
        public int? CreateChallenge(Guid providerUserKey, ChallengeModel model)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.FirstOrDefault(u => u.Id == providerUserKey);
                if (user != null)
                {
                    Challenges challenge = new Challenges();


                    Utils.CopyProperties(model, challenge);

                    challenge.Owner_Id = user.Id;
                    challenge.Instance_Id = user.Instance_Id;

                    challenge.DateAdded = DateTime.Now;
                    challenge.Popularity = 0;
                    //  challenge.State = model.AustraliaState;

                    if (model.ProfileImages.Count > 0)
                    {
                        if (model.ProfileImages[0] != null && model.ProfileImages[0].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[0]);
                            challenge.ImageId1 = upload.UploadId;
                        }

                        if (model.ProfileImages[1] != null && model.ProfileImages[1].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[1]);
                            challenge.ImageId2 = upload.UploadId;
                        }

                        if (model.ProfileImages[2] != null && model.ProfileImages[2].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[2]);
                            challenge.ImageId3 = upload.UploadId;
                        }

                        if (model.ProfileImages[3] != null && model.ProfileImages[3].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[3]);
                            challenge.ImageId4 = upload.UploadId;
                        }

                    }

                    context.Challenges.Add(challenge);
                    try
                    {
                        context.SaveChanges();

                        return challenge.Id;

                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                                    validationErrors.Entry.Entity.GetType().FullName,
                                    validationError.PropertyName,
                                    validationError.ErrorMessage);
                            }
                        }

                        return null;
                    }
                }
            }
            return null;
        }

        public int? UpdateChallenge(Guid providerUserKey, ChallengeModel model)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.FirstOrDefault(u => u.Id == providerUserKey);
                var challenge = context.Challenges.FirstOrDefault(r => r.Id == model.Id);

                if (user != null && challenge != null)
                {

                    Utils.CopyProperties(model, challenge);
                    // challenge.State = model.AustraliaState;

                    challenge.Owner_Id = user.Id;
                    challenge.Instance_Id = user.Instance_Id;
                    challenge.LogoImageId = model.LogoImageId;

                    if (model.ProfileImages.Count > 0)
                    {
                        if (model.ProfileImages[0] != null && model.ProfileImages[0].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[0]);
                            challenge.ImageId1 = upload.UploadId;
                        }
                        else if (model.ImageId1 == null)
                        {
                            Uploads uploadOld = context.Uploads.SingleOrDefault(x => x.Id == challenge.ImageId1);
                            if (uploadOld != null)
                            {
                                context.Uploads.Remove(uploadOld);
                                challenge.ImageId1 = null;
                            }
                        }

                        if (model.ProfileImages[1] != null && model.ProfileImages[1].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[1]);
                            challenge.ImageId2 = upload.UploadId;
                        }
                        else if (model.ImageId2 == null)
                        {
                            Uploads uploadOld = context.Uploads.SingleOrDefault(x => x.Id == challenge.ImageId2);
                            if (uploadOld != null)
                            {
                                context.Uploads.Remove(uploadOld);
                                challenge.ImageId2 = null;
                            }
                        }

                        if (model.ProfileImages[2] != null && model.ProfileImages[2].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[2]);
                            challenge.ImageId3 = upload.UploadId;
                        }
                        else if (model.ImageId3 == null)
                        {
                            Uploads uploadOld = context.Uploads.SingleOrDefault(x => x.Id == challenge.ImageId3);
                            if (uploadOld != null)
                            {
                                context.Uploads.Remove(uploadOld);
                                challenge.ImageId3 = null;
                            }
                        }

                        if (model.ProfileImages[3] != null && model.ProfileImages[3].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[3]);
                            challenge.ImageId4 = upload.UploadId;
                        }
                        else if (model.ImageId4 == null)
                        {
                            Uploads uploadOld = context.Uploads.SingleOrDefault(x => x.Id == challenge.ImageId4);
                            if (uploadOld != null)
                            {
                                context.Uploads.Remove(uploadOld);
                                challenge.ImageId4 = null;
                            }
                        }
                    }

                    try
                    {
                        context.SaveChanges();

                        return challenge.Id;
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                                    validationErrors.Entry.Entity.GetType().FullName,
                                    validationError.PropertyName,
                                    validationError.ErrorMessage);
                            }
                        }

                        return null;
                    }
                }
            }

            return null;
        }


        public ChallengesListModel GetChallenges(Guid providerUserKey, int page, int pageSize, int orderBy, int? status, int? category, string search)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = new UserRepository().GetUserById(providerUserKey);
                ChallengesListModel model = new ChallengesListModel();

                var challenges = context.Challenges.Where(r => r.Instance_Id == user.Instance_Id)
                                                    .Where(r => (category == null || category == 0) || r.ChallengeCategoryId == category)
                                                    .Where(r => search == null || r.Name.Contains(search) || r.About.Contains(search));


                DateTime now = DateTime.Now.Date;

                DateTime inWeek = DateTime.Now.Date.AddDays(7);


                switch (status)
                {
                    case 1: challenges = challenges.Where(x => x.Display == false);
                        break;
                    case 2: challenges = challenges.Where(x => (x.Display == true) && (x.StartDate <= now || x.StartDate == null) && (x.EndDate >= now || x.EndDate == null));
                        break;
                    case 3: challenges = challenges.Where(x => (x.Display == false) || (x.StartDate > now) || (x.EndDate < now));
                        break;
                    case 4: challenges = challenges.Where(x => x.EndDate > now && x.EndDate <= inWeek);
                        break;

                }

                model.NumChallenges = challenges.Count();
                model.Page = page;
                model.NumPages = (int)Math.Ceiling((double)challenges.Count() / pageSize);

                if (pageSize == 0)
                    pageSize = model.NumChallenges;



                List<ChallengeModel> list = new List<ChallengeModel>();
                //IEnumerable<ChallengeModel> list = new IEnumerable<ChallengeModel>();
                foreach (var challenge in challenges)
                {
                    var challengeModel = new ChallengeModel();
                    Utils.CopyProperties(challenge, challengeModel);
                    challengeModel.Participants = challenge.UserChallenges.Count();
                    //  challengeModel.AustraliaState = challenge.State;
                    if (challenge.ChallengeCategoryId != null)
                    {
                        challengeModel.ChallengeCategory = challenge.ChallengeCategories.Name;
                    }
                    else
                    {
                        challengeModel.ChallengeCategory = "";
                    }

                    if ((challenge.StartDate <= DateTime.Now || challenge.StartDate == null) && (challenge.EndDate >= now || challenge.EndDate == null) && (challenge.Display == true))
                    {
                        challengeModel.ChallengeStatus = "Active";
                    }
                    else
                    {
                        challengeModel.ChallengeStatus = "Not active";
                    }

                    list.Add(challengeModel);
                }

                switch (orderBy)
                {
                    case 0: list = list.OrderByDescending(x => x.DateAdded).ToList();
                        break;
                    case 1: list = list.OrderBy(x => x.DateAdded).ToList();
                        break;
                    case 2: list = list.OrderByDescending(x => x.Participants).ToList();
                        break;
                    case 3: list = list.OrderBy(x => x.EndDate).ToList();
                        break;
                    case 4: list = list.OrderByDescending(x => x.Popularity).ToList();
                        break;

                }



                model.ChallengesList = list.AsEnumerable<ChallengeModel>().Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return model;
            }
        }


        public ChallengeModel GetChallengeDetails(int id)
        {
            using (var context = new greenMoneyEntities())
            {
                var challenge = context.Challenges.SingleOrDefault(x => x.Id == id);


                ChallengeModel model = new ChallengeModel();

                Utils.CopyProperties(challenge, model);
                // model.AustraliaState = challenge.State;

                return model;
            }
        }

        public UserChallengeListModel GetUserChallenges(int id, int? orderBy, bool? pointsClaimed, string suburb, string search, int? timeRange, int page, int pageSize)
        {
            using (var context = new greenMoneyEntities())
            {
                UserChallengeListModel listModel = new UserChallengeListModel();
                if (timeRange == 100 || timeRange == null)
                    timeRange = 0;
                int range = (-1) * Convert.ToInt32(timeRange);
                DateTime history = DateTime.Now.Date.AddDays(range);

                var userChallenges = from uc in context.UserChallenges
                                     join u in context.Users1 on uc.UserId equals u.Id //uc.UserId.Equals(u.Id)
                                     join a in context.Addresses on u.Address_Id equals a.Id
                                     where uc.ChallengeId == id && (uc.PointsClaimed == pointsClaimed || pointsClaimed == null) && (a.Suburb == suburb || suburb == "Suburb") && (u.FirstName.Contains(search) || u.LastName.Contains(search))
                                            && (uc.Issued > history || range == 0)
                                     orderby uc.Issued descending
                                     select new
                                     {
                                         Id = uc.Id,
                                         ChallengeId = uc.ChallengeId,
                                         Issued = uc.Issued,
                                         UserId = uc.UserId,
                                         UserName = (String.IsNullOrEmpty(u.FirstName) ? "" : u.FirstName) + " " + (String.IsNullOrEmpty(u.LastName) ? "" : u.LastName),
                                         Suburb = a.Suburb,
                                         Workplace = a.StreetName,
                                         PointsClaimed = uc.PointsClaimed
                                     };

                if (pageSize == 0)
                    pageSize = userChallenges.Count();

                switch (orderBy)
                {
                    case 0: userChallenges = userChallenges.OrderByDescending(x => x.Issued);
                        break;
                    case 1: userChallenges = userChallenges.OrderBy(x => x.Issued);
                        break;
                }

                listModel.NumChallenges = userChallenges.Count();
                listModel.Page = page;
                listModel.NumPages = (int)Math.Ceiling((double)userChallenges.Count() / pageSize);

                userChallenges = userChallenges.Skip((page - 1) * pageSize).Take(pageSize);


                //var userChallenges = context.UserChallenges.Where(x => x.ChallengeId == id).Include(p => p.Users1);

                List<UserChallengeModel> list = new List<UserChallengeModel>();

                foreach (var userChallenge in userChallenges)
                {
                    UserChallengeModel userChallengeModel = new UserChallengeModel();
                    Utils.CopyProperties(userChallenge, userChallengeModel);
                    list.Add(userChallengeModel);
                }
                listModel.UserChallengeList = list;
                return listModel;


            }
        }

        public ChallengeModel CopyChallenge(int id)
        {
            using (var context = new greenMoneyEntities())
            {
                Challenges challenge = context.Challenges.Single(x => x.Id == id);
                challenge.DateAdded = DateTime.Now;
                context.Challenges.Add(challenge);

                try
                {
                    context.SaveChanges();

                    ChallengeModel challengeModel = new ChallengeModel();

                    Utils.CopyProperties(challenge, challengeModel);
                    //  challengeModel.AustraliaState = challenge.State;

                    return challengeModel;

                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                                validationErrors.Entry.Entity.GetType().FullName,
                                validationError.PropertyName,
                                validationError.ErrorMessage);
                        }
                    }

                    return null;
                }
            }
        }

        public bool DeleteChallenge(int id)
        {
            using (var context = new greenMoneyEntities())
            {
                Challenges challenge = context.Challenges.Single(x => x.Id == id);
                context.Challenges.Remove(challenge);
                try
                {
                    context.SaveChanges();

                }
                catch (Exception)
                {

                    return false;
                }
                return true;
            }
        }

        public bool DeleteUserChallenge(int id)
        {
            using (var context = new greenMoneyEntities())
            {
                UserChallenges userChallenge = context.UserChallenges.Single(x => x.Id == id);
                context.UserChallenges.Remove(userChallenge);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception)
                {

                    return false;
                }
                return true;
            }
        }

        public bool AddPoints(int userChallengeId)
        {
            using (var context = new greenMoneyEntities())
            {
                var time = DateTime.Now;

                UserChallenges userChallenge = context.UserChallenges.Single(x => x.Id == userChallengeId);
                Users1 user = context.Users1.Single(x => x.Id == userChallenge.UserId);
                Challenges challenge = context.Challenges.Single(x => x.Id == userChallenge.ChallengeId);

                userChallenge.PointsClaimed = true;

                var transaction = new Transactions
                {
                    Address_Id = user.Address_Id,
                    User_Id = user.Id,
                    Time = time,
                    Description = "Challenge " + challenge.Name,
                    Points = Convert.ToInt64(challenge.Points),
                    TransactionTypeID = 2
                };

                context.Transactions.Add(transaction);
                context.SaveChanges();

                return true;
            }
        }
        public DateTime GetLastActivityDate(Guid id)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users.SingleOrDefault(x => x.UserId == id);
                DateTime lastActivity = user.LastActivityDate;
                return lastActivity;
            }
        }

        public bool CheckInInstance(int challengeId, Guid userId)
        {
            using (var context = new greenMoneyEntities())
            {
                var challenge = context.Challenges.SingleOrDefault(x => x.Id == challengeId);
                var user = context.Users1.Single(x => x.Id == userId);

                bool InInstance = (challenge.Instance_Id == user.Instance_Id);
                return InInstance;
            }

        }


        public UserChallengeModel GetUserChallenge(int id)
        {
            using (var context = new greenMoneyEntities())
            {
                UserChallenges userChallenge = context.UserChallenges.Single(x => x.Id == id);
                UserChallengeModel ucModel = new UserChallengeModel();

                Utils.CopyProperties(userChallenge, ucModel);

                return ucModel;
            }
        }

    }
}
