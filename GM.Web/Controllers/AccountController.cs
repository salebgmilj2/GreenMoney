using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Amazon.SimpleEmail;
using evointernal;
using GM.BusinessLogic;
using GM.Models;
using GM.Models.Public;
using GM.Utility;
using GM.Utility.EmailService;
using GM.ViewModels;
using GM.ViewModels.Shared;
using MailChimp;
using MailChimp.Lists;
using MailChimp.Helper;
using Amazon.SimpleEmail.Model;
using RazorEngine;

namespace GM.Controllers
{
    [Authorize]
    public class AccountController : SimpleApplicationController
    {
        public ActionResult Index()
        {
            return RedirectToAction("MyProfile");
        }

        public ActionResult MyProfile(int page = 1)
        {

            if (LayoutViewModel.IsSuplier)
            {
                return RedirectToAction("Dashboard", "Supplier");
            }

            if (LayoutViewModel.IsCouncil)
            {
                return RedirectToAction("Index", "ChallengesAdmin");
            }

            LayoutViewModel.ActiveLink = Links.AccountMyProfile;

            //Code to change user password from the code
            //string username = membershipUser.UserName;
            //string password = "newpassword";
            //MembershipUser mu = Membership.GetUser(username);
            //mu.ChangePassword(mu.ResetPassword(), password);


            ProfileModel profileModel =
                new ProfileService().GetMyProfile(
                LayoutViewModel.ProviderUserKey, LayoutViewModel.CurrentUserEmail, page);

            MyProfileViewModel viewModel = new MyProfileViewModel();
            viewModel.ProfileModel = profileModel;
            viewModel.EmailAddress = LayoutViewModel.CurrentUserEmail; //TODO - fix this
            viewModel.HouseholdMemberModel = new HouseholdMemberModel();

            if (profileModel.User.DateOfBirth != null)
            {
                viewModel.BirthDate = profileModel.User.DateOfBirth.Value.ToString("dd MMM yyyy");
            }


            //Auspost specific
            if (LayoutViewModel.IsAusPost)
            {
                if (profileModel.User.DateOfBirth != null)
                {
                    viewModel.BirthDate = profileModel.User.DateOfBirth.Value.ToString("dd MMM yyyy");

                    var numOfYears = DateTime.Today.Year - profileModel.User.DateOfBirth.Value.Year;
                    const int interval = 8;

                    string range = CheckValueInRange(0, 17, numOfYears)
                        ? "< 18"
                        : (CheckValueInRange(18, interval, numOfYears)
                            ? "18-25"
                            : (CheckValueInRange(26, interval, numOfYears)
                                ? "26-33"
                                : (CheckValueInRange(34, interval, numOfYears)
                                    ? "34-41"
                                    : (CheckValueInRange(42, interval, numOfYears)
                                        ? "42-49"
                                        : (CheckValueInRange(50, interval, numOfYears)
                                             ? "50-57"
                                             : (CheckValueInRange(58, interval, numOfYears)
                                                ? "58-65"
                                                : (CheckValueInRange(66, interval, numOfYears)
                                                    ? "66-73"
                                                    : "74 >"
                                    )))))));

                    viewModel.NumberOfYears = String.Format("{0} years", range);
                }

                viewModel.NumberOfChallenges = new ChallengesService().GetNumberOfMyChallenges(LayoutViewModel.ProviderUserKey, DateTime.Today.AddMonths(-12));

                return View("MyProfileAuspost", viewModel);
            }

            viewModel.AddressFull = ViewModelHelper.GetUserAddress(profileModel.Address);

            return View(viewModel);
        }


        public static bool CheckValueInRange(int start, int end, int valueToTest)
        {
            // check value in range...
            bool ValueInRange = Enumerable.Range(start, end).Contains(valueToTest);
            // return value...
            return ValueInRange;
        }


        [HttpGet]
        public ActionResult UpdateProfile()
        {
            LayoutViewModel.ActiveLink = Links.AccountUpdateProfile;

            var profileModel = new ProfileService().GetMyProfile(LayoutViewModel.ProviderUserKey, LayoutViewModel.CurrentUserEmail, 1);

            UpdateProfileViewModel viewModel = new UpdateProfileViewModel
            {
                FirstName = profileModel.User.FirstName,
                LastName = profileModel.User.LastName,
                PhoneNumber = profileModel.User.PhoneNumber,
                Sex = profileModel.User.Sex,
                Email = LayoutViewModel.CurrentUserEmail,
                PhotoId = profileModel.User.PhotoID,
                Address = ViewModelHelper.GetUserAddress(profileModel.Address),
                SupportEmail = ConfigurationManager.AppSettings["SupportEmail"],
                SendEmailUpdates = profileModel.User.SendEmailUpdates,
                PushNotifications = profileModel.User.PushNotifications,
                PostToFacebook = profileModel.User.PostToFacebook
            };


            if (profileModel.User.DateOfBirth.HasValue)
            {
                DateTime date = profileModel.User.DateOfBirth.Value;
                viewModel.DateOfBirthDay = date.Day;
                viewModel.DateOfBirthMonth = date.Month;
                //viewModel.DateOfBirthMonthText =
                //    viewModel.Months.Where(m => int.Parse(m.Value) == date.Month).First().Text;

                viewModel.DateOfBirthYear = date.Year;
            }

            //Auspost
            if (LayoutViewModel.IsAusPost)
            {
                return RedirectToAction("UpdateAuspost");
            }

            return View(viewModel);

        }

