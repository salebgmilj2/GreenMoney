using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using GM.BusinessLogic;
using GM.Models;
using GM.ViewModels.Shared;
using System.Configuration;

namespace GM.Controllers
{
    public class SimpleApplicationController : Controller
    {
        protected LayoutViewModel LayoutViewModel { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            var returnUrl = requestContext.HttpContext.Request.QueryString["ReturnUrl"];
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = requestContext.HttpContext.Request.Url.PathAndQuery;
            }

            LayoutViewModel = new LayoutViewModel
            {
                Title = "Green Money"
            };

            if (Request.IsAuthenticated)
            {

                var membershipUser = Membership.GetUser();

                if (membershipUser == null)
                {
                    FormsAuthentication.SignOut();
                    Session.Abandon();
                }
                else
                {
                    var currentUserId = membershipUser.ProviderUserKey.ToString();
                    //currentUserId = "C26B6CA5-6262-4AFA-8AC7-529929CAB0B1";
                    //currentUserId = "CDEE65FA-FD6C-40D7-A134-43A4B0AD696D";
                    //currentUserId = "A26E61B4-016D-4ECD-8F04-4CE10632A94D";

                    
                    var user = new ProfileService().GetMyProfileBase(currentUserId, membershipUser.Email);

                    LayoutViewModel.IsAuthenticated = true;
                    LayoutViewModel.IsSuplier = Roles.IsUserInRole(membershipUser.Email, "Supplier");
                    LayoutViewModel.IsCouncil = Roles.IsUserInRole(membershipUser.Email, "Council");

                    var privateClientId = ConfigurationManager.AppSettings["PrivateClientId"] == null
                    ? 5
                    : int.Parse(ConfigurationManager.AppSettings["PrivateClientId"]);
                    LayoutViewModel.IsAusPost = user.Instance_Id == privateClientId;

                    LayoutViewModel.CurrentAccountId = new Guid(currentUserId);
                    LayoutViewModel.CurrentFirstName = user.User.FirstName;
                    LayoutViewModel.CurrentLastName = user.User.LastName;
                    LayoutViewModel.CurrentUserEmail = membershipUser.Email;
                    LayoutViewModel.CurrentUserPhotoId = user.User.PhotoID;
                    LayoutViewModel.ProviderUserKey = currentUserId;

                    LayoutViewModel.CurrentUserTotalPoints = user.PointsAvailable;
                    LayoutViewModel.CurrentUserTotalCart = user.CartTotal;
                    LayoutViewModel.CurrentUserTotalChallenges = user.ChallengesTotal;

                    LayoutViewModel.Instance_Id = user.Instance_Id;
                    
                    // for Council
                    if (Roles.IsUserInRole(membershipUser.Email, "Council"))
                    {
                        LayoutViewModel.UserChallengesCount = (new ProfileService().GetCouncilProfile(Convert.ToInt32(user.Instance_Id))).UserChallengesCount;
                    }
                }
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
 
            var viewResult = filterContext.Result as ViewResultBase;
            if (viewResult != null)
            {
                var model = viewResult.ViewData.Model as LayoutViewModel;
                if (model != null)
                {
                    Utils.CopyProperties(LayoutViewModel, model);
                }
                else
                {
                    viewResult.ViewData.Model = LayoutViewModel;
                }
            }
        } 
    }
}
