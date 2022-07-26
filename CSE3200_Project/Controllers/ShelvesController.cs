﻿using System;
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
    public class ShelvesController : Controller
    {
        private DRIEntities db = new DRIEntities();

        // GET: Shelves
        [CurrentUser]
        public ActionResult Index()
        {
            User current_user = (User)HttpContext.Items["current_user"];

            IQueryable<Shelf> shelves;
            if (current_user != null)
            {
                shelves = db.Shelves.Include(s => s.User).Include(s => s.User1).Where(
                    s => s.creator_id == current_user.id
                    );
            }
            else
            {
                shelves = db.Shelves.Include(s => s.User).Include(s => s.User1).Where(
                    s => s.privacy.ToLower().Equals("public")
                    );
            }
            return View(shelves.ToList());
        }

        // GET: Shelves/Details/5
        [CurrentUser]
        public ActionResult Details(int? id)
        {
            User current_user = (User)HttpContext.Items["current_user"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shelf shelf = db.Shelves.Find(id);
            if (shelf == null)
            {
                return HttpNotFound();
            }
            if (shelf.privacy.ToLower().Equals("public") || (current_user != null && shelf.creator_id == current_user.id))
            {
                return View(shelf);
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

        // GET: Shelves/Create
        public ActionResult Create()
        {
            User current_user = (User)HttpContext.Items["current_user"];
            ViewBag.contents = db.Contents.Where(content => content.creator_id == current_user.id);
            return View();
        }

        // POST: Shelves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,details,privacy")] Shelf shelf)
        {
            User current_user = (User)HttpContext.Items["current_user"];
            if (ModelState.IsValid)
            {
                string[] contents_ids = Request.Form.GetValues("contents");
                if (contents_ids != null && contents_ids.Length > 0)
                {
                    foreach (string content_id in contents_ids)
                    {
                        Content content = db.Contents.Find(int.Parse(content_id));
                        shelf.Contents.Add(content);
                    }
                }

                // comma separated tags
                string tags = Request.Form.Get("tags");
                if (tags != null && tags.Length > 0)
                {
                    ICollection<tag> tags_list = new HashSet<tag>();
                    string[] tags_array = tags.Split(',');
                    foreach (string tag_name in tags_array)
                    {
                        string trimmed_lowerd_tag = tag_name.Trim().ToLower();
                        tag tag = db.tags.Where(t => t.tag1.ToLower().Equals(trimmed_lowerd_tag)).FirstOrDefault();
                        if (tag == null)
                        {
                            tag = new tag();
                            tag.tag1 = trimmed_lowerd_tag;
                            db.tags.Add(tag);
                        }
                        tags_list.Add(tag);
                    }
                    shelf.tags = tags_list;
                }

                shelf.creation_datetime = DateTime.Now;
                shelf.modification_datetime = shelf.creation_datetime;
                shelf.creator_id = current_user.id;
                shelf.modifier_id = current_user.id;
                shelf.status = true;
                db.Shelves.Add(shelf);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.contents = db.Contents.Where(content => content.creator_id == current_user.id);
            return View(shelf);
        }

        // GET: Shelves/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shelf shelf = db.Shelves.Find(id);
            if (shelf == null)
            {
                return HttpNotFound();
            }
            User current_user = ((User)HttpContext.Items["current_user"]);
            if (shelf.creator_id != current_user.id)
            {
                HttpCookie responseCookie = new HttpCookie("Message");
                responseCookie["message"] = "Not Authorized to view the target page! Please try again with an authorized account!";
                Response.Cookies.Add(responseCookie);
                Response.StatusCode = 403;
                if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.AbsoluteUri);
                else return Redirect("/");
            }
            ViewBag.unassociated_contents = db.Contents.Include(content => content.User).Where(content => content.creator_id == current_user.id);
            return View(shelf);
        }

        // POST: Shelves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,details,privacy,status")] Shelf shelf)
        {
            User current_user = ((User)HttpContext.Items["current_user"]);
            if (ModelState.IsValid)
            {
                Shelf shelf_db = db.Shelves.Find(shelf.id);
                if (shelf_db != null)
                {
                    if (shelf_db.creator_id != current_user.id)
                    {
                        HttpCookie responseCookie = new HttpCookie("Message");
                        responseCookie["message"] = "Not Authorized to view the target page! Please try again with an authorized account!";
                        Response.Cookies.Add(responseCookie);
                        Response.StatusCode = 403;
                        if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.AbsoluteUri);
                        else return Redirect("/");
                    }

                    string[] contents_ids = Request.Form.GetValues("contents");
                    if (contents_ids != null && contents_ids.Length > 0)
                    {
                        foreach (string content_id in contents_ids)
                        {
                            Content content = db.Contents.Find(int.Parse(content_id));
                            shelf.Contents.Add(content);
                            if (!shelf_db.Contents.Contains(content))
                            {
                                shelf_db.Contents.Add(content);
                            }
                        }
                    }

                    foreach (Content content in shelf_db.Contents.ToList())
                    {
                        if (!shelf.Contents.Contains(content))
                        {
                            shelf_db.Contents.Remove(content);
                        }
                    }

                    string tags = Request.Form.Get("tags");
                    if (tags != null && tags.Length > 0)
                    {
                        string[] tags_array = tags.Split(',');
                        foreach (string tag_name in tags_array)
                        {
                            string trimmed_lowerd_tag = tag_name.Trim().ToLower();
                            tag tg = db.tags.Where(t => t.tag1.ToLower().Equals(trimmed_lowerd_tag)).FirstOrDefault();
                            if (tg == null)
                            {
                                tg = new tag();
                                tg.tag1 = trimmed_lowerd_tag;
                                db.tags.Add(tg);
                            }
                            shelf.tags.Add(tg);
                            if (!shelf_db.tags.Contains(tg))
                            {
                                shelf_db.tags.Add(tg);
                            }
                        }
                    }

                    foreach (tag tg in shelf_db.tags.ToList())
                    {
                        if (!shelf.tags.Contains(tg))
                        {
                            shelf_db.tags.Remove(tg);
                        }
                    }


                    shelf_db.modifier_id = ((User)HttpContext.Items["current_user"]).id;
                    shelf_db.title = shelf.title;
                    shelf_db.details = shelf.details;
                    shelf_db.privacy = shelf.privacy;
                    shelf_db.status = shelf.status;

                    db.Entry(shelf_db).State = EntityState.Modified;
                    
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.unassociated_contents = db.Contents.Include(content => content.User).Where(content => content.creator_id == current_user.id);
            return View(shelf);
        }
        

        // GET: Shelves/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shelf shelf = db.Shelves.Find(id);
            if (shelf == null)
            {
                return HttpNotFound();
            }
            User current_user = ((User)HttpContext.Items["current_user"]);
            if (shelf.creator_id != current_user.id)
            {
                HttpCookie responseCookie = new HttpCookie("Message");
                responseCookie["message"] = "Not Authorized to view the target page! Please try again with an authorized account!";
                Response.Cookies.Add(responseCookie);
                Response.StatusCode = 403;
                if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.AbsoluteUri);
                else return Redirect("/");
            }
            return View(shelf);
        }

        // POST: Shelves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shelf shelf = db.Shelves.Find(id);
            
            User current_user = ((User)HttpContext.Items["current_user"]);
            if (shelf.creator_id != current_user.id)
            {
                HttpCookie responseCookie = new HttpCookie("Message");
                responseCookie["message"] = "Not Authorized to view the target page! Please try again with an authorized account!";
                Response.Cookies.Add(responseCookie);
                Response.StatusCode = 403;
                if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.AbsoluteUri);
                else return Redirect("/");
            }

            db.Shelves.Remove(shelf);
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