        [HttpPost]
        public ActionResult UpdateProfile(UpdateProfileViewModel viewModel)
        {
            LayoutViewModel.ActiveLink = Links.AccountUpdateProfile;

            var membershipUser = Membership.GetUser();

            if (ModelState.IsValid)
            {
                UpdateProfileModel updateModel = new UpdateProfileModel();

                updateModel.FirstName = viewModel.FirstName;
                updateModel.LastName = viewModel.LastName;
                updateModel.PhoneNumber = viewModel.PhoneNumber;
                updateModel.Sex = viewModel.Sex;
                updateModel.SendEmailUpdates = viewModel.SendEmailUpdates;
                updateModel.PushNotifications = viewModel.PushNotifications;
                updateModel.PostToFacebook = viewModel.PostToFacebook;

                if (viewModel.DateOfBirthDay != null && viewModel.DateOfBirthMonth != null && viewModel.DateOfBirthYear != null)
                {
                    updateModel.BirthDate =
                        new DateTime(year: viewModel.DateOfBirthYear.Value,
                            month: viewModel.DateOfBirthMonth.Value,
                            day: viewModel.DateOfBirthDay.Value);
                }


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

                    UploadModel upload = new UploadService().UploadFile(LayoutViewModel.ProviderUserKey, model, true);

                }

                if (success)
                {

                    var updated = new ProfileService().UpdateProfile(LayoutViewModel.ProviderUserKey, updateModel);

                    // change username COMPLICATED
                    if (membershipUser.UserName != viewModel.Email)
                    {
                         
                        var config = WebConfigurationManager.OpenWebConfiguration("~");
                        var section = config.SectionGroups["system.web"].Sections["membership"] as MembershipSection;
                        var defaultProvider = section.DefaultProvider;
                        var connectionStringName = section.Providers[defaultProvider].ElementInformation.Properties["connectionStringName"].Value.ToString();
                        
                        string connectionString = config.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;


                        var changed = new ProfileService().ChangeUsername(membershipUser.UserName, viewModel.Email, connectionString);

                        if (changed)
                        {
                            // change email as well
                            membershipUser.Email = viewModel.Email;

                            // need to re-verify
                            membershipUser.IsApproved = false;
                            SendVerifyEmail(membershipUser.Email, updateModel.FirstName, LayoutViewModel.CurrentAccountId, false);
                            
                            //instead I'm showing verifucation code in the view
                            Membership.UpdateUser(membershipUser);

                            // need to sign out to force verification
                            FormsAuthentication.SignOut();

                            TempData["VerifyCode"] = ZBase32.Encode(LayoutViewModel.CurrentAccountId.ToByteArray());       

                            // redirect to screen which tells user to check email
                            return RedirectToAction("EmailChangeSuccess");
                        }
                        else
                        {
                            ModelState.AddModelError("", "A user for that email address already exists. Please enter a different email address.");
                        }
                    }


                    return RedirectToAction("MyProfile");

                }

                //Something not valid, show errors
                viewModel.SupportEmail = ConfigurationManager.AppSettings["SupportEmail"];

                return View(viewModel);
            }

            //Something not valid, show errors
            viewModel.SupportEmail = ConfigurationManager.AppSettings["SupportEmail"];
            
            return View(viewModel);

        }


        //Auspost -> id = 5
        [HttpGet]
        public ActionResult UpdateAuspost()
        {
            LayoutViewModel.ActiveLink = Links.AccountUpdateProfile;

            var profileModel = new ProfileService().GetMyProfile(LayoutViewModel.ProviderUserKey,
                LayoutViewModel.CurrentUserEmail, 1);

            UpdateProfileAuspostViewModel viewModelCompany = new UpdateProfileAuspostViewModel
            {
                FirstName = profileModel.User.FirstName,
                LastName = profileModel.User.LastName,
                PhoneNumber = profileModel.User.PhoneNumber,
                Sex = profileModel.User.Sex,
                Email = LayoutViewModel.CurrentUserEmail,
                CompanyEmail = LayoutViewModel.CurrentUserEmail.Split('@')[0],
                CompanyEmailDomain = string.Format("@{0}", LayoutViewModel.CurrentUserEmail.Split('@')[1]),
                PhotoId = profileModel.User.PhotoID,
                Address = ViewModelHelper.GetUserAddress(profileModel.Address),
                SupportEmail = ConfigurationManager.AppSettings["SupportEmail"],
                PushNotifications = profileModel.User.PushNotifications
            };

            //Private company - auspost
            viewModelCompany.EmploymentType = profileModel.User.EmploymentType;


            if (profileModel.User.DateOfBirth.HasValue)
            {
                DateTime date = profileModel.User.DateOfBirth.Value;
                viewModelCompany.DateOfBirthDay = date.Day;
                viewModelCompany.DateOfBirthMonth = date.Month;
                viewModelCompany.DateOfBirthYear = date.Year;
            }


            var streetTypes = new UserService().GetAllStreetTypes((int)LayoutViewModel.Instance_Id);
            var streetNames = new UserService().GetAllStreetNames((int)LayoutViewModel.Instance_Id);
            var suburbs = new UserService().GetAllSuburbs((int)LayoutViewModel.Instance_Id);
            var states = new UserService().GetAllStates((int)LayoutViewModel.Instance_Id);

            viewModelCompany.StreetTypes = new SelectList(streetTypes);
            viewModelCompany.StreetNames = new SelectList(streetNames);
            viewModelCompany.Subrps = new SelectList(suburbs);
            viewModelCompany.States = new SelectList(states);

            viewModelCompany.StreetType = profileModel.Address.StreetType;
            viewModelCompany.StreetName = profileModel.Address.StreetName;
            viewModelCompany.Subrp = profileModel.Address.Suburb;
            viewModelCompany.State = profileModel.Address.State;

            return View("UpdateProfileAuspost", viewModelCompany);

        }

