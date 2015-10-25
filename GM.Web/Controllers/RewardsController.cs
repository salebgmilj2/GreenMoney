using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using EvoPdf.HtmlToPdf;
using GM.BusinessLogic;
using GM.Models.Public;
using GM.Utility;
using GM.ViewModels;
using GM.ViewModels.Shared;

namespace GM.Controllers
{
    [Authorize]
    public class RewardsController : SimpleApplicationController
    {

        [AllowAnonymous]
        public ActionResult Index(int? instId, string category = null, int page = 1, string sortBy = "popular", string filter = "")
        {
            LayoutViewModel.ActiveLink = Links.Rewards;

            RewardsListModel rewardsModel;

            //Auspost doesn't have dropdown
            var privateClientId = ConfigurationManager.AppSettings["PrivateClientId"] == null
                ? 5 : int.Parse(ConfigurationManager.AppSettings["PrivateClientId"]);


            if (LayoutViewModel.Instance_Id == privateClientId)
            {
                //Melbourne is considered as default council and selected for auspost by default rewards view
                var defaultClientId = ConfigurationManager.AppSettings["DefaultClientId"] == null
                    ? 1 : int.Parse(ConfigurationManager.AppSettings["DefaultClientId"]);

                instId = SetDefaultCouncil(Request, instId, defaultClientId);

                rewardsModel = new RewardsService().GetRewards(instId, null, sortBy, category, filter, page, 10);
            }
            else
            {
                instId = SetDefaultCouncil(Request, instId, LayoutViewModel.Instance_Id);
                rewardsModel = new RewardsService().GetRewards(instId, privateClientId, sortBy, category, filter, page, 10);
            }

            SortByEnum selectedSortByEnum = EnumUtils.ConvertStringToEnum<SortByEnum>(sortBy);


            RewardCategory selectedCategory = null;
            IEnumerable<RewardCategory> categoriesAll = GetRewardCategories();

            List<RewardCategory> subCategories = null;

            var categories = categoriesAll as IList<RewardCategory> ?? categoriesAll.ToList();
            var rewardCategories = categoriesAll as IList<RewardCategory> ?? categories.ToList();
            if (category != null && categoriesAll != null)
            {
                selectedCategory = rewardCategories.FirstOrDefault(c => c.Slug.Equals(category, StringComparison.OrdinalIgnoreCase));

                //If subcategory is selected mark parent as selected category
                if (selectedCategory != null)
                {
                    if (selectedCategory.ParentId != 0)
                    {
                        subCategories = categories.Where(c => selectedCategory != null && c.ParentId == selectedCategory.ParentId).ToList();

                        var subCategoryAsSelected = subCategories.FirstOrDefault(c => selectedCategory != null && c.Id == selectedCategory.Id);
                        if (subCategoryAsSelected != null)
                            subCategoryAsSelected.IsSelected = true;

                        selectedCategory = categoriesAll.FirstOrDefault(c => selectedCategory.ParentId != null && c.Id == (int)selectedCategory.ParentId);
                    }
                    else
                    {
                        subCategories = categories.Where(c => selectedCategory != null && c.ParentId == selectedCategory.Id).ToList();
                    }
                }
            }

            RewardsViewModel viewModel = new RewardsViewModel
            {
                Rewards = rewardsModel.RewardsList,
                InstanceId = instId,
                ShowDropdownCity = true,
                Page = rewardsModel.Page,
                NumPages = rewardsModel.NumPages,
                NumRewards = rewardsModel.NumRewards,
                SortBy = sortBy,
                SortByText = EnumUtils.GetDescription(selectedSortByEnum),
                Category = selectedCategory,
                SubCategories = subCategories,
                Categories = rewardCategories.Where(c => c.ParentId == 0).ToList(),
                Filter = filter
            };


            return View(viewModel);
        }

        private int? SetDefaultCouncil(System.Web.HttpRequestBase Request, int? instId, int? defaultInstId)
        {
            if (Request.UrlReferrer != null && Request.UrlReferrer.LocalPath == Request.Url.LocalPath)
            {
                return instId;
            }
            return defaultInstId;
        }

        [AllowAnonymous]
        public ViewResult Details(int id)
        {
            RewardCategory selectedCategory = null;
            IEnumerable<RewardCategory> categoriesAll = GetRewardCategories();


            RewardDetailsViewModel viewModel = new RewardDetailsViewModel();
            RewardModel rewardModel = new RewardsService().GetRewardDetails(id);
            viewModel.Reward = rewardModel;

            var numOfImages = 0;
            if (rewardModel.ImageId != null)
            {
                numOfImages++;
            }
            if (rewardModel.Image2Id != null)
            {
                numOfImages++;
            }
            if (rewardModel.Image3Id != null)
            {
                numOfImages++;
            }
            if (rewardModel.Image4Id != null)
            {
                numOfImages++;
            }

            viewModel.ShowSlider = numOfImages > 1;

            var rewardCategories = categoriesAll as IList<RewardCategory> ?? categoriesAll.ToList();

            // Reward category is not one of main categories
            var mainCategories = rewardCategories.Where(c => c.ParentId == 0).ToList();
            if (mainCategories.All(c => c.Id != rewardModel.CategoryId))
            {
                var cat = rewardCategories.First(c => c.Id == rewardModel.CategoryId);
                viewModel.SelectedCategory = rewardCategories.FirstOrDefault(c => c.Id == cat.ParentId);
            }
            else // Reward category is one of main categories
            {
                viewModel.SelectedCategory = rewardCategories.FirstOrDefault(c => c.Id == rewardModel.CategoryId);
            }

            viewModel.Categories = mainCategories;

            return View(viewModel);
        }

        private IEnumerable<RewardCategory> GetRewardCategories()
        {
            List<RewardCategory> rewardsCategories = new RewardsService().GetRewardCategories();

            return rewardsCategories;
        }

        public ActionResult Voucher(Guid id, int? rewardId)
        {
            var pdfConverter = new PdfConverter();

            pdfConverter.LicenseKey = "Zk1URlVVRldTXlVGVUhWRlVXSFdUSF9fX18=";

            pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
            pdfConverter.HtmlViewerWidth = 800;

            //var url = Url.Action("VoucherHtml", "Rewards", new { id });
            var url = Url.Action("VoucherHtml", "Rewards",
                new RouteValueDictionary(new { id = id, rewardId }),
                "http", Request.Url.Host);

            try
            {
                var bytes = pdfConverter.GetPdfBytesFromUrl(url);
                return File(bytes, "application/pdf");
            }
            catch (Exception)
            {
                return RedirectToAction("VoucherHtml", new { id });
            }

        }

        [AllowAnonymous]
        public ActionResult VoucherHtml(Guid id, int? rewardId)
        {
            //Dummy voucher
            if (id == new Guid())
            {
                var viewModelDummy = new VoucherHtmlViewModel();
                RewardModel rewardModel = new RewardsService().GetRewardDetails((int)rewardId);
                viewModelDummy.VoucherModel = new VoucherModel
                {
                    Issued = DateTime.Now,
                    RewardModel = rewardModel,
                    UserFirstName = "First name",
                    UserLastName = "Last name"
                };

                return View("Voucher", viewModelDummy);
            }

            VoucherModel voucher = new RewardsService().GetVoucher(id);

            VoucherHtmlViewModel viewModel = new VoucherHtmlViewModel();
            viewModel.VoucherModel = voucher;

            return View("Voucher", viewModel);
        }

    }
}
