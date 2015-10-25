using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using GM.DataAccess.Entities;
using GM.DataAccess.Interfaces;
using GM.Models;
using GM.Models.Public;

namespace GM.DataAccess
{
    public class RewardsRepository : IRewardsRepository
    {
        public RewardsListModel GetRewards(int? instanceId, int? excludeInstanceId, string sortBy, string category, string filter, int page, int pageSize)
        {
            using (var context = new greenMoneyEntities())
            {

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                //Get all rewards
                var rewards = context.Rewards
                    .Where(r => r.AvailableFrom == null || r.AvailableFrom.Value <= DateTime.Now)
                    .Where(r => r.AvailableTo == null || r.AvailableTo.Value > DateTime.Now)
                    .Where(r => r.Display)
                    .Where(
                        r =>
                            r.State == null || (int)r.State == (int)RewardState.Approved ||
                            (int)r.State == (int)RewardState.NotModerated).ToList();

                if (instanceId != null && instanceId != 0)
                {
                    rewards = rewards.Where(r => r.Instance_Id == instanceId).ToList();
                }
                else if (excludeInstanceId != null)
                {
                    rewards = rewards.Where(r => r.Instance_Id != excludeInstanceId).ToList();
                }

                //Filter all by text query
                if (!string.IsNullOrEmpty(filter))
                    rewards =
                        rewards.Where(
                            r =>
                                r.Name.ToLower().IndexOf(filter.ToLower()) > -1 ||
                                r.Description.ToLower().IndexOf(filter.ToLower()) > -1).ToList();


                List<Rewards> selected = new List<Rewards>();

                //Filter all by selected category
                RewardCategories rewardCategory;
                if (category != null)
                {
                    rewardCategory =
                        context.RewardCategories.FirstOrDefault(
                            c => category.Equals(c.Slug, StringComparison.OrdinalIgnoreCase));
                    if (rewardCategory == null)
                    {
                        throw new Exception("reward category not found");
                    }


                    //get all items, for selected and for subcategories
                    if (rewardCategory.ParentId == 0)
                    {
                        var allAvailableCategories = context.RewardCategories
                            .Where(c => c.Rewards.Any(
                                r => (r.AvailableFrom == null || r.AvailableFrom.Value <= DateTime.Now)
                                     && (r.AvailableTo == null || r.AvailableTo.Value > DateTime.Now)));

                        List<int> subCategoriesIds = allAvailableCategories
                            .Where(c => c.ParentId == rewardCategory.Id).Select(c => c.Id).ToList();

                        subCategoriesIds.Add(rewardCategory.Id);

                        foreach (var subCategoriesId in subCategoriesIds)
                        {
                            selected.AddRange(rewards.Where(r => r.CategoryId == subCategoriesId).ToList());
                        }
                    }
                    else
                    {
                        selected = rewards.Where(r => r.CategoryId == rewardCategory.Id).ToList();
                    }
                }
                else
                {
                    selected = rewards.ToList();
                }

                switch (sortBy)
                {
                    case "newest":
                        selected = selected.OrderByDescending(r => r.AvailableFrom).ThenByDescending(r => r.Id).ToList();
                        break;
                    case "oldest":
                        selected = selected.OrderBy(r => r.AvailableFrom).ThenBy(r => r.Id).ToList();
                        break;
                    case "cheapest":
                        selected = selected.OrderBy(r => r.Price).ToList();
                        break;
                    case "expensive":
                        selected = selected.OrderByDescending(r => r.Price).ToList();
                        break;
                    case "popular":
                        selected = selected.OrderByDescending(r => r.Popularity).ToList();
                        break;
                    default:
                        selected = selected.OrderByDescending(r => r.Popularity).ToList();
                        break;
                }

                //Include pagination
                var list = selected.Skip((page - 1) * pageSize).Take(pageSize);

                List<RewardModel> models = new List<RewardModel>();

                foreach (var item in list)
                {
                    RewardModel m = new RewardModel();
                    Utils.CopyProperties(item, m);

                    models.Add(m);
                }


                RewardsListModel model = new RewardsListModel
                {
                    RewardsList = models,
                    NumPages = (int)Math.Ceiling((double)selected.Count() / pageSize),
                    NumRewards = selected.Count(),
                    Page = page
                };

                return model;
            }

        }