        [HttpPost]
        public ActionResult UpdateProfileAuspost(UpdateProfileAuspostViewModel viewModel)
        {
            LayoutViewModel.ActiveLink = Links.AccountUpdateProfile;

            var membershipUser = Membership.GetUser();

            if (ModelState.IsValid)
            {
                UpdateProfileModel updateModel = new UpdateProfileModel();

                updateModel.FirstName = viewModel.FirstName;
                updateModel.LastName = viewModel.LastName;
                updateModel.PhoneNumber = viewModel.PhoneNumber;
                updateModel.Sex = viewModel.Sex;
                updateModel.PushNotifications = viewModel.PushNotifications;
                updateModel.EmploymentType = viewModel.EmploymentType;

                if (viewModel.DateOfBirthDay != null && viewModel.DateOfBirthMonth != null && viewModel.DateOfBirthYear != null)
                {
                    updateModel.BirthDate =
                        new DateTime(year: viewModel.DateOfBirthYear.Value,
                            month: viewModel.DateOfBirthMonth.Value,
                            day: viewModel.DateOfBirthDay.Value);
                }

                // find new user's address
                //int? addressId = new UserService().FindMatchingAuspostAddressId(
                //    streetName: viewModel.StreetName,
                //    streetType: viewModel.StreetType,
                //    suburb: viewModel.Subrp,
                //    state: viewModel.State
                //    );

                //if (addressId != null)
                //{
                //    updateModel.AddressId = addressId;
                //}
                //else
                //{
                //    FillBaseViewModelData(viewModel);

                //    ModelState.AddModelError("", "Incomplete workplace details. Please update your details and save.");
                //    return View(viewModel);
                //}

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

                    UploadModel upload = new UploadService().UploadFile(LayoutViewModel.ProviderUserKey, model, true);

                }

                if (success)
                {

                    var updated =
                        new ProfileService().UpdateProfile(LayoutViewModel.ProviderUserKey, updateModel);


                    // change username COMPLICATED
                    if (membershipUser.UserName != viewModel.Email)
                    {

                        var config = WebConfigurationManager.OpenWebConfiguration("~");
                        var section = config.SectionGroups["system.web"].Sections["membership"] as MembershipSection;
                        var defaultProvider = section.DefaultProvider;
                        var connectionStringName = section.Providers[defaultProvider].ElementInformation.Properties["connectionStringName"].Value.ToString();

                        string connectionString = config.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;

                        string companyEmail = string.Join(viewModel.CompanyEmail, viewModel.CompanyEmailDomain);

                        var changed = new ProfileService().ChangeUsername(membershipUser.UserName, companyEmail, connectionString);

                        if (changed)
                        {
                            // change email as well
                            membershipUser.Email = companyEmail;

                            // need to re-verify
                            membershipUser.IsApproved = false;
                            SendVerifyEmail(membershipUser.Email, updateModel.FirstName, LayoutViewModel.CurrentAccountId, false);

                            //instead I'm showing verifucation code in the view
                            Membership.UpdateUser(membershipUser);

                            // need to sign out to force verification
                            FormsAuthentication.SignOut();

                            TempData["VerifyCode"] = ZBase32.Encode(LayoutViewModel.CurrentAccountId.ToByteArray());

                            // redirect to screen which tells user to check email
                            return RedirectToAction("EmailChangeSuccess");
                        }
                        else
                        {
                            ModelState.AddModelError("", "A user for that email address already exists. Please enter a different email address.");
                        }
                    }


                    return RedirectToAction("MyProfile");

                }

                //Something not valid, show errors
                viewModel.SupportEmail = ConfigurationManager.AppSettings["SupportEmail"];

                return View(viewModel);
            }

            FillBaseViewModelData(viewModel);

