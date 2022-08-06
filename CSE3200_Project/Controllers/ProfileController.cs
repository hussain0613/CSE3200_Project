using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSE3200_Project.Attributes;
using CSE3200_Project.Models;

namespace CSE3200_Project.Controllers
{
    [CurrentUser(login_required = true)]
    public class ProfileController : Controller
    {

        // GET: Profile
        [HttpGet]
        public ActionResult Index()
        {
            User current_user = (User)HttpContext.Items["current_user"];
            return View(current_user);
        }
    }
}