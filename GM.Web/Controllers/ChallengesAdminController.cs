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
using GM.ViewModels.Council;
using RazorEngine;

namespace GM.Controllers
{
    public class ChallengesAdminController : SimpleApplicationController
    {
        //
        // GET: /Challenge/


        [HttpGet]
        public ActionResult Create(int? id, bool initial = false, bool copy = false)
        {
            LayoutViewModel.ActiveLink = Links.CreateChallenge;

            var membershipUser = Membership.GetUser();

            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                if (LayoutViewModel.IsCouncil)
                {

                    CreateChallengeViewModel viewModel;

                    if (id != null)
                    {
                        viewModel = GetCreateChallengeViewModelForUpdate(id.Value);

                        if (viewModel == null)
                        {
                            return RedirectToAction("Index", "ChallengesAdmin");
                        }
                        if (copy == false)
                        {
                            viewModel.IsUpdate = true;
                        }
                        else
                        {
                            viewModel.IsUpdate = false;
                        }

                        LayoutViewModel.ActiveLink = Links.Challenges;
                    }
                    else
                    {
                        viewModel = new CreateChallengeViewModel();

                        var partner = new UserService().GetUserById(LayoutViewModel.CurrentAccountId);
                        viewModel.Instance_Id = partner.Instance_Id;

                        //if (initial)
                        //{
                        //    LayoutViewModel.HideTopWrapperMenu = true;
                        //    viewModel.IsInitialRegistrationStep = true;
                        //}
                    }

                    ViewModelHelper.SetDefaultsForChallengeViewModel(viewModel, membershipUser);


                    return View(viewModel);
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Create(CreateChallengeViewModel model, string saveAndPreviewButton, string saveAndExitButton)
        {
            LayoutViewModel.ActiveLink = Links.CreateChallenge;

            var membershipUser = Membership.GetUser();

            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                if (LayoutViewModel.IsCouncil)
                {

                    if (ModelState.IsValid)
                    {
                        ChallengeModel challengeModel = new ChallengeModel();

                        Utils.CopyProperties(model, challengeModel);

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
                            challengeModel.LogoImageId = upload.UploadId;
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

                            challengeModel.ProfileImages = profileImages;
                        }


                        // rewardModel.PartnerEmail = model.PartnerEmail ?? membershipUser.Email;

                        if (model.StartDate == DateTime.MinValue)
                        {
                            challengeModel.StartDate = null;
                        }
                        if (model.EndDate == DateTime.MinValue)
                        {
                            challengeModel.EndDate = null;
                        }

                        if (model.Points == null)
                        {
                            challengeModel.Points = 0;
                        }

                        int? challengeId;
                        if (model.IsUpdate)
                        {
                            challengeId = new ChallengesAdminService().UpdateChallenge((Guid)membershipUser.ProviderUserKey, challengeModel);
                        }
                        else
                        {
                            challengeId = new ChallengesAdminService().CreateChallenge((Guid)membershipUser.ProviderUserKey, challengeModel);
                        }

                        if (challengeId != null)
                        {
                            //if (rewardModel.State == (int)RewardState.WaitingApproval)
                            //{
                            //    SendRewardSubmitEmail(rewardModel);
                            //}
                        }
                        else
                        {
                            ModelState.AddModelError("", "Unable to save changes to database");

                            ViewModelHelper.SetDefaultsForChallengeViewModel(model, membershipUser);

                            return View(model);
                        }

                        //if (saveAndApproveButton != null)
                        //{
                        //    return RedirectToAction("Create", new { id = rewardId, approve = true });
                        //}
                        if (saveAndExitButton != null)
                        {
                            return RedirectToAction("Index");
                        }
                        if (saveAndPreviewButton != null)
                        {
                            // redirect to Challenge page
                            return RedirectToAction("Details", "Challenges", new { id = challengeId });
                        }


                        return RedirectToAction("Create", new { id = challengeId });

                    }


                    ViewModelHelper.SetDefaultsForChallengeViewModel(model, membershipUser);

                    if (model.IsInitialRegistrationStep)
                    {
                        LayoutViewModel.HideTopWrapperMenu = true;
                    }

                    return View(model);
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");
        }

        private CreateChallengeViewModel GetCreateChallengeViewModelForUpdate(int id)
        {

            ChallengeModel model = new ChallengesAdminService().GetChallengeDetails(id);

            //Current user doesn't have privileges to see this reward
            //can see challenges from same instance
            if (!new ChallengesAdminService().CheckInInstance(id, new Guid(LayoutViewModel.ProviderUserKey)))
            {
                return null;
            }
            //if (new Guid(LayoutViewModel.ProviderUserKey) != model.Owner_Id)
            //{
            //    return null;
            //}

            CreateChallengeViewModel viewModel = new CreateChallengeViewModel();

            Utils.CopyProperties(model, viewModel);

            //viewModel.ProfileImages = model.ProfileImages;


            return viewModel;
        }

        public ActionResult Index(string submitBtn, string SearchString, int? SelectedStatus, int? SelectedChallengeCategory, int SelectedOrderBy = 0, int page = 1, int pageSize = 8)
        {
            if (submitBtn != null)
            {
                page = 1;
            }

            LayoutViewModel.ActiveLink = Links.Challenges;

            ChallengesIndexViewModel viewModel = new ChallengesIndexViewModel();

            var membershipUser = Membership.GetUser();

            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                if (LayoutViewModel.IsCouncil)
                {
                    ViewBag.Error = TempData["error"];

                    var challenges = new ChallengesAdminService().GetChallenges((Guid)membershipUser.ProviderUserKey, page, pageSize, SelectedOrderBy, SelectedStatus, SelectedChallengeCategory, SearchString);
                    viewModel.Instance_Id = LayoutViewModel.Instance_Id;

                    viewModel.Challenges = challenges;
                    viewModel.SearchString = SearchString;
                    viewModel.SelectedChallengeCategory = SelectedChallengeCategory;
                    viewModel.SelectedOrderBy = SelectedOrderBy;
                    viewModel.SelectedStatus = SelectedStatus;

                    ViewModelHelper.SetDefaultsForIndexChallengesViewModel(viewModel, membershipUser);
                    return View(viewModel);
                }
                else
                    return RedirectToAction("Index", "Home");
            }


            return RedirectToAction("Index", "Home");
        }



        public ActionResult Copy(int id)
        {
            ChallengeModel challengeModel = new ChallengesAdminService().CopyChallenge(id);

            return RedirectToAction("Create", new { id = challengeModel.Id });

        }

        public ActionResult Delete(int id)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                if (new ChallengesAdminService().CheckInInstance(id, new Guid(LayoutViewModel.ProviderUserKey)))
                {

                    bool deleted = new ChallengesAdminService().DeleteChallenge(id);
                    if (deleted == false)
                    {
                        TempData["error"] = "The item cannot be removed";
                        ModelState.AddModelError(string.Empty, "The item cannot be removed");
                    }
                    return RedirectToAction("Index", "ChallengesAdmin", new { id = id });
                }
                else
                    return RedirectToAction("Index");

            }
            return RedirectToAction("Index");
        }

