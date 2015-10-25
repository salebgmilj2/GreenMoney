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
    public class AuspostController : SimpleApplicationController
    {
        //
        // GET: /Bussiness/

        public ActionResult Index()
        {
            var model = new AuspostHomeViewModel();
 
            model.LoginViewModel = new LoginAuspostViewModel();

            LayoutViewModel.IsAuthenticated = false;
            LayoutViewModel.HideTopWrapperMenu = true;
            LayoutViewModel.ActiveLink = Links.HomePage;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginAuspostViewModel loginViewModel)
        {
            if (ModelState.IsValid && Membership.ValidateUser(loginViewModel.UserName, loginViewModel.Password))
            {
                FormsAuthentication.SetAuthCookie(loginViewModel.UserName, createPersistentCookie: false);

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
            ModelState.AddModelError("CustomError", "The user name or password provided is incorrect.");



            var model = new AuspostHomeViewModel();

            LoginAuspostViewModel loginModel = new LoginAuspostViewModel();
            loginModel.UserName = loginViewModel.UserName;
            loginModel.Password = loginViewModel.Password;

            model.LoginViewModel = loginModel;

            LayoutViewModel.IsAuthenticated = false;
            LayoutViewModel.HideTopWrapperMenu = true;
            LayoutViewModel.ActiveLink = Links.HomePage;
 
            return View(model);
        }
    }
}
