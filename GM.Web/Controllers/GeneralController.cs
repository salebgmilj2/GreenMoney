using System;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using GM.BusinessLogic;
using GM.Models.Public;
using GM.Models.Shared;
using GM.ViewModels;

namespace GM.Controllers
{
    public class GeneralController : SimpleApplicationController
    {
        [OutputCache(CacheProfile = "Upload")]
        public ActionResult Upload(int id)
        {
            var upload = new UploadService().GetUpload(id);

            if (upload == null)
                return HttpNotFound();

            return File(upload.Contents, upload.ContentType ?? "image/jpeg");
        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                MemoryStream target = new MemoryStream();
                file.InputStream.CopyTo(target);
                byte[] data = target.ToArray();

                UploadModel model = new UploadModel
                {
                    ContentType = file.ContentType,
                    Contents = data
                };

                UploadModel upload = new UploadService().UploadFile(LayoutViewModel.ProviderUserKey, model);
 
            }

            return View();
        }

        //public ActionResult AddSupplierAddressDefault()
        //{

        //    string addressId = new UploadService().AddSupplierAddressDefault();

        //    ViewData["addressId"] = addressId;

        //    return View();
        //}

        //public ActionResult DeleteUser(string username)
        //{
        //    MembershipUser membershipUser = Membership.GetUser(username);

        //    Guid userId = (Guid) membershipUser.ProviderUserKey;

        //    var config = WebConfigurationManager.OpenWebConfiguration("~");
        //    var section = config.SectionGroups["system.web"].Sections["membership"] as MembershipSection;
        //    var defaultProvider = section.DefaultProvider;
        //    var connectionStringName = section.Providers[defaultProvider].ElementInformation.Properties["connectionStringName"].Value.ToString();

        //    string connectionString = config.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;


        //    var deleted = new UserService().DeleteUser(userId, connectionString);

           
        //    return View();
        //}

        //public ActionResult ChangePassword(string username)
        //{
        //    //Code to change user password from the code
        //    string password = "newpassword";
        //    MembershipUser mu = Membership.GetUser(username);
        //    mu.ChangePassword(mu.ResetPassword(), password);

        //    return View();
        //}

        //public ActionResult AddRewards4Images()
        //{
        //    var config = WebConfigurationManager.OpenWebConfiguration("~");
        //    var section = config.SectionGroups["system.web"].Sections["membership"] as MembershipSection;
        //    var defaultProvider = section.DefaultProvider;
        //    var connectionStringName = section.Providers[defaultProvider].ElementInformation.Properties["connectionStringName"].Value.ToString();

        //    string connectionString = config.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;

        //    var deleted = new UserService().AddRewards4Images(connectionString);

        //    return View();
        //}


        public ActionResult Page(string id = "", string inst = "")
        {
            try
            {
                if(string.IsNullOrWhiteSpace(inst))
                {
                    inst = LayoutViewModel.Instance_Id != null
                        ? LayoutViewModel.Instance_Id.ToString()
                        : string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(id))
                {
                    PageViewModel viewModel = new PageViewModel
                    {
                        PageModel = new CmsService().GetPage(id, inst)
                    };

                    return View(viewModel);
                }
                else
                {
                    return View("Error");
                }
            }
            catch
            {
                return View("Error");
            }
        }

        public ActionResult Section(string id = "")
        {
            // Find the first page in the section and send them there, as happened before.
            try
            {               
                if (!string.IsNullOrWhiteSpace(id))
                {
                    string sectionPageUrl = new CmsService().GetSectionPageUrl(id);

                    if (sectionPageUrl != null)
                    {
                        return RedirectToAction("Page", new { id = sectionPageUrl, inst = LayoutViewModel.Instance_Id });
                    }
                    return View("Error");
                }
                else
                {
                    return View("Error");
                }
            }
            catch
            {
                return View("Error");
            }
        }

        public ActionResult AlterChallenges()
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var section = config.SectionGroups["system.web"].Sections["membership"] as MembershipSection;
            var defaultProvider = section.DefaultProvider;
            var connectionStringName = section.Providers[defaultProvider].ElementInformation.Properties["connectionStringName"].Value.ToString();

            string connectionString = config.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;

            var deleted = new UserService().AlterChallenges(connectionString);

            return View();
        }
    }
}
