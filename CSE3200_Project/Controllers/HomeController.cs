using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSE3200_Project.Attributes;
using CSE3200_Project.Models;

namespace CSE3200_Project.Controllers
{
    [CurrentUser]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        
    }
}