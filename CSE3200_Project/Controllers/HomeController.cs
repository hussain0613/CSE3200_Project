using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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
            User current_user = (User)HttpContext.Items["current_user"];
            string query = Request.QueryString.Get("query");
            string filter_tag = Request.QueryString.Get("tag");
            string filter_type = Request.QueryString.Get("type");

            IQueryable<Shelf> public_shelves, user_shelves = null;
            IQueryable<Content> public_contents, user_contents = null;
            
            if (current_user != null)
            {
                user_shelves = db.Shelves.Include(shelf => shelf.tags).Where(shelf => shelf.creator_id == current_user.id);
                user_contents = db.Contents.Include(cont => cont.tags).Where(cont => cont.creator_id == current_user.id);
                

                public_shelves = db.Shelves.Where(shelf => shelf.privacy.ToLower().Equals("public") && shelf.creator_id != current_user.id);
                public_contents = db.Contents.Where(cont => cont.privacy.ToLower().Equals("public") && cont.creator_id != current_user.id);
            }
            else
            {
                public_shelves = db.Shelves.Where(shelf => shelf.privacy.ToLower().Equals("public"));
                public_contents = db.Contents.Where(cont => cont.privacy.ToLower().Equals("public"));
            }

            if (string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(filter_tag) && (string.IsNullOrWhiteSpace(filter_type) || filter_type.Equals("any")))
            {
                //public_contents = public_contents.Where(cont => cont.Shelves.Count() == 0);
                //if(current_user != null)
                //{
                //    user_contents = user_contents.Where(cont => cont.Shelves.Count() == 0);
                //}
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(query))
                {
                    query = query.Trim().ToLower();
                    ViewBag.query = query;

                    public_shelves = public_shelves.Where(shelf => shelf.title.ToLower().Contains(query));
                    public_contents = public_contents.Where(
                        cont => cont.title.ToLower().Contains(query) || 
                        cont.url.ToLower().Contains(query) ||
                        cont.alternative_url.ToLower().Contains(query)
                        );

                    if(current_user != null)
                    {
                        user_shelves = user_shelves.Where(shelf => shelf.title.ToLower().Contains(query));
                        user_contents = user_contents.Where(
                            cont => cont.title.ToLower().Contains(query) ||
                            cont.url.ToLower().Contains(query) ||
                            cont.alternative_url.ToLower().Contains(query)
                            );
                    }
                }
                if(!string.IsNullOrWhiteSpace(filter_tag))
                {
                    filter_tag = filter_tag.Trim().ToLower();
                    //tag tg = db.tags.Where(t => t.tag1.Equals(filter_tag)).FirstOrDefault();
                    ViewBag.tag = filter_tag;
                    
                    public_shelves = public_shelves.Where(shelf => shelf.tags.Any(t => t.tag1.Equals(filter_tag)));
                    public_contents = public_contents.Where(cont => cont.tags.Any(t => t.tag1.Equals(filter_tag)));

                    if (current_user != null)
                    {
                        user_shelves = user_shelves.Where(shelf => shelf.tags.Any(t => t.tag1.Equals(filter_tag)));
                        user_contents = user_contents.Where(cont => cont.tags.Any(t => t.tag1.Equals(filter_tag)));
                    }
                    
                }

                if (!string.IsNullOrWhiteSpace(filter_type) && !filter_type.Equals("any"))
                {
                    filter_type = filter_type.Trim().ToLower();
                    ViewBag.filter = filter_type;
                    if (filter_type.Equals("shelf"))
                    {
                        public_contents = null;
                        user_contents = null;
                    }else if (filter_type.Equals("content"))
                    {
                        public_shelves = null;
                        user_shelves = null;
                    }
                    else
                    {
                        public_shelves = null;
                        user_shelves = null;

                        public_contents = public_contents.Where(cont => cont.type.Equals(filter_type));
                        if(current_user != null)
                        {
                            user_contents = user_contents.Where(cont => cont.type.Equals(filter_type));
                        }
                    }
                }
                
            }
            
            if(public_shelves != null && public_shelves.ToList().Count() > 0)
            {
                ViewBag.public_shelves = public_shelves.OrderByDescending(shelf => shelf.modification_datetime);
            }
            if(public_contents != null && public_contents.Count() > 0)
            {
                ViewBag.public_contents = public_contents.OrderByDescending(cont => cont.modification_datetime);
            }

            if(current_user != null)
            {
                if(user_shelves != null && user_shelves.Count() > 0) ViewBag.user_shelves = user_shelves.OrderByDescending(shelf => shelf.modification_datetime);
                if(user_contents != null && user_contents.Count() > 0) ViewBag.user_contents = user_contents.OrderByDescending(cont => cont.modification_datetime);
            }

            if (ViewBag.tag != null && ViewBag.type == null) ViewBag.type = "any";
            return View();
        }

        
    }
}