        //public ActionResult Error()
        //{

        //}

        public ActionResult DeleteUserChallenge(int id)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                UserChallengeModel ucModel = new ChallengesAdminService().GetUserChallenge(id);
                int challengeId = ucModel.ChallengeId;
                if (new ChallengesAdminService().CheckInInstance(challengeId, new Guid(LayoutViewModel.ProviderUserKey)))
                {
                    bool deleted = new ChallengesAdminService().DeleteUserChallenge(id);
                    if (deleted == false)
                    {
                        TempData["error"] = "The item cannot be removed";
                        ModelState.AddModelError(string.Empty, "The item cannot be removed");
                    }
                    return RedirectToAction("Report", "ChallengesAdmin", new { id = challengeId });
                }
                else
                    return RedirectToAction("Index", "ChallengesAdmin");
            }
            else
                return RedirectToAction("Index", "Home");
        }


        public ActionResult MarkCompleted(int id, int challengeId)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                if (new ChallengesAdminService().CheckInInstance(challengeId, new Guid(LayoutViewModel.ProviderUserKey)))
                {
                    bool updated = new ChallengesAdminService().AddPoints(id);
                    return RedirectToAction("Report", "ChallengesAdmin", new { id = challengeId });
                }
                else
                {
                    return RedirectToAction("Index", "ChallengesAdmin");
                }

            }
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult Report(int id, int? SelectedOrderBy, int SelectedChallengeStatus = 0, string SelectedSuburb = "Suburb", string SearchString = "", int? timeRange = null, int page = 1, int pageSize = 8)
        {
            LayoutViewModel.ActiveLink = Links.Challenges;

            ChallengeReportViewModel viewModel = new ChallengeReportViewModel();

            var membershipUser = Membership.GetUser();

            bool? pointsClaimed = null;
            switch (SelectedChallengeStatus)
            {
                case 1:
                    pointsClaimed = true;
                    break;
                case 2:
                    pointsClaimed = false;
                    break;
                case 0:
                    pointsClaimed = null;
                    break;
            }


            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                bool InInstance = new ChallengesAdminService().CheckInInstance(id, (Guid)membershipUser.ProviderUserKey);
                if (LayoutViewModel.IsCouncil)
                {

                    if (InInstance)
                    {

                        var usersForChallenge = new ChallengesAdminService().GetUserChallenges(id, SelectedOrderBy, pointsClaimed, SelectedSuburb, SearchString, timeRange, page, pageSize);
                        // viewModel.UsersList.UserChallengeList = usersForChallenge.UserChallengeList;
                        viewModel.UsersList = usersForChallenge;

                        var userId = (Guid)membershipUser.ProviderUserKey;
                        viewModel.ChallengesList = new ChallengesAdminService().GetChallenges(userId, 1, 0, 0, 0, null, null);


                        viewModel.UserId = userId;
                        //  viewModel.User = new UserService().GetUserById(userId);
                        viewModel.ChallengeId = id;
                        viewModel.SelectedSuburb = SelectedSuburb;
                        viewModel.SearchString = SearchString;
                        // viewModel.SelectedChallenge = id;
                        //viewModel.UsersList.NumChallenges = usersForChallenge.NumChallenges;
                        //viewModel.UsersList.NumPages = usersForChallenge.NumPages;
                        //viewModel.UsersList.Page = usersForChallenge.Page;

                        ChallengeModel model = new ChallengesAdminService().GetChallengeDetails(id);

                        viewModel.Challenge = model;                                         

                        ViewModelHelper.SetDefaultsForChallengeReportViewModel(viewModel, membershipUser);
                        // Utils.CopyProperties(usersForChallenge, viewModel.Challenge.UsersChallenge);    
                        return View(viewModel);
                    }
                    else return RedirectToAction("Index", "ChallengesAdmin");
                }

                else return RedirectToAction("Index", "Home");

            }
            else return RedirectToAction("Index", "Home");


        }
        //public ActionResult ReportPdf(Guid id, int? challengeId)
        //{
        public ActionResult ReportPdf(int id, int? SelectedOrderBy = null, int? SelectedChallengeStatus = null, string SelectedSuburb = "Suburb", string SearchString = "", int? timeRange = null, int page = 1, int pageSize = 0)
        {
            var membershipUser = Membership.GetUser();
            bool? Status = null;
            switch (SelectedChallengeStatus)
            {
                case 1:
                    Status = true;
                    break;
                case 2:
                    Status = false;
                    break;
                case 0:
                    Status = null;
                    break;
            }
            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                bool InInstance = new ChallengesAdminService().CheckInInstance(id, (Guid)membershipUser.ProviderUserKey);
                if (LayoutViewModel.IsCouncil)
                {

                    if (InInstance)
                    {
                        var pdfConverter = new PdfConverter();

                        pdfConverter.LicenseKey = "Zk1URlVVRldTXlVGVUhWRlVXSFdUSF9fX18=";

                        pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
                        pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
                        pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
                        pdfConverter.HtmlViewerWidth = 800;


                        var url = Url.Action("ReportHtml", "ChallengesAdmin",
                            new RouteValueDictionary(new { id, SelectedChallengeStatus, Status, SelectedSuburb, SearchString, timeRange, page, pageSize }),
                            "http", Request.Url.Host);


                        try
                        {
                            var bytes = pdfConverter.GetPdfBytesFromUrl(url);
                            return File(bytes, "application/pdf");
                        }
                        catch (Exception)
                        {
                            return RedirectToAction("ReportHtml", new { id, SelectedChallengeStatus, Status, SelectedSuburb, SearchString, timeRange, page, pageSize });
                        }
                    }
                    else return RedirectToAction("Index", "ChallengesAdmin");
                }
                else return RedirectToAction("Index", "Home");
            }
            else return RedirectToAction("Index", "Home");

        }


        public ActionResult ReportHtml(int id, int? SelectedChallengeStatus = null, bool? Status = null, string SelectedSuburb = "Suburb", string SearchString = "", int? timeRange = null, int page = 1, int pageSize = 0)
        {
            UserChallengeListModel usersForChallenge = new ChallengesAdminService().GetUserChallenges(id, SelectedChallengeStatus, Status, SelectedSuburb, SearchString, timeRange, page, pageSize);
            ChallengeModel challenge = new ChallengesAdminService().GetChallengeDetails(id);


            ChallengeReportViewModel viewModel = new ChallengeReportViewModel();
            viewModel.UsersList = usersForChallenge;
            viewModel.Challenge = challenge;

            if (timeRange != 0)
                viewModel.DateRange = DateTime.Now.AddDays((double)-timeRange).ToString("dd/MM/yyyy") + " - " + DateTime.Now.ToString("dd/MM/yyyy");
            else
                viewModel.DateRange = ((DateTime)viewModel.Challenge.DateAdded).ToString("dd/MM/yyyy") + " - " + DateTime.Now.ToString("dd/MM/yyyy");

            // viewModel.UsersList.NumChallenges = usersForChallenge.NumChallenges;
            return View("ReportPdf", viewModel);
        }

        public void DownloadCsv(int id, int instance, int? SelectedChallengeStatus = null, bool? Status = null, string SelectedSuburb = "Suburb", string SearchString = "", int? timeRange = null, int page = 1, int pageSize = 0)
        {
            var list = new ChallengesAdminService().GetUserChallenges(id, SelectedChallengeStatus, Status, SelectedSuburb, SearchString, timeRange, page, pageSize);

            if (instance == 5)
            {
                List<UserChallengeCsvAusPostModel> listCsv = new List<UserChallengeCsvAusPostModel>();
                foreach (var l in list.UserChallengeList)
                {
                    UserChallengeCsvAusPostModel ucCsv = new UserChallengeCsvAusPostModel();
                    ucCsv.DateIssued = l.Issued;
                    ucCsv.Participant = l.UserName;
                    ucCsv.Workplace = l.Workplace;
                    ucCsv.Status = l.PointsClaimed ? "Completed" : "Not completed";
                    listCsv.Add(ucCsv);                    
                }

                using (var csv = new CsvWriter(new StreamWriter(Response.OutputStream)))
                {
                    csv.WriteRecords(listCsv);
                }


            }
            else
            {
                List<UserChallengeCsvModel> listCsv = new List<UserChallengeCsvModel>();
                foreach (var l in list.UserChallengeList)
                {
                    UserChallengeCsvModel ucCsv = new UserChallengeCsvModel();
                    ucCsv.DateIssued = l.Issued;
                    ucCsv.Participant = l.UserName;
                    ucCsv.Suburb = l.Suburb;
                    ucCsv.Status = l.PointsClaimed ? "Completed" : "Not completed";
                    listCsv.Add(ucCsv);
                }

                using (var csv = new CsvWriter(new StreamWriter(Response.OutputStream)))
                {
                    csv.WriteRecords(listCsv);
                }

            }
            //List<UserChallengeCsvModel> listCsv = new List<UserChallengeCsvModel>();
            //foreach (var l in list.UserChallengeList)
            //{
            //    UserChallengeCsvModel ucCsv = new UserChallengeCsvModel();
            //    ucCsv.DateIssued = l.Issued;
            //    ucCsv.Participant = l.UserName;
            //    ucCsv.Suburb = l.Suburb;
            //    ucCsv.Workplace = l.Workplace;
            //    ucCsv.Status = l.PointsClaimed ? "Completed" : "Not completed";
            //    listCsv.Add(ucCsv);
            //}

            //using (var csv = new CsvWriter(new StreamWriter(Response.OutputStream)))
            //{
            //    if (instance == 5)
            //    {
            //        csv.Configuration.ClassMapping<CustomClassMapAuspost>();
            //    }
            //    else
            //    {
            //        csv.Configuration.ClassMapping<CustomClassMap>();
            //    }

            //    csv.WriteRecords(listCsv);
            //}

            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", "attachment; filename=challenges.csv");
            Response.End();
        }
    }


}
