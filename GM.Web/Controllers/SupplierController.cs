using System.Configuration;
using System.Web.Configuration;
using System.Web.Routing;
using GM.BusinessLogic;
using GM.Models.Public;
using GM.Utility;
using GM.ViewModels;
using GM.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GM.ViewModels.Supplier;

namespace GM.Controllers
{
    [Authorize]
    public class SupplierController : SimpleApplicationController
    {

        // GET: /Supplier/
        // Update profile
        public ActionResult Index()
        {
            LayoutViewModel.ActiveLink = Links.SupplierUpdateProfile;

            LayoutViewModel.ActiveLink = Links.AccountUpdateProfile;

            var userModel = new UserService().GetUserById(new Guid(LayoutViewModel.ProviderUserKey));

            bool isDefaultSupplierAddress;
            UpdateSupplierProfileViewModel viewModel = new UpdateSupplierProfileViewModel
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                PhotoID = userModel.PhotoID,
                BusinessName = userModel.BusinessName,
                BusinessNumberABN = userModel.BusinessNumberABN,
                BusinessType = userModel.BusinessType,
                BussinesEmail = LayoutViewModel.CurrentUserEmail,//userModel.BussinesEmail,
                BussinesLocation = userModel.BussinesLocation,
                BussinesPhone = userModel.BussinesPhone,
                BussinesPhoneArea = userModel.BussinesPhoneArea,
                BussinesWebSite = userModel.BussinesWebSite,
                BussinesPhoneMobile = userModel.BussinesPhoneMobile,
                EmailBussinesOnVoucherRedeem = userModel.EmailBussinesOnVoucherRedeem,
                SendEmailUpdates = userModel.SendEmailUpdates,
                Address = GetSupplierAddress(userModel.AddressModel, out isDefaultSupplierAddress),
                IsDefaultSupplierAddress = isDefaultSupplierAddress
            };

            return View(viewModel);
        }

