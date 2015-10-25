using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using System.Web.Security;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using GM.BusinessLogic;
using GM.Models;
using GM.Models.Public;
using GM.Utility;
using GM.Utility.EmailService;
using GM.ViewModels;
using GM.ViewModels.Shared;
using Newtonsoft.Json;
using System.Configuration;

namespace GM.Controllers
{
    public class HomeController : SimpleApplicationController
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("MyProfile", "Account");
            //else
            //    return RedirectToAction("Login", "Account");

            //Code to change user password from the code
            //string username = "jeremy.yk.ong@gmail.com";
            //string password = "newpassword";
            //MembershipUser mu = Membership.GetUser(username);
            //mu.ChangePassword(mu.ResetPassword(), password);

            //Code to change user password from the code
            //string username = "ezra@punchlane.com.au";
            //string password = "newpassword";
            //MembershipUser mu = Membership.GetUser(username);
            //mu.ChangePassword(mu.ResetPassword(), password);
            

            var model = new HomeViewModel();
            model.RegistrationViewModel = new RegistrationViewModel
            {
                SendEmailOffers = true
            };

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

        [HttpPost]
        public ActionResult Index(RegistrationViewModel registrationViewModel)
        {
            if (ModelState.IsValid)
            {

                InstanceModel instanceModel = new UserService().GetInstanceForPostCode(registrationViewModel.Postcode);

                bool userIsAlreadyInInterestedPeople = new UserService().UserIsInInterestedPeople(registrationViewModel.Email);

                if (instanceModel != null && userIsAlreadyInInterestedPeople)
                {
                    ModelState.AddModelError("", "Email address has already been registered with GreenMoney.");
                }

                DateTime bdate = DateTime.MinValue;

                //No matching postcode found, so just register them as an intersted person
                if (instanceModel == null)
                {
                    DateTime? saveDate = null;
                    if (bdate > DateTime.MinValue)
                        saveDate = bdate;

                    UserModel userModel = new UserModel();
                    userModel.FirstName = registrationViewModel.FirstName;
                    userModel.LastName = registrationViewModel.LastName;
                    userModel.Postcode = registrationViewModel.Postcode;
                    userModel.SendEmailOffers = false;
                    userModel.SendEmailUpdates = registrationViewModel.SendEmailOffers;//TODO check this match
                    userModel.Email = registrationViewModel.Email;
                    userModel.DateOfBirth = saveDate;

                    UserModel m = new UserService().AddUserToInInterestedPeople(userModel);


                    var emailContentPath = Server.MapPath("~/App_Data/Emails/register-interest.html");
                    EmailService.SendEmailRegisterInterest(emailContentPath, m.Email);
                     

                    HomeViewModel viewModel = new HomeViewModel();
                    viewModel.PostCodeIsNotMatching = true;
                    viewModel.RegistrationViewModel = registrationViewModel;

                    var instances = new UserService().GetAllInstances();
                    viewModel.RegistrationSupplierViewModel = new RegistrationSupplierViewModel
                    {
                        Instances = new SelectList(instances, "Id", "Name")
                    };

                    return View(viewModel);
                }
                else
                {
                    //Start registration process for current instance
                    string passwordEncrypted = Encryption.EncryptData(registrationViewModel.Password);

                    return RedirectToAction("RegisterAddress", "Account", new
                    {
                        firstname = registrationViewModel.FirstName,
                        lastname = registrationViewModel.LastName,
                        email = registrationViewModel.Email,
                        p = passwordEncrypted,
                        code = registrationViewModel.Postcode,
                        instid = instanceModel.Id,
                        sendme = registrationViewModel.SendEmailOffers
                    });
                }

            }

            // something failed, redisplay form
            else
            {
                HomeViewModel viewModel = new HomeViewModel();
                viewModel.RegistrationIsNotValid = true;
                viewModel.RegistrationViewModel = registrationViewModel;

                var instances = new UserService().GetAllInstances();
                viewModel.RegistrationSupplierViewModel = new RegistrationSupplierViewModel
                {
                    Instances = new SelectList(instances, "Id", "Name")
                };
                LayoutViewModel.IsAuthenticated = false;
                LayoutViewModel.HideTopWrapperMenu = true;

                return View(viewModel);
            }
        }


