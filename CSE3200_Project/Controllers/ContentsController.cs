using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CSE3200_Project.Models;
using CSE3200_Project.Attributes;


namespace CSE3200_Project.Controllers
{
    [CurrentUser(login_required = true)]
    public class ContentsController : Controller
    {
        private DRIEntities db = new DRIEntities();

        // GET: Contents
        [CurrentUser]
        public ActionResult Index()
        {
            User current_user = (User)HttpContext.Items["current_user"];

            IQueryable<Content> contents;
            if (current_user != null)
            {
                contents = db.Contents.Include(c => c.User).Include(c => c.User1).Where(
                    c => c.creator_id == current_user.id
                    );
            }
            else
            {
                contents = db.Contents.Include(c => c.User).Include(c => c.User1).Where(
                    c => c.privacy.ToLower().Equals("public")
                    );
            }
            return View(contents.ToList());
        }

        // GET: Contents/Details/5
        [CurrentUser]
        public ActionResult Details(int? id)
        {
            User current_user = (User)HttpContext.Items["current_user"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = db.Contents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            if(content.privacy.ToLower().Equals("public") || (current_user != null && content.creator_id == current_user.id))
            {
                return View(content);
            }
            else
            {
                HttpCookie responseCookie = new HttpCookie("Message");
                responseCookie["message"] = "Not Authorized to view the target page! Please try again with an authorized account!";
                Response.Cookies.Add(responseCookie);
                Response.StatusCode = 403;
                if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.AbsoluteUri);
                else return Redirect("/");
            }
        }

        // GET: Contents/Create
        public ActionResult Create()
        {
            User current_user = (User)HttpContext.Items["current_user"];

            ViewBag.shelves = db.Shelves.Include(c => c.User).Where(c => c.creator_id == current_user.id);
            return View();
        }

        // POST: Contents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,details,url,alternative_url,type,privacy")] Content content)
        {
            User current_user = (User)HttpContext.Items["current_user"];

            if (ModelState.IsValid)
            {
                string[] shelves_ids = Request.Form.GetValues("shelves");
                if (shelves_ids != null && shelves_ids.Length > 0)
                {
                    ICollection<Shelf> shelves = new HashSet<Shelf>();
                    foreach (string shelf_id in shelves_ids)
                    {
                        Shelf shelf = db.Shelves.Find(int.Parse(shelf_id));
                        shelves.Add(shelf);
                    }
                    content.Shelves = shelves;
                }

                content.creation_datetime = DateTime.Now;
                content.modification_datetime = content.creation_datetime;
                content.creator_id = current_user.id;
                content.modifier_id = current_user.id;
                content.status = true;
                db.Contents.Add(content);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.shelves = db.Shelves.Include(c => c.User).Where(c => c.creator_id == current_user.id);
            return View(content);
        }

        // GET: Contents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = db.Contents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            User current_user = ((User)HttpContext.Items["current_user"]);
            ViewBag.unassociated_shelves = db.Shelves.Include(shelf => shelf.User).Where(shelf => shelf.creator_id == current_user.id);
            return View(content);
        }

        // POST: Contents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,details,url,alternative_url,type,privacy,status")] Content content)
        {
            if (ModelState.IsValid)
            {
                Content content_db = db.Contents.Find(content.id);
                if (content_db != null)
                {
                    string[] shelves_ids = Request.Form.GetValues("shelves");
                    if (shelves_ids != null && shelves_ids.Length > 0)
                    {
                        foreach (string shelf_id in shelves_ids)
                        {
                            Shelf shelf = db.Shelves.Find(int.Parse(shelf_id));
                            content.Shelves.Add(shelf);
                            if (!content_db.Shelves.Contains(shelf))
                            {
                                content_db.Shelves.Add(shelf);
                            }
                        }
                    }

                    foreach (Shelf shelf in content_db.Shelves.ToList())
                    {
                        if (!content.Shelves.Contains(shelf))
                        {
                            content_db.Shelves.Remove(shelf);
                        }
                    }
                    
                    content_db.modification_datetime = DateTime.Now;
                    content_db.modifier_id = ((User)HttpContext.Items["current_user"]).id;
                    content_db.title = content.title;
                    content_db.details = content.details;
                    content_db.url = content.url;
                    content_db.alternative_url = content.alternative_url;
                    content_db.type = content.type;
                    content_db.privacy = content.privacy;
                    content_db.status = content.status;

                    db.Entry(content_db).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            User current_user = ((User)HttpContext.Items["current_user"]);
            ViewBag.unassociated_shelves = db.Shelves.Include(shelf => shelf.User).Where(shelf => shelf.creator_id == current_user.id);
            return View(content);
        }

        // GET: Contents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = db.Contents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            return View(content);
        }

        // POST: Contents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Content content = db.Contents.Find(id);
            db.Contents.Remove(content);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