            return View(viewModel);

        }

        private static void FillBaseViewModelData(UpdateProfileAuspostViewModel viewModel)
        {
            var privateClientId = ConfigurationManager.AppSettings["PrivateClientId"] == null
                ? 5
                : int.Parse(ConfigurationManager.AppSettings["PrivateClientId"]);

            var streetTypes = new UserService().GetAllStreetTypes(privateClientId);
            var streetNames = new UserService().GetAllStreetNames(privateClientId);
            var suburbs = new UserService().GetAllSuburbs(privateClientId);
            var states = new UserService().GetAllStates(privateClientId);

            viewModel.StreetTypes = new SelectList(streetTypes);
            viewModel.StreetNames = new SelectList(streetNames);
            viewModel.Subrps = new SelectList(suburbs);
            viewModel.States = new SelectList(states);

            //Something not valid, show errors
            viewModel.SupportEmail = ConfigurationManager.AppSettings["SupportEmail"];
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult EmailChangeSuccess()
        {
            var temp = TempData["VerifyCode"];
            TempData["VerifyCode"] = TempData["VerifyCode"];

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Verify(string code)
        {
            if (code != null)
            {
                if (code == null)
                    throw new ArgumentNullException("code");

                var id =  new Guid(ZBase32.Decode(code));
                var user = Membership.GetUser(id);

                if (user != null)
                {
                    // approve user
                    user.IsApproved = true;
                    Membership.UpdateUser(user);
                     
                    return RedirectToAction("Login", new { verified = true });
                }
                else
                {
                    //TODO - ModelState.AddModelError("", "Validation code is incorrect.");
                    return RedirectToAction("Login");

                }
            }

            //TODO - ModelState.AddModelError("", "Something went wrong...");
            return RedirectToAction("Login");
        }

        // GET: /Account/ForgotPassword

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordViewModel viewModel = new ForgotPasswordViewModel();
            return View(viewModel);
        }

        //
        // POST: /Account/ForgotPassword

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            var membershipUser = Membership.GetUser(model.Email);

            if (membershipUser != null)
            {
                var user = new UserService().GetUserById((Guid)membershipUser.ProviderUserKey);

                // generate password
                const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";

                var sb = new StringBuilder();
                var rand = new Random();
                for (var i = 0; i < 10; i++)
                    sb.Append(chars[rand.Next(chars.Length)]);

                var password = sb.ToString();

                // reset password
                var old = membershipUser.ResetPassword();
                membershipUser.ChangePassword(old, password);
                Membership.UpdateUser(membershipUser);

                // create login url
                var url = Url.Action("Login", "Account", null, Request.Url.Scheme);
                var emailContentPath = Server.MapPath("~/App_Data/Emails/reset-password.cshtml");
                EmailService.SendEmailResetPassword(emailContentPath, user, password, url, membershipUser);


                return RedirectToAction("ForgotPasswordSuccess");
            }
            else
            {
                ModelState.AddModelError("", "No user with that email exists.");
            }

            // something failed
            return View(model);
        }

        // GET: /Account/ForgotPasswordSuccess
		public ActionResult ForgotPasswordSuccess()
		{
            return View();
        }

        [HttpGet]
        public ActionResult Inbox()
        {
            LayoutViewModel.ActiveLink = Links.AccountInbox;

            return View();
        }
 

        [HttpGet]
        public ActionResult Cart()
        {
            LayoutViewModel.ActiveLink = Links.AccountMyCart;

            CartModel cartModel = new ProfileService().GetMyCart(LayoutViewModel.ProviderUserKey);

            MyCartViewModel viewModel = new MyCartViewModel();
            viewModel.CartModel = cartModel;


            return View(viewModel);
        }

        //Update cart (update quantity or remove reward)
        [HttpPost]
        public ActionResult Cart(MyCartViewModel myCartViewModel)
        {
            LayoutViewModel.ActiveLink = Links.AccountMyCart;

            bool updated = new ProfileService().UpdateMyCart(LayoutViewModel.ProviderUserKey, myCartViewModel.CartModel.Items);

            return RedirectToAction("Cart");
        }

        //[Authorize]
        [HttpPost]
        public bool CartToAdd(int id, int quantity = 1)
        {

            bool rewardAddedToCart = new ProfileService().AddToMyCart(LayoutViewModel.ProviderUserKey, id);

            return rewardAddedToCart;
        }

        [Authorize]
        public ActionResult Checkout(bool failure = false)
        {
            LayoutViewModel.ActiveLink = Links.AccountMyCart;

            CartModel cartModel = new ProfileService().GetMyCart(LayoutViewModel.ProviderUserKey);

            MyCartCheckoutViewModel viewModel = new MyCartCheckoutViewModel();
            viewModel.CartModel = cartModel;
            viewModel.TotalQuantity = cartModel.Items.Sum(i => i.Quantity).ToString();
            viewModel.TotalPoints = cartModel.CartTotal.ToString("#,##0");
            viewModel.UserEmail = LayoutViewModel.CurrentUserEmail;

            if (failure)
            {
                viewModel.CheckoutFailure = true;
            }

            return View(viewModel);
        }

        // Checkout submit
        [Authorize]
        [HttpPost, ActionName("Checkout")]
        public ActionResult CheckoutSubmit()
        {
            LayoutViewModel.ActiveLink = Links.AccountMyCart;

            CheckoutSubmitModel addToMyWallet = new ProfileService().AddCartToMyWallet(LayoutViewModel.ProviderUserKey);

            // Send email to reward owner user
            // + Send email to current user
            string url = Request.Url.Host;
            var voucherUrlBase = Url.Action("Voucher", "Rewards", null, Request.Url.Scheme);
            string contentPath = Server.MapPath("~/App_Data/Emails/reward-redeemed.cshtml");
            string contentPath1 = Server.MapPath("~/App_Data/Emails/product-order-confirmation.cshtml");
            string contentPath2 = Server.MapPath("~/App_Data/Emails/order-confirmation.cshtml");
            EmailService.SendEmailsOnRewardsRedeem(contentPath, contentPath1, contentPath2, 
                LayoutViewModel.CurrentUserEmail, LayoutViewModel.CurrentFirstName, LayoutViewModel.CurrentLastName,
                url, voucherUrlBase, addToMyWallet);
  
 
            if (addToMyWallet.CheckoutSubmitModelState == CheckoutSubmitModelState.NoItemsFound)
            {
                return RedirectToAction("Checkout");
            }

            if (addToMyWallet.CheckoutSubmitModelState == CheckoutSubmitModelState.NotEnoughPoints)
            {
                return RedirectToAction("Checkout", new {failure = true});
            }

            if (addToMyWallet.CheckoutSubmitModelState == CheckoutSubmitModelState.SuccessWithOrderConfirmation)
            {
                // TODO Send Order Confirmation email
                return RedirectToAction("Wallet");
            }

            if (addToMyWallet.CheckoutSubmitModelState == CheckoutSubmitModelState.SuccessWithProductOrderConfirmation)
            {
                // TODO Send Product Order Confirmation email
                return RedirectToAction("Wallet");
            }

            return RedirectToAction("Wallet");
        }

        [HttpGet]
        public ActionResult Wallet(bool? challenges)
        {
            LayoutViewModel.ActiveLink = Links.AccountMyWallet;

            MyWalletViewModel viewModel = new MyWalletViewModel();
            if (challenges == null)
            {
                viewModel.CartModel = new ProfileService().GetMyWallet(LayoutViewModel.ProviderUserKey);
                return View(viewModel);
            }
            else
            {
                viewModel.ShowChallengesView = true;
                viewModel.CartModel = new ChallengesService().GetMyChallenges(LayoutViewModel.ProviderUserKey);
                return View("Challenges", viewModel);
            }
        }

        [HttpGet]
        public ActionResult AdditionalUsers()
        {
            LayoutViewModel.ActiveLink = Links.AccountAdditionalUsers;

            AdditionalAccountsViewModel viewModel = new AdditionalAccountsViewModel();
            List<UserModel> list = new UserService().GetAdditionalAccounts(LayoutViewModel.ProviderUserKey);
            viewModel.AdditionalAccounts = list;

            List<UserModel> invitedUsers = new UserService().GetInvitedUsersAccounts(LayoutViewModel.ProviderUserKey);

            invitedUsers = invitedUsers.Where(x => !list.Select(l=>l.Email).Contains(x.Email)).ToList();

            viewModel.InvitedUsersAccounts = invitedUsers;

            var currentUser = new UserService().GetUserById(new Guid(LayoutViewModel.ProviderUserKey));

            viewModel.HouseholdMemberModel = new HouseholdMemberModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AdditionalUsers(HouseholdMemberModel householdMemberModel)
        {
            LayoutViewModel.ActiveLink = Links.AccountAdditionalUsers;

            //TODO check model state etc.
            var currentUser = new UserService().GetUserById(new Guid(LayoutViewModel.ProviderUserKey));

            var isValid = !string.IsNullOrWhiteSpace(householdMemberModel.Email) &&
                          !string.IsNullOrWhiteSpace(householdMemberModel.ConfirmEmail) &&
                          !string.IsNullOrWhiteSpace(householdMemberModel.FirstName) &&
                          !string.IsNullOrWhiteSpace(householdMemberModel.LastName);

            if (isValid)
            {

                // create invite additional members url
                var url = Url.Action("RegisterHouseholdMembers", "Account", new
                {
                    emailaddress = householdMemberModel.Email,
                    inviter = currentUser.FirstName,
                    addressid = currentUser.AddressModel.Id,
                    firstname = householdMemberModel.FirstName,
                    lastname = householdMemberModel.LastName
                }, Request.Url.Scheme);


                // email content

                var userModel = new InviteUserModel
                {
                    Email = householdMemberModel.Email,
                    FirstName = householdMemberModel.FirstName,
                    LastName = householdMemberModel.LastName,
                    User_Id = currentUser.Id
                };

                // Give them the Invite Member Bonus 
                int numBonusPoints = Convert.ToInt16(ConfigurationManager.AppSettings["BonusPoints.InviteMember.Points"]);
                string descBonusPoints = Convert.ToString(ConfigurationManager.AppSettings["BonusPoints.InviteMember.Description"]);
                int? transactionTypeId = Convert.ToInt16(ConfigurationManager.AppSettings["TransactionType.ShareHeart"]);

                InviteUserModel invited = new UserService().InviteHouseholdMembers(userModel, numBonusPoints, descBonusPoints, transactionTypeId);

                //Send email to invite members to register
                EmailService.SendEmailAdditionalMemberInvitation(householdMemberModel.Email, householdMemberModel.InviterName, url);

            }

            return RedirectToAction("AdditionalUsers");
        }

        [HttpPost]
        public bool AdditionalUsersAjax(string email, string firstname, string lastname)
        {
            //TODO check model state etc.
            var currentUser = new UserService().GetUserById(new Guid(LayoutViewModel.ProviderUserKey));

            var isValid = !string.IsNullOrWhiteSpace(email) &&
                          !string.IsNullOrWhiteSpace(firstname) &&
                          !string.IsNullOrWhiteSpace(lastname);

            if (isValid)
            {

                // create invite additional members url
                var url = Url.Action("RegisterHouseholdMembers", "Account", new
                {
                    emailaddress = email,
                    inviter = currentUser.FirstName,
                    addressid = currentUser.AddressModel.Id,
                    firstname = firstname,
                    lastname = lastname
                }, Request.Url.Scheme);


                // email content

                var userModel = new InviteUserModel
                {
                    Email = email,
                    FirstName = firstname,
                    LastName = lastname,
                    User_Id = currentUser.Id
                };

                int numBonusPoints = Convert.ToInt16(ConfigurationManager.AppSettings["BonusPoints.InviteMember.Points"]);
                string descBonusPoints = Convert.ToString(ConfigurationManager.AppSettings["BonusPoints.InviteMember.Description"]);
                int? transactionTypeId = Convert.ToInt16(ConfigurationManager.AppSettings["TransactionType.ShareHeart"]);

                InviteUserModel invited = new UserService().InviteHouseholdMembers(userModel, numBonusPoints, descBonusPoints, transactionTypeId);
                
                //Send email to invite members to register
                EmailService.SendEmailAdditionalMemberInvitation(email, currentUser.FirstName, url);

                return true;
            }

            return false;
        }


        [AllowAnonymous]
        public ActionResult Login(string returnUrl, bool verified = false)
        {
            ViewBag.ReturnUrl = returnUrl;

            LoginViewModel viewModel = new LoginViewModel();
            viewModel.IsVerified = verified;

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {

            ////Code to change user password from the code
            //string username = "david@greenmoney.com.au";
            //string password = "newpassword";
            //MembershipUser mu = Membership.GetUser(username);
            //mu.ChangePassword(mu.ResetPassword(), password);


            if (ModelState.IsValid && Membership.ValidateUser(model.UserName, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: model.RememberMe);

                var returnUrl = model.ReturnUrl;
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1)
                {
                    // redirect to ?returnUrl
                    return Redirect(returnUrl);
                }
                else
                {
                    // redirect to my profile
                    return RedirectToAction("MyProfile", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginTop(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid && Membership.ValidateUser(loginViewModel.UserName, loginViewModel.Password))
            {
                FormsAuthentication.SetAuthCookie(loginViewModel.UserName, createPersistentCookie: loginViewModel.RememberMe);

                var returnUrl = loginViewModel.ReturnUrl;
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1)
                {
                    // redirect to ?returnUrl
                    return Redirect(returnUrl);
                }
                else
                {
                    // redirect to my profile
                    return RedirectToAction("MyProfile", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");

            LoginViewModel model = new LoginViewModel();
            model.UserName = loginViewModel.UserName;
            model.Password = loginViewModel.Password;
            model.RememberMe = loginViewModel.RememberMe;
        
            return View("Login", model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            //Bussines home page
            if (LayoutViewModel.IsSuplier)
            {
                return RedirectToAction("Index", "Business");
            }

            //Auspost home page
            if (LayoutViewModel.IsAusPost)
            {
                return RedirectToAction("Index", "Auspost");
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult RegisterAddress(string instid, string firstname, string lastname, string email, string sex, 
            string birthDate, string p, string code, bool sendme = false)
        {

            DateTime bdate;
            var hasBirthDate = DateTime.TryParse(birthDate, out bdate);

            var passDecrypted = "notused";//p is empty for fb login
            if (!string.IsNullOrWhiteSpace(p))
            {
                passDecrypted = Encryption.DecryptString(p);
            }

            var viewModel = new RegisterAccountViewModel();
            viewModel.InstanceId = !string.IsNullOrWhiteSpace(instid) ? int.Parse(instid) : 0; //new added field
            viewModel.FirstName = firstname;
            viewModel.LastName = lastname;
            viewModel.Email = email;
            viewModel.SendEmailOffers = sendme;
            viewModel.SendEmailUpdates = sendme;
            viewModel.sendme = sendme;
            //model.Sex = sex;
            viewModel.Password = passDecrypted;
            viewModel.Postcode = code;

            var privateClientId = ConfigurationManager.AppSettings["PrivateClientId"] == null
                ? 5
                : int.Parse(ConfigurationManager.AppSettings["PrivateClientId"]);

            var suburbs = new UserService().GetAllSuburbs(privateClientId, toExclude: true);
            var streetTypes = new UserService().GetAllStreetTypes(privateClientId, toExclude: true);
            viewModel.Suburbs = new SelectList(suburbs);
            viewModel.StreetTypes = new SelectList(streetTypes);

            viewModel.DateOfBirth = hasBirthDate ? bdate : DateTime.MinValue;

            //Display
            LayoutViewModel.HideTopWrapperMenu = true;

            return View(viewModel);
        }


        // STEP 1 - Register Address
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RegisterAddress(RegisterAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
 
                AddressModel addressModelFind = new AddressModel();

                // find new user's address
                AddressModel address = new UserService().FindMatchingAdressModel(
                    unitNumber: model.UnitNumber,
                    streetNumber: model.StreetNumber,
                    streetName: model.StreetName,
                    streetType: model.StreetType,
                    suburb: model.Suburb,
                    postcode: null
                    );

                //Address exists in database
                if (address != null)
                {
                    bool addressHasUsers = new UserService().CheckAddressHasUsersRegistered(
                        unitNumber: model.UnitNumber,
                        streetNumber: model.StreetNumber,
                        streetName: model.StreetName,
                        streetType: model.StreetType,
                        suburb: model.Suburb,
                        postcode: null
                        );

                    //This address still does not have registered users
                    if (!addressHasUsers)
                    {
                        var id = Guid.NewGuid();

                        Boolean isFBUser = false;
                        Boolean isApproved = false;
                        Int64 FBUserId = 0;

                        FacebookUserModel fbUser = null;
                        if (Session["FacebookUser"] != null)
                        {
                            fbUser = (FacebookUserModel)Session["FacebookUser"];
                        }

                        if (fbUser != null)
                        {
                            isFBUser = true;
                            isApproved = true;
                            FBUserId = Convert.ToInt64(fbUser.id);
                        }

                        // create the ASP membership user
                        MembershipCreateStatus status;

                        //Don't allow duplicate user
                        var tryToFindUser = Membership.GetUserNameByEmail(model.Email);
                        if (string.IsNullOrEmpty(tryToFindUser))
                        {
                            var membershipUser = Membership.CreateUser(username: model.Email, password: model.Password, email: model.Email,
                                passwordQuestion: null, passwordAnswer: null,
                                isApproved: isApproved, providerUserKey: id,
                                status: out status
                            );
                            
 
                            if (status == MembershipCreateStatus.Success)
                            {
                                DateTime? saveDate = null;

                                if (model.DateOfBirthDay != null && model.DateOfBirthMonth != null && model.DateOfBirthYear != null)
                                {
                                    saveDate = new DateTime(year: (int)model.DateOfBirthYear.Value,
                                        month: (int)model.DateOfBirthMonth.Value,
                                        day: (int)model.DateOfBirthDay.Value);
                                }

                                // create the GM user
                                UserModel userModel = new UserModel();
                                userModel.Id = id;
                                userModel.FirstName = model.FirstName;
                                userModel.LastName = model.LastName;
                                userModel.AddressId = address.Id;
                                userModel.Instance_Id = address.Instance_Id;
                                userModel.SendEmailOffers = model.SendEmailOffers; //TODO check matching
                                userModel.SendEmailUpdates = model.SendEmailUpdates; //TODO check matching

                                userModel.IsFBAccount = isFBUser;
                                userModel.FBUserId = FBUserId;
                                userModel.IsAdditionalAccountHolder = false;
                                userModel.Sex = model.Sex;
                                userModel.DateOfBirth = saveDate;
                                userModel.PhoneNumber = model.PhoneNumber;

                                // Give them the New Member Bonus 
                                int numBonusPoints = Convert.ToInt16(ConfigurationManager.AppSettings["BonusPoints.NewMember.Points"]);
                                string descBonusPoints = Convert.ToString(ConfigurationManager.AppSettings["BonusPoints.NewMember.Description"]);
                                int? transactionTypeId = Convert.ToInt16(ConfigurationManager.AppSettings["TransactionType.ShareHeart"]);

                                UserModel newUserModel = new UserService().CreateUser(userModel, numBonusPoints, descBonusPoints, transactionTypeId);

                                if (newUserModel != null)
                                {
                                    FormsAuthentication.SetAuthCookie(model.Email, createPersistentCookie: false);
                                }

                                if (!isApproved)
                                {
                                    SendVerifyEmail(membershipUser.Email, userModel.FirstName, userModel.Id, true);
                                    // THey are a normal user
                                    //TODO later - email sending before approve, not it is approved immediately
                                    //SendVerifyEmail(membershipUser, user, true);
                                    membershipUser.IsApproved = true;
                                    Membership.UpdateUser(membershipUser);
                                

                                    //TODO later - checkout what is this
                                    //if (false)
                                    //{
                                    //    // Add them to the relevant MailChimp List
                                    //    MailChimpManager mc = new MailChimpManager(ConfigurationManager.AppSettings["MailChimp.APIKey"]);
                                    //    EmailParameter email = new EmailParameter()
                                    //    {
                                    //        Email = user.Email
                                    //    };

                                    //    EmailParameter results = mc.Subscribe(ConfigurationManager.AppSettings["MailChimp.ListId"], email);
                                    //}

                                }
                                else
                                {
                                    // They have just logged in with FB so Login as usual
                                    FormsAuthentication.SetAuthCookie(model.Email, createPersistentCookie: false);
                                }

                                // the next registration step
                                return RedirectToAction("AddHouseholdMembers", new { addressId = address.Id, inviter = model.Email, inviterId = id });

                            }
                            else
                            {
                                ModelState.AddModelError("", ErrorCodeToString(status));
                            }
                        }
                        else
                        {
                            // email has already been registered in the system
                            ModelState.AddModelError("", "User with the same email address has already been registered with GreenMoney.");
                        }
                    }
                    else
                    {
                        // address already has a user on it!
                        ModelState.AddModelError("", "Address has already been registered with GreenMoney.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Please contact us with your address details and we'll assist you in creating an account.");

                    TempData["ProblemWithAddress"] = true;
                }
            }

            var suburbs = new UserService().GetAllSuburbs();
            var streetTypes = new UserService().GetAllStreetTypes();

            model.Suburbs = new SelectList(suburbs);
            model.StreetTypes = new SelectList(streetTypes);

            //Display
            LayoutViewModel.HideTopWrapperMenu = true;

            // something failed, redisplay form
            return View(model);

        }


        // STEP 2 - Add Household Members
        [AllowAnonymous]
        [HttpGet]
        public ActionResult AddHouseholdMembers(int addressId, string inviter, string inviterId)
        {
            var model = new AddHouseholdMembersViewModel();
            var list = new List<HouseholdMemberModel>
            {
                new HouseholdMemberModel(),
                new HouseholdMemberModel(),
                new HouseholdMemberModel(),
                new HouseholdMemberModel()
            };

            foreach (var item in list)
            {
                item.AddressId = addressId;
                item.InviterName = inviter;
                item.InviterId = inviterId;
            }

            model.HouseholdMembers = list;

            ViewData["competed"] = true;

            //Display
            LayoutViewModel.HideTopWrapperMenu = true;

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddHouseholdMembers(List<HouseholdMemberModel> householdMembers)
        {
            ViewData["competed"] = true;

            string tempUrl = string.Empty;
            foreach (var member in householdMembers.Where(m => !string.IsNullOrEmpty(m.Email)))
            {
                // create invite additional members url
                var url = Url.Action("RegisterHouseholdMembers", "Account", new
                {
                    emailaddress = member.Email,
                    inviter = member.InviterName,
                    addressid = member.AddressId,
                    firstname = member.FirstName,
                    lastname = member.LastName
                }, Request.Url.Scheme);

                tempUrl = string.Format("{0} ... {1}", tempUrl, url);


                // Give them the Invite Member Bonus 
                int numBonusPoints = Convert.ToInt16(ConfigurationManager.AppSettings["BonusPoints.InviteMember.Points"]);
                string descBonusPoints = Convert.ToString(ConfigurationManager.AppSettings["BonusPoints.InviteMember.Description"]);
                int? transactionTypeId = Convert.ToInt16(ConfigurationManager.AppSettings["TransactionType.ShareHeart"]);


                // email content
                if (User.Identity.IsAuthenticated)
                {
                    var membership = Membership.GetUser();
                    if (membership != null)
                    {
                        var userModel = new InviteUserModel
                        {
                            Email = member.Email,
                            FirstName = member.FirstName,
                            LastName = member.LastName,
                            User_Id = new Guid(member.InviterId)
                        };

                        InviteUserModel invited = new UserService().InviteHouseholdMembers(userModel, numBonusPoints, descBonusPoints, transactionTypeId);

                        //Send email to invite members to register
                        EmailService.SendEmailAdditionalMemberInvitation(member.Email, member.InviterName, url);
                    }

                    //return RedirectToAction("InviteFriends", new {urltemp = tempUrl}); TODO later
                    return RedirectToAction("MyProfile", "Account");
                }
            }

            //return RedirectToAction("InviteFriends");
            return RedirectToAction("MyProfile", "Account");
        }



        public ActionResult InviteFriends(string urltemp)
        {
            TempData["LinkInviteMember"] = urltemp;

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult RegisterHouseholdMembers(string emailaddress, string inviter, string addressid, string firstname, string lastname)
        {
            var addressIdInt = Int32.Parse(addressid);

            bool inviterUserFound = new UserService().FoundOriginalUserForAddressById(addressIdInt);

            if (inviterUserFound)
            {
                var model = new HouseholdMemberModel
                {
                    Email = emailaddress,
                    ConfirmEmail = emailaddress, //hidden on form
                    FirstName = firstname,
                    LastName = lastname,
                    AddressId = addressIdInt
                };

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RegisterHouseholdMembers(HouseholdMemberModel householdMemberModel)
        {
            AddressModel address = new UserService().GetAddressById(householdMemberModel.AddressId);

            //Don't allow duplicate user
            var tryToFindUser = Membership.GetUserNameByEmail(householdMemberModel.Email);
            if (string.IsNullOrEmpty(tryToFindUser))
            {

                var id = Guid.NewGuid();

                Boolean isFBUser = false;
                Boolean isApproved = true;
                Int32 FBUserId = 0;

                // create the ASP membership user
                MembershipCreateStatus status;
                var membershipUser = Membership.CreateUser(username: householdMemberModel.Email, password: householdMemberModel.Password, email: householdMemberModel.Email,
                    passwordQuestion: null, passwordAnswer: null,
                    isApproved: isApproved, providerUserKey: id,
                    status: out status
                );

                // Check inviter user role and the same role for invited user
                var addressHolderUserEmail = new UserService().GetUserNameHolderForAddress(householdMemberModel.AddressId);
                if (!string.IsNullOrWhiteSpace(addressHolderUserEmail))
                {
                    if (Roles.IsUserInRole(addressHolderUserEmail, "Supplier") && membershipUser != null)
                    {
                        Roles.AddUserToRole(membershipUser.Email, "Supplier");
                    }
                }

                if (status == MembershipCreateStatus.Success)
                {
                    // create the GM user
                    var user = new UserModel
                    {
                        Id = id,
                        FirstName = householdMemberModel.FirstName,
                        LastName = householdMemberModel.LastName,
                        AddressModel = address,
                        AddressId = address.Id,
                        Instance_Id = address.Instance_Id,
                        SendEmailOffers = householdMemberModel.SendEmailOffers,
                        SendEmailUpdates = householdMemberModel.SendEmailUpdates,
                        IsFBAccount = isFBUser,
                        FBUserId = FBUserId,
                        IsAdditionalAccountHolder = true
                    };

                    // Give them the New Member Bonus 
                    int numBonusPoints = Convert.ToInt16(ConfigurationManager.AppSettings["BonusPoints.NewMember.Points"]);
                    string descBonusPoints = Convert.ToString(ConfigurationManager.AppSettings["BonusPoints.NewMember.Description"]);
                    int? transactionTypeId = Convert.ToInt16(ConfigurationManager.AppSettings["TransactionType.ShareHeart"]);

                    // store it
                    var newUser = new UserService().CreateUser(user, numBonusPoints, descBonusPoints, transactionTypeId);

                    int numBonusInvitationAcceptedPoints = Convert.ToInt16(ConfigurationManager.AppSettings["BonusPoints.InviteMember.Points"]);
                    string descInvitationAcceptedBonusPoints = Convert.ToString(ConfigurationManager.AppSettings["BonusPoints.InviteMember.Description"]);

                    bool addPoints = new UserService().InvitationAcceptedAddBonusPoints(addressHolderUserEmail,
                        numBonusInvitationAcceptedPoints, descInvitationAcceptedBonusPoints, transactionTypeId);


                    SendVerifyEmail(membershipUser.Email, user.FirstName, user.Id, true);

                    bool isProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["MailChimp.ListId"]);

                    if (isProduction)
                    {
                        // Add them to the relevant MailChimp List
                        MailChimpManager mc = new MailChimpManager(ConfigurationManager.AppSettings["MailChimp.APIKey"]);
                        EmailParameter email = new EmailParameter()
                        {
                            Email = user.Email
                        };

                        EmailParameter results = mc.Subscribe(ConfigurationManager.AppSettings["MailChimp.ListId"], email);

                    }


                    return RedirectToAction("RegisterAccountSuccess");

                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(status));
                    return View(householdMemberModel);
                }
            }
            else
            {
                // email has already been registered in the system
                ModelState.AddModelError("", "User with the same email address has already been registered with GreenMoney.");
                return View(householdMemberModel);

            }
        }

        [AllowAnonymous]
        public ActionResult RegisterAccountSuccess()
        {
            return View();
        }


        /// <summary>
        /// Code for this user, used for account verification.
        /// </summary>
        public string GetVerifyCode(Guid userId)
        {
            return ZBase32.Encode(userId.ToByteArray()); 
        }

        /// <summary>
        /// Given a user verify code (see VerifyCode), gets the GUID Id for the user.
        /// </summary>
        public static Guid IdFromVerifyCode(string code)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            return new Guid(ZBase32.Decode(code));
        }

        [NonAction]
        private void SendVerifyEmail(string email, string firstName, Guid userId, bool registration)
        {
            // create verification url
            var url = Url.Action("Verify", "Account", new
            {
                code = GetVerifyCode(userId)
            }, Request.Url.Scheme);

            var contentPath = Server.MapPath(registration
                ? "~/App_Data/Emails/registration.cshtml"
                : "~/App_Data/Emails/email-change.cshtml");

            EmailService.SendEmailVerifyAccount(contentPath, email, firstName, url);

        }


        #region Helpers

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        private static string SetRecipientEmail(string email)
        {
            bool isProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["MailChimp.ListId"]);
            var recipientEmail = ConfigurationManager.AppSettings["TestEmailAddress"];
            if (isProduction)
            {
                recipientEmail = email;
            }
            return recipientEmail;
        }
        
        #endregion


    }
}