        //User is authenticated by facebook, check post code and insert into:
        // - interested people (post code not found) or 
        // - create account
        [HttpGet]
        public ActionResult FacebookLoginNew(string token)
        {
            LayoutViewModel.HideTopWrapperMenu = true;

            if (token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var fbUser = GetFacebookUser(token);

            Int64 userIdAsInt = Convert.ToInt64(fbUser.id);

            try
            {
                UserModel userModel = new UserService().GetFacebookUser(userIdAsInt);

                //var alreadyHasAccount = _context.Users.SingleOrDefault(a => a.Email == fbUser.email);
                // 0 - User already has account saved in db (the basic one not facebook). Link accounts?

                // 1 - User does not have fb account saved in db, prepare for registration
                if (userModel == null)
                {
                    // They are just registering
                    Session["FacebookUser"] = fbUser;
                    Session["IsFBRegistration"] = true;
                    Session["FBUserId"] = fbUser.id;

                    var model = new HomeViewModel();
                    model.EnterFbRgstrPostCode = true;

                    var instances = new UserService().GetAllInstances();
                    model.RegistrationSupplierViewModel = new RegistrationSupplierViewModel
                    {
                        Instances = new SelectList(instances, "Id", "Name")
                    };

                    return View("Index", model);

                }
                else // 2 - User has fb account saved in db, authenticate this user into our system
                {
                    // Login as usual
                    FormsAuthentication.SetAuthCookie(userModel.Email, createPersistentCookie: false);

                    // redirect to my profile
                    return RedirectToAction("Index", "Account");
                }
            }
            catch (Exception ex)
            {
                Redirect(Url.RouteUrl("Error", new { message = "We experienced an error when attempting to log you in with Facebook.  Please try again later." }));
            }

            return RedirectToAction("Index");
        }



        [NonAction]
        public FacebookUserModel GetFacebookUser(string accessToken)
        {
            var userFeed = GetFromAddress("https://graph.facebook.com/me?access_token=" + accessToken);
            var fbUser = JsonConvert.DeserializeObject<FacebookUserModel>(userFeed);
            fbUser.AccessToken = accessToken;

            return fbUser;
        }


        public string GetFromAddress(string address)
        {

            HttpWebResponse response = null;
            string result;
            try
            {
                // Create and initialize the web request  
                var request = WebRequest.Create(address) as HttpWebRequest;
                // Get response  
                response = request.GetResponse() as HttpWebResponse;
                // Get the response stream  
                var reader = new StreamReader(response.GetResponseStream());
                // Read it into a StringBuilder  
                var sbSource = new StringBuilder(reader.ReadToEnd());
                // Set result  
                result = sbSource.ToString();
            }
            catch (WebException wex)
            {
                // This exception will be raised if the server didn't return 200 - OK  
                // Try to retrieve more information about the network error  
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        //errorResponse log this error
                    }
                }
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }


        [HttpPost]
        public JsonResult FacebookLoginVerifyPostCode(string code)
        {

            FacebookUserModel fbUser = null;
            if (Session["FacebookUser"] != null)
            {
                fbUser = (FacebookUserModel)Session["FacebookUser"];
            }

            if (fbUser == null || string.IsNullOrEmpty(code))//TODO add validation for post code
            {
                return Json(new { success = false, result = "Error occured during verifying post code" });
            }

            InstanceModel instanceModel = new UserService().GetInstanceForPostCode(code);

            bool userIsAlreadyInInterestedPeople = new UserService().UserIsInInterestedPeople(fbUser.email);

            if (instanceModel != null && userIsAlreadyInInterestedPeople)
            {
                ModelState.AddModelError("", "Email address has already been registered with GreenMoney.");
            }

            // Try to get birht day from fb api
            DateTime bdate = fbUser.birthday; //DateTime bdate = DateTime.MinValue;

            // Add user to global DB as an interested person
            if (instanceModel == null)
            {
                DateTime? saveDate = null;
                if (bdate > DateTime.MinValue)
                    saveDate = bdate;

                UserModel userModel = new UserModel();
                userModel.FirstName = fbUser.first_name ?? "N/A";
                userModel.LastName = fbUser.last_name ?? "N/A";
                userModel.Postcode = code;
                userModel.SendEmailOffers = false;
                userModel.SendEmailUpdates = false;
                userModel.Email = fbUser.email ?? "fbUser" + (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds + "@email.com"; // TODO check this
                userModel.DateOfBirth = saveDate;

                UserModel m = new UserService().AddUserToInInterestedPeople(userModel);


                var emailContentPath = Server.MapPath("~/App_Data/Emails/register-interest.html");
                EmailService.SendEmailRegisterInterest(emailContentPath, m.Email);
                    

                HomeViewModel viewModel = new HomeViewModel();
                viewModel.PostCodeIsNotMatching = true;

                return Json(new { success = false, result = "No matching postcode found." });

            }
            else
            {
                string fbEmail; //fb email might be restricted0
                if (string.IsNullOrWhiteSpace(fbUser.email))
                {
                    fbEmail = "fbUser" + (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds + "@email.com";
                }
                else
                {
                    fbEmail = fbUser.email;
                }

                //Register them on the correct site
                //var redirectUrl = instanceModel.Url + "/account/registeraddress?firstname=" + fbUser.first_name
                //                  + "&lastname=" + fbUser.last_name
                //                  + "&email=" + fbEmail
                //                  + "&code=" + code
                //                  + "&instid=" + instanceModel.Id
                //                    //+ "&sex=" + model.Sex //Redesing
                //                  + "&birthDate=" + fbUser.birthday;

                var redirectUrl = Url.Action("RegisterAddress", "Account", new
                {
                    firstname = fbUser.first_name,
                    lastname = fbUser.last_name,
                    email = fbEmail,
                    code = code,
                    instid = instanceModel.Id,
                    birthDate = fbUser.birthday
                }, Request.Url.Scheme);


                return Json(new { success = true, result = redirectUrl });
            }

        }

        [AllowAnonymous]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