        // Update profile
        [HttpPost]
        public ActionResult Index(UpdateSupplierProfileViewModel viewModel)
        {
            LayoutViewModel.ActiveLink = Links.SupplierUpdateProfile;

            var membershipUser = Membership.GetUser();

            if (ModelState.IsValid)
            {
                UserModel userModel = new UserModel
                {
                    Id = LayoutViewModel.CurrentAccountId,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    PhotoID = viewModel.PhotoID,
                    BusinessName = viewModel.BusinessName,
                    BusinessNumberABN = viewModel.BusinessNumberABN,
                    BusinessType = viewModel.BusinessType,
                    BussinesEmail = viewModel.BussinesEmail,
                    BussinesLocation = viewModel.BussinesLocation,
                    BussinesPhone = viewModel.BussinesPhone,
                    BussinesPhoneArea = viewModel.BussinesPhoneArea,
                    BussinesPhoneMobile = viewModel.BussinesPhoneMobile,
                    BussinesWebSite = viewModel.BussinesWebSite,
                    EmailBussinesOnVoucherRedeem = viewModel.EmailBussinesOnVoucherRedeem,
                    SendEmailUpdates = viewModel.SendEmailUpdates
                };

                var success = true;

                // change password
                if (!string.IsNullOrEmpty(viewModel.OldPassword) || !string.IsNullOrEmpty(viewModel.NewPassword)
                    || !string.IsNullOrEmpty(viewModel.ConfirmNewPassword))
                {
                    if (string.IsNullOrEmpty(viewModel.OldPassword))
                    {
                        ModelState.AddModelError("OldPassword", "The Old password field is required.");
                        success = false;
                    }

                    if (string.IsNullOrEmpty(viewModel.NewPassword))
                    {
                        ModelState.AddModelError("NewPassword", "The New password field is required.");
                        success = false;
                    }

                    if (string.IsNullOrEmpty(viewModel.ConfirmNewPassword))
                    {
                        ModelState.AddModelError("ConfirmNewPassword", "The Confirm new password field is required.");
                        success = false;
                    }

                    if (success && viewModel.NewPassword != viewModel.ConfirmNewPassword)
                    {
                        ModelState.AddModelError("ConfirmNewPassword", "New password and confirmation password do not match.");
                        success = false;
                    }

                    if (success && !membershipUser.ChangePassword(viewModel.OldPassword, viewModel.NewPassword))
                    {
                        ModelState.AddModelError("OldPassword", "Old password is incorrect.");
                        success = false;
                    }
                }


                if (viewModel.Photo != null && viewModel.Photo.ContentLength > 0)
                {
                    MemoryStream target = new MemoryStream();
                    viewModel.Photo.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();

                    UploadModel model = new UploadModel
                    {
                        ContentType = viewModel.Photo.ContentType,
                        Contents = data,
                        FileName = viewModel.Photo.FileName
                    };

                    UploadModel upload = new UploadService().UploadFile(membershipUser.ProviderUserKey.ToString(), model, true);

                }

                if (success)
                {
                    var updated = new UserService().UpdateSupplierUser(userModel);

                    // change username COMPLICATED
                    if (membershipUser.UserName != viewModel.BussinesEmail)
                    {

                        var config = WebConfigurationManager.OpenWebConfiguration("~");
                        var section = config.SectionGroups["system.web"].Sections["membership"] as MembershipSection;
                        var defaultProvider = section.DefaultProvider;
                        var connectionStringName = section.Providers[defaultProvider].ElementInformation.Properties["connectionStringName"].Value.ToString();

                        string connectionString = config.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;


                        var changed = new ProfileService().ChangeUsername(membershipUser.UserName, viewModel.BussinesEmail, connectionString);

                        if (changed)
                        {
                            // change email as well
                            membershipUser.Email = viewModel.BussinesEmail;

                            // need to re-verify
                            membershipUser.IsApproved = false;
                            //SendVerifyEmail(membershipUser, user, false);
                            //instead I'm showing verifucation code in the view

                            Membership.UpdateUser(membershipUser);

                            // need to sign out to force verification
                            FormsAuthentication.SignOut();

                            TempData["VerifyCode"] = ZBase32.Encode(LayoutViewModel.CurrentAccountId.ToByteArray());

                            // redirect to screen which tells user to check email
                            return RedirectToAction("EmailChangeSuccess", "Account");
                        }
                        else
                        {
                            ModelState.AddModelError("", "A user for that email address already exists. Please enter a different email address.");
                        }
                    }

                    return RedirectToAction("Dashboard");
                }

                return View(viewModel);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult CompleteProfile()
        {
            LayoutViewModel.ActiveLink = Links.SupplierUpdateProfile;

            LayoutViewModel.HideTopWrapperMenu = true;

            var userModel = new UserService().GetUserById(new Guid(LayoutViewModel.ProviderUserKey));

            CompleteSupplierProfileViewModel viewModel = new CompleteSupplierProfileViewModel
            {
                BusinessNumberABN = userModel.BusinessNumberABN,
                BusinessType = userModel.BusinessType,
                BussinesWebSite = userModel.BussinesWebSite
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CompleteProfile(CompleteSupplierProfileViewModel viewModel)
        {
            LayoutViewModel.ActiveLink = Links.SupplierUpdateProfile;

            LayoutViewModel.HideTopWrapperMenu = true;

            var userModel = new UserModel
            {
                Id = LayoutViewModel.CurrentAccountId,
                BusinessNumberABN = viewModel.BusinessNumberABN,
                BusinessType = viewModel.BusinessType,
                BussinesWebSite = viewModel.BussinesWebSite
            };

            var updated = new UserService().UpdateToCompleteSupplierUser(userModel);

            return RedirectToAction("CreateReward");
        }

        public ActionResult CreateReward()
        {
            return RedirectToAction("Create", "SupplierRewards", new {initial = true});
        }

        public ActionResult Dashboard(int page = 1)
        {
            LayoutViewModel.ActiveLink = Links.SupplierDashboard;

            ProfileModel profileModel =
                new ProfileService().GetMyProfile(
                LayoutViewModel.ProviderUserKey, LayoutViewModel.CurrentUserEmail, page);
 
            DashboardViewModel viewModel = new DashboardViewModel();
            viewModel.ProfileModel = profileModel;
            viewModel.RewardsSummaryListModel = new RewardsService().GetSupplierRewardsSummary(new Guid(LayoutViewModel.ProviderUserKey), page, 5);
            viewModel.ActivitySummaryModel = new RewardsService().GetActivitySummaryModelModel(LayoutViewModel.ProviderUserKey);
            viewModel.HouseholdMemberModel = new HouseholdMemberModel();

            bool isDefaultSupplierAddress;
            viewModel.Address = GetSupplierAddress(profileModel.Address, out isDefaultSupplierAddress);

            return View(viewModel);
        }

        private string GetSupplierAddress(AddressModel addressModel, out bool isDefaultSupplierAddress)
        {
            isDefaultSupplierAddress = false;
            //Address ID given to all suppliers after decision not to forse suppliers to enter address
            var supplierAddressDefault = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultSupplierAddressId"]);

            if (addressModel.Id == supplierAddressDefault)
            {
                isDefaultSupplierAddress = true;
            }

            return isDefaultSupplierAddress
                ? "Address not entered"
                : ViewModelHelper.GetUserAddress(addressModel);
        }
    }
}