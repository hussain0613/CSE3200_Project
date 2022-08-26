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
                responseCookie["message"] = "Not Authorized to view the target page!";
                Response.Cookies.Add(responseCookie);
                Response.StatusCode = 403;
                return Redirect("/");
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

                // comma separated tags
                string tags = Request.Form.Get("tags");
                if (tags != null && tags.Length > 0)
                {
                    ICollection<tag> tags_list = new HashSet<tag>();
                    string[] tags_array = tags.Split(',');
                    foreach (string tag_name in tags_array)
                    {
                        tag tag = db.tags.Where(t => t.tag1.ToLower().Equals(tag_name.Trim().ToLower())).FirstOrDefault();
                        if (tag == null)
                        {
                            tag = new tag();
                            tag.tag1 = tag_name;
                            db.tags.Add(tag);
                        }
                        tags_list.Add(tag);
                    }
                    content.tags = tags_list;
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
            ViewBag.creator_id = new SelectList(db.Users, "id", "name", content.creator_id);
            ViewBag.modifier_id = new SelectList(db.Users, "id", "name", content.modifier_id);
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
                content.modification_datetime = DateTime.Now;
                content.modifier_id = ((User)HttpContext.Items["current_user"]).id;
                db.Entry(content).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.creator_id = new SelectList(db.Users, "id", "name", content.creator_id);
            ViewBag.modifier_id = new SelectList(db.Users, "id", "name", content.modifier_id);
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
