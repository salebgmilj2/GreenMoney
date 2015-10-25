using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GM.BusinessLogic;
using GM.Models.Public;
using GM.Utility;
using GM.ViewModels.Challenges;

namespace GM.Controllers
{
    [Authorize]
    public class ChallengesController : SimpleApplicationController
    {
        private readonly ChallengesService _challengesService;
        public ChallengesController()
        {
            _challengesService = new ChallengesService();
        }

        [AllowAnonymous]
        public ActionResult Index(string category = null, int page = 1, string sortBy = "popular", string filter = "")
        {
            var instanceId = LayoutViewModel.Instance_Id.HasValue ? LayoutViewModel.Instance_Id.Value : -1;
            var challengesModel = _challengesService.GetChallenges(LayoutViewModel.ProviderUserKey, instanceId, sortBy, category, filter, page, 10);

            var selectedSortByEnum = EnumUtils.ConvertStringToEnum<SortByEnum>(sortBy);

            var categoriesAll = _challengesService.GetChallengeCategories();
            ChallengeCategory selectedCategory = null;
            if (!string.IsNullOrEmpty(category))
            {
                selectedCategory = categoriesAll.FirstOrDefault(x => x.ShortName.ToLower().Equals(category.ToLower()));
            }

            var viewModel = new ChallengesViewModel
            {
                Challenges = challengesModel.ChallengesList,
                Page = challengesModel.Page,
                NumPages = challengesModel.NumPages,
                NumRewards = challengesModel.NumRewards,
                SortBy = sortBy,
                SortByText = EnumUtils.GetDescription(selectedSortByEnum),
                Category = selectedCategory,
                Categories = categoriesAll,
                Filter = filter
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public ViewResult Details(int id)
        {
            var categoriesAll = _challengesService.GetChallengeCategories();

            var viewModel = new ChallengeDetailsViewModel();
            var challengeModel = new ChallengesService().GetChallengeDetails(id);
            viewModel.Challenge = challengeModel;

            var exists = _challengesService.CheckIfInMyChallenges(LayoutViewModel.ProviderUserKey, id);
            if (exists)
            {
                viewModel.PointsClaimed = _challengesService.ClaimedPoint(LayoutViewModel.ProviderUserKey, id);
            }
            
            var numOfImages = 0;
            if (challengeModel.ImageId1 != null)
            {
                numOfImages++;
            }
            if (challengeModel.ImageId2 != null)
            {
                numOfImages++;
            }
            if (challengeModel.ImageId3 != null)
            {
                numOfImages++;
            }
            if (challengeModel.ImageId4 != null)
            {
                numOfImages++;
            }

            viewModel.ShowSlider = numOfImages > 1;

            var challengeCategories = categoriesAll as IList<ChallengeCategory> ?? categoriesAll.ToList();

            viewModel.SelectedCategory = challengeCategories.FirstOrDefault(c => c.Id == challengeModel.ChallengeCategoryId);

            viewModel.Categories = challengeCategories;
            if (viewModel.SelectedCategory == null) return View("CategoryAction", viewModel);
            if (viewModel.SelectedCategory.ShortName.ToLower()
                .Equals(ChallengeCategoryEnum.Action.ToString().ToLower()))
            {
                return View("CategoryAction", viewModel);
            }
            if (viewModel.SelectedCategory.ShortName.ToLower()
                .Equals(ChallengeCategoryEnum.Pledge.ToString().ToLower()))
            {
                viewModel.Participating = exists && !viewModel.PointsClaimed;
                return View("CategoryPledge", viewModel);
            }
            if (viewModel.SelectedCategory.ShortName.ToLower()
                .Equals(ChallengeCategoryEnum.Learn.ToString().ToLower()))
            {
                return View("CategoryLearn", viewModel);
            }
            return View("CategoryAction", viewModel);
        }

        [AllowAnonymous]
        public bool ClaimPoints(int id)
        {
            var claimedPoints = _challengesService.ClaimPoints(LayoutViewModel.ProviderUserKey, id);
            return claimedPoints;
        }

        [AllowAnonymous]
        public bool ClaimPointsForPledge(int id)
        {
            var claimedPoints = _challengesService.ClaimPointsForPledge(LayoutViewModel.ProviderUserKey, id);
            return claimedPoints;
        }

        [AllowAnonymous]
        public bool AddPoints(int id, string promocode="")
        {
            var addedPoints = _challengesService.AddPoints(LayoutViewModel.ProviderUserKey, id, promocode);
            return addedPoints;
        }

        [AllowAnonymous]
        public bool ClaimPointsForLearn(int id)
        {
            var claimedPoints = _challengesService.ClaimPoints(LayoutViewModel.ProviderUserKey, id);
            var addedPoints = _challengesService.AddPoints(LayoutViewModel.ProviderUserKey, id);
            return addedPoints;
        }

    }
}
