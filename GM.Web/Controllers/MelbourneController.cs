using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GM.Controllers
{
    public class MelbourneController : Controller
    {
        //
        // GET: /Melbourne/

        public ActionResult Index()
        {
            return RedirectToActionPermanent("Index", "Home");
        }

    }
}
