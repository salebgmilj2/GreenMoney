using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using CsvHelper;
using evointernal;
using EvoPdf.HtmlToPdf;
using GM.BusinessLogic;
using GM.Models;
using GM.Models.Public;
using GM.Utility;
using GM.Utility.EmailService;
using GM.ViewModels.Shared;
using GM.ViewModels.Supplier;
using RazorEngine;

namespace GM.Controllers
{
    [Authorize]
    public class SupplierRewardsController : SimpleApplicationController
    {
        //
        // GET: /SupplierRewards/

        public ActionResult Index(int page = 1)
        {
            LayoutViewModel.ActiveLink = Links.SupplierRewards;

            SupplierRewardsViewModel viewModel = new SupplierRewardsViewModel();

            var membershipUser = Membership.GetUser();

            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                viewModel.Rewards = new RewardsService().GetSupplierRewards((Guid)membershipUser.ProviderUserKey, page, 5);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Create(int? id, bool initial = false, bool approve = false)
        {
            LayoutViewModel.ActiveLink = Links.CreateReward;

            var membershipUser = Membership.GetUser();

            CreateRewardViewModel viewModel;

            if (id != null)
            {
                viewModel = GetCreateRewardViewModelForUpdate(id.Value);

                if (viewModel == null)
                {
                    return RedirectToAction("Index", "SupplierRewards");
                }
                viewModel.IsUpdate = true;
                viewModel.IsSentForApprove = approve;

                LayoutViewModel.ActiveLink = Links.Rewards;
            }
            else
            {
                viewModel = new CreateRewardViewModel();
                //EnableDisplayRewardOption is false - always disabled when create new reward

                var partner = new UserService().GetUserById(LayoutViewModel.CurrentAccountId);
                viewModel.PartnerName = partner.BusinessName;
                viewModel.PartnerLocationCurrent = null;

                if (initial)
                {
                    LayoutViewModel.HideTopWrapperMenu = true;
                    viewModel.IsInitialRegistrationStep = true;
                }

                // Set default text area values
                viewModel.HowToRedeem =
                    "1. Print out this GreenMoney voucher.\r\n2. Present the printed voucher at the location displayed.\r\n3. Enjoy and tell your friends.";
                viewModel.TermsAndConditions =
                    "\u2022 This offer cannot be multiplied or exchanged for cash.\r\n" +
                    "\u2022 The offer is not valid towards previous purchase.\r\n" +
                    "\u2022 Void if copied or transferred.\r\n" +
                    "\u2022 One voucher per customer.\r\n" +
                    "\u2022 Limit one per transaction.\r\n " +
                    "\u2022 Not to be combined with any other offers.\r\n" +
                    "\u2022 Expires 30 days after redemption.\r\n" +
                    "\u2022 Must present voucher to receive redemption.";
            }


            var supplierUser = new ProfileService().GetMyProfile(membershipUser.ProviderUserKey.ToString(), membershipUser.Email);
            viewModel.PartnerLocationCurrent = string.Format("{0} {1}, {2} {3} {4} {5}",
                supplierUser.Address.StreetNumber, supplierUser.Address.StreetName, supplierUser.Address.StreetType,
                supplierUser.Address.Suburb, supplierUser.Address.State, supplierUser.Address.Postcode);


            ViewModelHelper.SetDefaultsForViewModel(viewModel, membershipUser);


            return View(viewModel);
        }

        private CreateRewardViewModel GetCreateRewardViewModelForUpdate(int id)
        {
            RewardModel model = new RewardsService().GetRewardDetails(id);

            //Current user doesn't have privileges to see this reward
            if (new Guid(LayoutViewModel.ProviderUserKey) != model.Owner_Id)
            {
                return null;
            }

            CreateRewardViewModel viewModel = new CreateRewardViewModel();

            Utils.CopyProperties(model, viewModel);

            if (model.State == (int)RewardState.Approved)
            {
                viewModel.EnableDisplayRewardOption = true;
            }
            else
            {
                viewModel.EnableDisplayRewardOption = false;
            }

            //viewModel.ProfileImages = model.ProfileImages;

            return viewModel;
        }

        [HttpPost]
        public ActionResult Create(CreateRewardViewModel model, string saveAndApproveButton, string saveAndExitButton)
        {
            LayoutViewModel.ActiveLink = Links.CreateReward;

            var membershipUser = Membership.GetUser();

            if (ModelState.IsValid)
            {
                RewardModel rewardModel = new RewardModel();

                Utils.CopyProperties(model, rewardModel);

                if (saveAndApproveButton != null)
                {
                    rewardModel.State = (int)RewardState.WaitingApproval;
                }
                if (saveAndExitButton != null) //cover cancel button too
                {
                    rewardModel.State = (int)RewardState.Incomplete;
                }
                if (model.IsUpdate)
                {
                    rewardModel.State = (int)RewardState.WaitingApproval;
                }

                if (model.LogoPhoto != null && model.LogoPhoto.ContentLength > 0)
                {
                    MemoryStream target = new MemoryStream();
                    model.LogoPhoto.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();

                    UploadModel uploadModel = new UploadModel
                    {
                        ContentType = model.LogoPhoto.ContentType,
                        Contents = data,
                        FileName = model.LogoPhoto.FileName
                    };

                    UploadModel upload = new UploadService().UploadFile(membershipUser.ProviderUserKey.ToString(), uploadModel);
                    rewardModel.ImageSmallId = upload.UploadId;
                }

                if (model.BarCodeImage != null && model.BarCodeImage.ContentLength > 0)
                {
                    MemoryStream target = new MemoryStream();
                    model.BarCodeImage.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();

                    UploadModel uploadModel = new UploadModel
                    {
                        ContentType = model.BarCodeImage.ContentType,
                        Contents = data,
                        FileName = model.BarCodeImage.FileName
                    };

                    UploadModel upload = new UploadService().UploadFile(membershipUser.ProviderUserKey.ToString(), uploadModel);
                    rewardModel.VoucherBarcodeId = upload.UploadId;
                }

                if (model.ProfileImage != null)
                {
                    List<UploadModel> profileImages = new List<UploadModel>();

                    for (var i = 0; i < 4; i++)
                    {
                        var image = model.ProfileImage[i];
                        UploadModel uploadModel = new UploadModel();

                        if (image != null)
                        {
                            MemoryStream target = new MemoryStream();
                            image.InputStream.CopyTo(target);
                            byte[] data = target.ToArray();

                            uploadModel.ContentType = image.ContentType;
                            uploadModel.Contents = data;
                            uploadModel.FileName = image.FileName;
                        }

                        profileImages.Add(uploadModel);

                    }

                    rewardModel.ProfileImages = profileImages;
                }

                //TODO s
                //public string PartnerLocationNewAddress { get; set; }
                //public string PartnerLocationNewState { get; set; }
                //public string PartnerLocationNewPostCode { get; set; }

                //public DateTime ValidFromDateTime { get; set; }
                //public DateTime ValidToDateTime { get; set; }

                //rewardModel.PartnerHours = model.PartnerHours;
                //rewardModel.PartnerUrl = model.PartnerUrl;
                //rewardModel.PartnerDescription = model.PartnerDescription;
                //rewardModel.PartnerName = model.PartnerName;
                //rewardModel.Name = model.Name;
                //rewardModel.Price = model.Price;
                //rewardModel.HowToRedeem = model.HowToRedeem;
                //rewardModel.Instructions = model.Instructions;
                //rewardModel.TermsAndConditions = model.TermsAndConditions;
                //rewardModel.Mobile = model.Mobile;
                //rewardModel.NotifyOnRedeem = model.NotifyOnRedeem;

                rewardModel.PartnerEmail = model.PartnerEmail ?? membershipUser.Email;

                if (model.ValidFrom == DateTime.MinValue)
                {
                    rewardModel.ValidFrom = null;
                }
                if (model.ValidTo == DateTime.MinValue)
                {
                    rewardModel.ValidTo = null;
                }

                int? rewardId;
                if (model.IsUpdate)
                {
                    rewardId = new RewardsService().UpdateReward((Guid)membershipUser.ProviderUserKey, rewardModel);
                }
                else
                {
                    rewardId = new RewardsService().CreateReward((Guid)membershipUser.ProviderUserKey, rewardModel);
                }

                if (rewardId != null)
                {
                    if (rewardModel.State == (int)RewardState.WaitingApproval)
                    {
                        SendRewardSubmitEmail(rewardModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes to database");

                    ViewModelHelper.SetDefaultsForViewModel(model, membershipUser);

                    return View(model);
                }

                if (saveAndApproveButton != null)
                {
                    return RedirectToAction("Create", new { id = rewardId, approve = true });
                }

                return RedirectToAction("Create", new { id = rewardId });

            }

            ViewModelHelper.SetDefaultsForViewModel(model, membershipUser);

            if (model.IsInitialRegistrationStep)
            {
                LayoutViewModel.HideTopWrapperMenu = true;
            }

            return View(model);

        }

        [NonAction]
        private void SendRewardSubmitEmail(RewardModel reward)
        {

            var category = ViewModelHelper.GetRewardCategories().FirstOrDefault(c => c.Id == reward.CategoryId);

            // email content
            var bodyContent = Razor.Parse(
                System.IO.File.ReadAllText(Server.MapPath("~/App_Data/Emails/rewardsubmit.cshtml")),
                new
                {
                    OwnerEmail = reward.PartnerEmail,
                    PartnerName = reward.PartnerName,
                    Name = reward.Name,
                    Category = category != null ? category.Name : string.Empty,
                    Site = Request.Url.Host
                }
            );

            var recipientEmail = ConfigurationManager.AppSettings["MasterAdminEmail"];
            recipientEmail = EmailService.SetRecipientEmail(recipientEmail);

            // create email request
            var request = new SendEmailRequest()
                .WithDestination(new Destination(new List<string> { recipientEmail }))
                .WithSource("no-reply@greenmoney.com.au")
                .WithReturnPath("andrew@izilla.com.au")
                .WithMessage(new Message()
                    .WithSubject(new Content("GreenMoney. Reward submitted for approval"))
                    .WithBody(new Body().WithHtml(new Content(bodyContent)))
                );

            // send it
            var client = new AmazonSimpleEmailServiceClient("AKIAIDP5FFSCJUHHC4QA", "NKAzwbtwwhvKuQZj2t6OXxOhaOEuaBYh3E34Jxbs");
            client.SendEmail(request);
        }

        public ActionResult Delete(int id)
        {
            bool deleted = new RewardsService().DeleteReward(id);

            return RedirectToAction("Index");
        }


        public ActionResult Redemptions(int page = 1)
        {
            if (page <= 0)
                page = 1;

            LayoutViewModel.ActiveLink = Links.SupplierRedemptions;

            RedemptionsViewModel viewModel = new RedemptionsViewModel();

            viewModel.RedemptionStatistics = new RewardsService().GetRedemptionsStatistics(LayoutViewModel.ProviderUserKey, page, 20);

            return View(viewModel);
        }

        public ActionResult RedemptionDetails(int id, bool thismonth = false, bool lastmonth = false, int page = 1, string filter = "")
        {
            if (page <= 0)
                page = 1;

            LayoutViewModel.ActiveLink = Links.SupplierRedemptions;

            RedemptionDetailsViewModel viewModel = new RedemptionDetailsViewModel();

            //Show last 7 days as default
            var dateToday = DateTime.Today;
            //var dateToday = DateTime.Today.AddMonths(-6); //testing

            //This week
            DateTime startDate = dateToday.AddDays(-7);
            DateTime endDate = dateToday.AddDays(1);

            //Last 28 days
            if (thismonth)
            {
                startDate = dateToday.AddDays(-28);
                endDate = dateToday.AddDays(1);
                viewModel.ReedemptionsInterval = RedemptionsInterval.ThisMonth;
            }//All ever
            else if (lastmonth)
            {
                startDate = new DateTime(2000, dateToday.Month - 1, 1);
                endDate = dateToday.AddDays(1);
                viewModel.ReedemptionsInterval = RedemptionsInterval.LastMonth;
            }

            RewardModel reward = new RewardsService().GetRewardDetails(id);

            var pageSize = 10;
            Tuple<int, List<RedemptionModel>> redemptionsTuple = new RewardsService().GetRedemptions(id, startDate, endDate, page, pageSize, filter);

            var redemptions = redemptionsTuple.Item2;

            var rewards = new RewardsService().GetSupplierRewards(new Guid(LayoutViewModel.ProviderUserKey), 1, 20);

            IEnumerable<SelectListItem> selectList =
                from c in rewards.RewardsList
                select new SelectListItem
                {
                    Selected = (c.Id == reward.Id),
                    Text = string.Format("{0} <br> {1}", c.PartnerName.Substring(0, Math.Min(c.PartnerName.Length - 1, 40)), c.Name.Substring(0, Math.Min(c.Name.Length - 1, 40))),
                    Value = c.Id.ToString()
                };


            viewModel.RewardsList = selectList;


            foreach (var redemption in redemptions)
            {
                var user = Membership.GetUser(redemption.UserId);
                redemption.Email = user != null ? user.UserName : null;
            }

            viewModel.RedemptionsModel = redemptions;
            viewModel.RewardModel = reward;
            viewModel.Page = page;
            viewModel.NumPages = (int)Math.Ceiling((double)redemptionsTuple.Item1 / pageSize);
            viewModel.NumRedemptions = redemptionsTuple.Item1;
            viewModel.Filter = filter;

            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult RedemptionDetailsHtml(int id, int interval)
        {

            var viewModel = new RedemptionDetailsViewModel();

            //Show last 7 days as default
            var dateToday = DateTime.Today;
            //var dateToday = DateTime.Today.AddMonths(-6); //testing

            DateTime endDate = dateToday.AddDays(-7);
            DateTime startDate = new DateTime(2000, dateToday.Month - 1, 1);

            //Last 28 days
            if (interval == (int)RedemptionsInterval.ThisMonth)
            {
                startDate = dateToday.AddDays(-28);
                endDate = dateToday.AddDays(1);
            } //All ever
            else if (interval == (int)RedemptionsInterval.LastMonth)
            {
                endDate = dateToday.AddDays(1);
            }
            else //Last 7 days
            {
                startDate = dateToday.AddDays(-7);
                endDate = dateToday.AddDays(1);
            }

            RewardModel reward = new RewardsService().GetRewardDetails(id);

            Tuple<int, List<RedemptionModel>> redemptionsTuple = new RewardsService().GetRedemptions(id, startDate, endDate, 1, 500, null);

            var redemptions = redemptionsTuple.Item2;

            foreach (var redemption in redemptions)
            {
                var user = Membership.GetUser(redemption.UserId);
                redemption.Email = user != null ? user.UserName : null;
            }

            viewModel.RedemptionsModel = redemptions;
            viewModel.RewardModel = reward;

            return View(viewModel);
        }

        public ActionResult DownloadRedemptionsPdf(int rewardId, int interval)
        {
            var pdfConverter = new PdfConverter();

            pdfConverter.LicenseKey = "Zk1URlVVRldTXlVGVUhWRlVXSFdUSF9fX18=";

            pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
            pdfConverter.HtmlViewerWidth = 800;

            var url = Url.Action("RedemptionDetailsHtml", "SupplierRewards",
                new RouteValueDictionary(new { id = rewardId, interval }),
                "http", Request.Url.Host);

            try
            {
                var bytes = pdfConverter.GetPdfBytesFromUrl(url);
                return File(bytes, "application/pdf");
            }
            catch (Exception)
            {
                return RedirectToAction("DownloadRedemptionsPdf", new { id = rewardId });
            }
        }

        public void DownloadCsv(int rewardId, int interval)
        {

            var endDate = DateTime.Now.AddDays(1);
            var startDate = endDate.AddYears(-1);
            var dateToday = DateTime.Today;

            //Last 28 days
            if (interval == (int)RedemptionsInterval.ThisMonth)
            {
                startDate = dateToday.AddDays(-28);
                endDate = dateToday.AddDays(1);
            }//All ever
            else if (interval == (int)RedemptionsInterval.LastMonth)
            {
                startDate = new DateTime(2000, dateToday.Month - 1, 1);
                endDate = dateToday.AddDays(1);
            }
            else //Last 7 days
            {
                startDate = dateToday.AddDays(-7);
                endDate = dateToday.AddDays(1);
            }


            var list = new RewardsService().GetRedemptions(rewardId, startDate, endDate, 1, 1000, null).Item2;

            List<RedemptionPrintModel> printList = new List<RedemptionPrintModel>();
            foreach (var item in list)
            {
                RedemptionPrintModel printModel = new RedemptionPrintModel();
                Utils.CopyProperties(item, printModel);
                printList.Add(printModel);
            }

            using (var csv = new CsvWriter(new StreamWriter(Response.OutputStream)))
            {
                csv.WriteRecords(printList);
            }

            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", "attachment; filename=vouchers-recent.csv");
            Response.End();
        }

    }

    public class RedemptionPrintModel
    {
        public string PartnerName { get; set; }
        public string Code { get; set; }
        public DateTime Issued { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public string RewardName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
    }
}