        public List<RewardCategory> GetRewardCategories()
        {
            using (var context = new greenMoneyEntities())
            {
                //var categories = context.RewardCategories
                //    .Where(c => c.Rewards.Any(r => r.Display
                //        && (r.AvailableFrom == null || r.AvailableFrom.Value <= DateTime.Now)
                //        && (r.AvailableTo == null || r.AvailableTo.Value > DateTime.Now)))
                //    .OrderBy(c => c.Name);

                //var categories = context.RewardCategories
                //    .Where(c => c.Rewards.Any(r => r.Display))
                //    .OrderBy(c => c.Name);


                var categories = context.RewardCategories
                    .OrderBy(c => c.Name);

                //var categories = context.RewardCategories
                //    .Where(c => c.Rewards.Any(r => (r.AvailableFrom == null || r.AvailableFrom.Value <= DateTime.Now)
                //        && (r.AvailableTo == null || r.AvailableTo.Value > DateTime.Now)))
                //    .OrderBy(c => c.Name);

                List<RewardCategory> list = new List<RewardCategory>();
                foreach (var category in categories)
                {
                    RewardCategory model = new RewardCategory();
                    Utils.CopyProperties(category, model);

                    list.Add(model);
                }

                return list;
            }
        }

        public RewardModel GetRewardDetails(int id)
        {
            using (var context = new greenMoneyEntities())
            {
                var reward = context.Rewards.SingleOrDefault(x => x.Id == id);
                RewardModel model = new RewardModel();

                Utils.CopyProperties(reward, model);

                return model;
            }
        }

        public VoucherModel GetVoucher(Guid id)
        {
            using (var context = new greenMoneyEntities())
            {
                var voucher = context.Vouchers.Find(id);

                VoucherModel model = new VoucherModel();
                model.VoucherId = voucher.Id;
                model.Issued = voucher.Issued;
                model.UserFirstName = voucher.Users1.FirstName;
                model.UserLastName = voucher.Users1.LastName;

                RewardModel rewardModel = new RewardModel();
                Utils.CopyProperties(voucher.Rewards, rewardModel);

                model.RewardModel = rewardModel;

                return model;
            }
        }

        public RewardsListModel GetSupplierRewards(Guid providerUserKey, int page, int pageSize)
        {
            using (var context = new greenMoneyEntities())
            {

                var rewards = context.Rewards.Where(r => r.Owner_Id == providerUserKey);
                //var uploads = context.Uploads.Where(u => u.UploadedBy_Id == providerUserKey);

                //Include pagination
                var rewardsList = rewards.OrderByDescending(r => r.DateAdded).Skip((page - 1) * pageSize).Take(pageSize);

                List<RewardModel> list = new List<RewardModel>();
                foreach (var reward in rewardsList)
                {
                    RewardModel rewardModel = new RewardModel();
                    Utils.CopyProperties(reward, rewardModel);

                    list.Add(rewardModel);
                }

                RewardsListModel model = new RewardsListModel();
                model.RewardsList = list;
                model.NumRewards = rewards.Count();
                model.Page = page;
                model.NumPages = (int)Math.Ceiling((double)rewards.Count() / pageSize);

                return model;
            }
        }

