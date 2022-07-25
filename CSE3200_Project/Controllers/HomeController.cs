using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSE3200_Project.Models;

namespace CSE3200_Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AuthHelpers ah = new AuthHelpers(Request);
            User cu = ah.get_current_user();
            ViewBag.IsAuthenticated = false;
            if (cu != null) {
                ViewBag.current_user = cu;
                ViewBag.IsAuthenticated = true;
            }

            return View();
        }
    }
}