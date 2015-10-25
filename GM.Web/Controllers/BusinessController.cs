using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GM.BusinessLogic;
using GM.Models.Public;
using GM.ViewModels;
using GM.ViewModels.Shared;

namespace GM.Controllers
{
    public class BusinessController : SimpleApplicationController
    {
        //
        // GET: /Bussiness/

        public ActionResult Index()
        {
            var model = new BusinessHomeViewModel();
 
            var instances = new UserService().GetAllInstances();
            model.RegistrationSupplierViewModel = new RegistrationSupplierViewModel
            {
                Instances = new SelectList(instances, "Id", "Name")
            };

            model.LoginViewModel = new LoginViewModel();

            LayoutViewModel.IsAuthenticated = false;
            LayoutViewModel.HideTopWrapperMenu = true;
            LayoutViewModel.ActiveLink = Links.HomePage;

            return View(model);
        }
        //Supplier register
        [HttpPost]
        public ActionResult RegisterSupplier(RegistrationSupplierViewModel registrationSupplierViewModel)
        {
            if (ModelState.IsValid)
            {

                //Don't allow duplicate user
                var tryToFindUser = Membership.GetUserNameByEmail(registrationSupplierViewModel.BusinessEmail);
                if (string.IsNullOrEmpty(tryToFindUser))
                {

                    // Create the ASP membership user
                    var id = Guid.NewGuid();
                    MembershipCreateStatus status;

                    var membershipUser = Membership.CreateUser(username: registrationSupplierViewModel.BusinessEmail,
                        password: registrationSupplierViewModel.Password, email: registrationSupplierViewModel.BusinessEmail,
                        passwordQuestion: null, passwordAnswer: null,
                        isApproved: true, providerUserKey: id,
                        status: out status
                    );

                    if (status == MembershipCreateStatus.Success)
                    {

                        // Create the GM user for supplier with default address (temp id= 258440)
                        UserModel userModel = new UserModel();
                        userModel.Id = id;
                        userModel.Instance_Id = registrationSupplierViewModel.InstanceId;
                        userModel.BusinessName = registrationSupplierViewModel.BusinessName;

                        // TODO - think about logic Email vs. BusinessEmail
                        userModel.BussinesEmail = registrationSupplierViewModel.BusinessEmail;
                        userModel.Email = registrationSupplierViewModel.BusinessEmail;

                        userModel.BussinesPhoneArea = registrationSupplierViewModel.BussinesPhoneArea;
                        userModel.BussinesPhone = registrationSupplierViewModel.BusinessPhone;
                        userModel.FirstName = registrationSupplierViewModel.FirstName;
                        userModel.LastName = registrationSupplierViewModel.LastName;

                        userModel.AddressId = int.Parse(ConfigurationManager.AppSettings["DefaultSupplierAddressId"]);
                        //TODO this should be bigint actually? add error logging
                        userModel.IsAdditionalAccountHolder = false;

                        // Give them the New Member Bonus 
                        int numBonusPoints = Convert.ToInt16(ConfigurationManager.AppSettings["BonusPoints.NewMember.Points"]);
                        string descBonusPoints = Convert.ToString(ConfigurationManager.AppSettings["BonusPoints.NewMember.Description"]);
                        int? transactionTypeId = Convert.ToInt16(ConfigurationManager.AppSettings["TransactionType.ShareHeart"]);

                        UserModel newUserModel = new UserService().CreateSupplierUser(userModel, numBonusPoints, descBonusPoints, transactionTypeId);

                        if (newUserModel != null)
                        {
                            if (!Roles.IsUserInRole(membershipUser.Email, "Supplier"))
                            {
                                Roles.AddUserToRole(membershipUser.Email, "Supplier");
                            }
                            FormsAuthentication.SetAuthCookie(registrationSupplierViewModel.BusinessEmail, createPersistentCookie: false);
                        }
                    }

                    return RedirectToAction("CompleteProfile", "Supplier");
                }
                else
                {
                    return RedirectToAction("Index", "Business");
                }
            }
            else // something failed, redisplay form
            {
                BusinessHomeViewModel viewModel = new BusinessHomeViewModel();
                viewModel.SupplierRegistrationIsNotValid = true;

                var instances = new UserService().GetAllInstances();
                registrationSupplierViewModel.Instances = new SelectList(instances, "Id", "Name");
                viewModel.RegistrationSupplierViewModel = registrationSupplierViewModel;

                LayoutViewModel.IsAuthenticated = false;
                LayoutViewModel.HideTopWrapperMenu = true;

                return View("Index", viewModel);
            }
        }


    }
}