        public RewardsSummaryListModel GetSupplierRewardsSummary(Guid providerUserKey, int page, int pageSize)
        {
            using (var context = new greenMoneyEntities())
            {
                var rewards = context.Rewards.Where(r => r.Owner_Id == providerUserKey);

                //Include pagination
                var rewardsList = rewards.OrderByDescending(r => r.DateAdded).Skip((page - 1) * pageSize).Take(pageSize);

                List<RewardSummaryModel> list = new List<RewardSummaryModel>();
                foreach (var reward in rewardsList)
                {
                    RewardSummaryModel rewardModel = new RewardSummaryModel();
                    rewardModel.Id = reward.Id;
                    rewardModel.ImageSmallId = reward.ImageSmallId;
                    rewardModel.Name = reward.Name;
                    rewardModel.PartnerName = reward.PartnerName;
                    rewardModel.State = reward.State;

                    var instance = context.Instance.Where(i => i.Id == reward.Instance_Id).FirstOrDefault();
                    if (instance != null)
                    {
                        rewardModel.City = instance.Name;
                    }

                    rewardModel.NumOfRedemptions = context.Vouchers
                                                    .Where(v => v.Reward_Id == reward.Id)
                                                    .Count();

                    RewardState status = RewardState.NotModerated;

                    if (reward.State != null)
                    {
                        if (reward.State == (int)RewardState.Approved && reward.Display == true)
                        {
                            status = RewardState.Approved;
                        }
                        else if (reward.Display == false)
                        {
                            status = RewardState.NotDisplayed;
                        }
                        else if (reward.State == (int)RewardState.WaitingApproval)
                        {
                            status = RewardState.WaitingApproval;
                        }
                    }

                    rewardModel.RewardState = status;


                    list.Add(rewardModel);
                }

                RewardsSummaryListModel model = new RewardsSummaryListModel();
                model.RewardsList = list;
                model.NumRewards = rewards.Count();
                model.Page = page;
                model.NumPages = (int)Math.Ceiling((double)rewards.Count() / pageSize);

                return model;
            }
        }

        public int? CreateReward(Guid providerUserKey, RewardModel model)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.FirstOrDefault(u => u.Id == providerUserKey);

                if (user != null)
                {
                    Rewards reward = new Rewards();
                    reward.PartnerEmail = model.PartnerEmail;
                    reward.State = (int)RewardState.Incomplete;
                    reward.Owner_Id = user.Id;
                    reward.Instance_Id = user.Instance_Id;
                    reward.Instance = user.Instance;

                    Utils.CopyProperties(model, reward);

                    reward.DateAdded = DateTime.Now;

                    if (model.ProfileImages.Count > 0)
                    {
                        if (model.ProfileImages[0] != null && model.ProfileImages[0].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[0]);
                            reward.ImageId = upload.UploadId;
                        }

                        if (model.ProfileImages[1] != null && model.ProfileImages[1].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[1]);
                            reward.Image2Id = upload.UploadId;
                        }

                        if (model.ProfileImages[2] != null && model.ProfileImages[2].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[2]);
                            reward.Image3Id = upload.UploadId;
                        }

