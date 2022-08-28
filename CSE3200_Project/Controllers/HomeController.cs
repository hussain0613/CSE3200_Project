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
        private DRIEntities db = new DRIEntities();
        public ActionResult Index()
        {
            int public_resources_count = 0;
            User current_user = (User)HttpContext.Items["current_user"];
            if(current_user != null)
            {
                ViewBag.user_shelves = db.Shelves.Where(shelf => shelf.creator_id == current_user.id).OrderByDescending(shelf => shelf.modification_datetime);
                ViewBag.user_contents = db.Contents.Where(cont => cont.creator_id == current_user.id && cont.Shelves.Count == 0).OrderByDescending(cont => cont.modification_datetime);

                IOrderedQueryable<Shelf> shelves = db.Shelves.Where(shelf => shelf.privacy.ToLower().Equals("public") && shelf.creator_id != current_user.id).
                    OrderByDescending(shelf => shelf.modification_datetime);
                public_resources_count += shelves.Count();
                ViewBag.public_shelves = shelves;

                IOrderedQueryable<Content> contents = db.Contents.Where(cont => cont.privacy.ToLower().Equals("public") && cont.creator_id != current_user.id && cont.Shelves.Count == 0).
                    OrderByDescending(cont => cont.modification_datetime);
                ViewBag.public_contents = contents;
                public_resources_count += contents.Count();
            }
            else
            {
                IOrderedQueryable<Shelf> shelves = db.Shelves.Where(shelf => shelf.privacy.ToLower().Equals("public")).
                    OrderByDescending(shelf => shelf.modification_datetime);
                public_resources_count += shelves.Count();
                ViewBag.public_shelves = shelves;

                IOrderedQueryable<Content> contents = db.Contents.Where(cont => cont.privacy.ToLower().Equals("public") && cont.Shelves.Count == 0).
                    OrderByDescending(cont => cont.modification_datetime);
                ViewBag.public_contents = contents;
                public_resources_count += contents.Count();
            }
            ViewBag.public_resources_count = public_resources_count;
            return View();
        }
    }
}