                        if (model.ProfileImages[3] != null && model.ProfileImages[3].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[3]);
                            reward.Image4Id = upload.UploadId;
                        }

                    }

                    context.Rewards.Add(reward);

                    try
                    {
                        context.SaveChanges();

                        return reward.Id;

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

        public int? UpdateReward(Guid providerUserKey, RewardModel model)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.FirstOrDefault(u => u.Id == providerUserKey);
                var reward = context.Rewards.FirstOrDefault(r => r.Id == model.Id);

                if (user != null && reward != null)
                {
                    reward.PartnerEmail = model.PartnerEmail;
                    Utils.CopyProperties(model, reward);

                    //CopyProperties doesn't work for null values...
                    reward.ImageSmallId = model.ImageSmallId;
                    reward.VoucherBarcodeId = model.VoucherBarcodeId;

                    if (model.ProfileImages.Count > 0)
                    {
                        if (model.ProfileImages[0] != null && model.ProfileImages[0].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[0]);
                            reward.ImageId = upload.UploadId;
                        }
                        else if (model.ImageId == null)
                        {
                            Uploads uploadOld = context.Uploads.SingleOrDefault(x => x.Id == reward.ImageId);
                            if (uploadOld != null)
                            {
                                context.Uploads.Remove(uploadOld);
                                reward.ImageId = null;
                            }
                        }

                        if (model.ProfileImages[1] != null && model.ProfileImages[1].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[1]);
                            reward.Image2Id = upload.UploadId;
                        }
                        else if (model.Image2Id == null)
                        {
                            Uploads uploadOld = context.Uploads.SingleOrDefault(x => x.Id == reward.Image2Id);
                            if (uploadOld != null)
                            {
                                context.Uploads.Remove(uploadOld);
                                reward.Image2Id = null;
                            }
                        }

                        if (model.ProfileImages[2] != null && model.ProfileImages[2].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[2]);
                            reward.Image3Id = upload.UploadId;
                        }
                        else if (model.Image3Id == null)
                        {
                            Uploads uploadOld = context.Uploads.SingleOrDefault(x => x.Id == reward.Image3Id);
                            if (uploadOld != null)
                            {
                                context.Uploads.Remove(uploadOld);
                                reward.Image3Id = null;
                            }
                        }

                        if (model.ProfileImages[3] != null && model.ProfileImages[3].FileName != null)
                        {
                            var upload = new UploadsRepository().UploadFile(providerUserKey.ToString(), model.ProfileImages[3]);
                            reward.Image4Id = upload.UploadId;
                        }
                        else if (model.Image4Id == null)
                        {
                            Uploads uploadOld = context.Uploads.SingleOrDefault(x => x.Id == reward.Image4Id);
                            if (uploadOld != null)
                            {
                                context.Uploads.Remove(uploadOld);
                                reward.Image4Id = null;
                            }
                        }
                    }

                    try
                    {

                        context.SaveChanges();

                        return reward.Id;
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

        public Tuple<int, List<RedemptionModel>> GetRedemptions(int rewardId, DateTime startDate, DateTime endDate, int page, int pageSize, string filter)
        {
            using (var context = new greenMoneyEntities())
            {
                var redemptions = context.Vouchers
                    .Where(v => v.Reward_Id == rewardId && v.Issued >= startDate && endDate >= v.Issued)
                    .OrderBy(v => v.Rewards.PartnerName)
                    .ThenBy(v => v.Issued)
                    .ToList()
                    .Select(v => new RedemptionModel
                    {
                        PartnerName = v.Rewards.PartnerName,
                        VoucherId = v.Id,
                        Code = GM.DataAccess.Utility.ZBase32.Encode(v.Id.ToByteArray()),
                        Issued = v.Issued,
                        ValidFrom = v.Rewards.ValidFrom,
                        ValidTo = v.Rewards.ValidTo,
                        RewardName = v.Rewards.Name,
                        FirstName = v.Users1.FirstName,
                        LastName = v.Users1.LastName,
                        UserId = v.Users1.Id,
                        City = v.Users1.Addresses.Suburb
                    }).ToList();


                //List<RedemptionModel>
                //ZBase32.Encode(Id.ToByteArray()

                //Filter all by text query
                if (!string.IsNullOrEmpty(filter))
                {
                    redemptions =
                        redemptions.Where(
                            r =>
                                r.FirstName.ToLower().IndexOf(filter.ToLower()) > -1 ||
                                r.LastName.ToLower().IndexOf(filter.ToLower()) > -1 ||
                                r.Code.ToLower().IndexOf(filter.ToLower()) > -1).ToList();
                }

                //Include pagination
                List<RedemptionModel> list = redemptions.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                Tuple<int, List<RedemptionModel>> tuple = new Tuple<int, List<RedemptionModel>>(redemptions.Count, list);

                return tuple;
            }
        }


        public RedemptionsStatisticsListModel GetRedemptionsStatistics(string providerUserKey, int page, int pageSize)
        {
            DateTime dateLast7Days = DateTime.Today.AddDays(-7);
            DateTime dateLast28Days = DateTime.Today.AddDays(-28);
            DateTime dateAllEver = new DateTime(2000, DateTime.Today.Month - 1, 1);

            RedemptionsStatisticsListModel model = new RedemptionsStatisticsListModel();

            using (var context = new greenMoneyEntities())
            {
                var rewards = context.Rewards.Where(r => r.Owner_Id == new Guid(providerUserKey)).OrderByDescending(r => r.DateAdded);

                //Include pagination
                var list = rewards.Skip((page - 1) * pageSize).Take(pageSize);

                List<RedemptionStatisticModel> statsList = new List<RedemptionStatisticModel>();

                foreach (var reward in list)
                {
                    RedemptionStatisticModel m = new RedemptionStatisticModel();

                    m.RewardId = reward.Id;
                    m.PartnerName = reward.PartnerName;
                    m.RewardName = reward.Name;
                    m.ImageSmallId = reward.ImageSmallId;
                    m.ImageId = reward.ImageId;

                    m.RedeemsForLast7Days = context.Vouchers
                    .Where(v => v.Reward_Id == reward.Id && v.Issued >= dateLast7Days)
                    .Count();

                    m.RedeemsForThisMonth = context.Vouchers
                    .Where(v => v.Reward_Id == reward.Id && v.Issued >= dateLast28Days)
                    .Count();

                    m.RedeemsForLastMonth = context.Vouchers
                    .Where(v => v.Reward_Id == reward.Id && v.Issued >= dateAllEver)
                    .Count();

                    statsList.Add(m);
                }

                model.RewardsList = statsList;
                model.NumPages = (int)Math.Ceiling((double)rewards.Count() / pageSize);
                model.NumRewards = rewards.Count();
                model.Page = page;

                return model;
            }
        }

        public ActivitySummaryModel GetActivitySummaryModelModel(string providerUserKey)
        {
            DateTime dateLast7Days = DateTime.Today.AddDays(-7);

            ActivitySummaryModel model = new ActivitySummaryModel
            {
                Gender = "0"
            };

            using (var context = new greenMoneyEntities())
            {
                var rewardsAll = context.Rewards;
                var rewards = rewardsAll.Where(r => r.Owner_Id == new Guid(providerUserKey));

                var order = (from h in context.Rewards
                             group h by new { h.Owner_Id }
                                 into hh
                                 select new
                                 {
                                     hh.Key.Owner_Id,
                                     Score = hh.Sum(s => s.Popularity)
                                 }).OrderByDescending(i => i.Score);

                var myScore = order.FirstOrDefault(x => x.Owner_Id == new Guid(providerUserKey));

                int myScoreValue = myScore == null ? 0 : myScore.Score;
                int position = 0;
                foreach (var x in order)
                {
                    position = position + 1;
                    if (x.Score <= myScoreValue)
                    {
                        break;
                    }
                }

                var count = 0;
                List<string> genders = new List<string>();
                List<DateTime?> ages = new List<DateTime?>();

                foreach (var reward in rewards)
                {
                    var q = context.Vouchers.Where(v => v.Reward_Id == reward.Id && v.Issued >= dateLast7Days);
                    var c = q.Count();
                    count += c;

                    List<string> sex = q.Select(v => v.Users1.Sex.ToLower()).ToList();
                    genders.AddRange(sex);

                    List<DateTime?> age = q.Select(v => v.Users1.DateOfBirth).ToList();
                    ages.AddRange(age);
                }

                if (count > 0)
                {
                    model.VoucherRedeemed = count;
                    model.OveralPopularity = position;

                    //TODO - include others too
                    var hasValue = genders.Where(g => g != null);
                    var male = hasValue.Count(g => g == "male");

                    var malePerc = (male / count * 100);
                    model.Gender = malePerc > 50
                        ? string.Format("{0}% male", malePerc)
                        : string.Format("{0}% female", 100 - malePerc);

                    var agesHasVale = ages.Where(x => x.HasValue);
                    var years = agesHasVale.Select(x => DateTime.Today.Year - x.Value.Year).Sum();

                    model.AverageAge = agesHasVale.Count() > 0 ? years / agesHasVale.Count() : 0;

                }

                return model;
            }
        }

        public bool DeleteReward(int id)
        {
            using (var context = new greenMoneyEntities())
            {
                Rewards reward = context.Rewards.Single(x => x.Id == id);
                context.Rewards.Remove(reward);
                context.SaveChanges();

                return true;
            }
        }
    }
}