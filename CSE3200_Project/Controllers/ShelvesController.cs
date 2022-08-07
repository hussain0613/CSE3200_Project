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
    [CurrentUser]
    public class ShelvesController : Controller
    {
        private DRIEntities db = new DRIEntities();

        // GET: Shelves
        public ActionResult Index()
        {
            var shelves = db.Shelves.Include(s => s.User).Include(s => s.User1);
            return View(shelves.ToList());
        }

        // GET: Shelves/Details/5
        public ActionResult Details(int? id)
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
            return View(shelf);
        }

        // GET: Shelves/Create
        public ActionResult Create()
        {
            ViewBag.creator_id = new SelectList(db.Users, "id", "name");
            ViewBag.modifier_id = new SelectList(db.Users, "id", "name");
            return View();
        }

        // POST: Shelves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,creator_id,creation_datetime,modifier_id,modification_datetime,title,details,privacy,status")] Shelf shelf)
        {
            if (ModelState.IsValid)
            {
                db.Shelves.Add(shelf);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.creator_id = new SelectList(db.Users, "id", "name", shelf.creator_id);
            ViewBag.modifier_id = new SelectList(db.Users, "id", "name", shelf.modifier_id);
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
            ViewBag.creator_id = new SelectList(db.Users, "id", "name", shelf.creator_id);
            ViewBag.modifier_id = new SelectList(db.Users, "id", "name", shelf.modifier_id);
            return View(shelf);
        }

        // POST: Shelves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,creator_id,creation_datetime,modifier_id,modification_datetime,title,details,privacy,status")] Shelf shelf)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shelf).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.creator_id = new SelectList(db.Users, "id", "name", shelf.creator_id);
            ViewBag.modifier_id = new SelectList(db.Users, "id", "name", shelf.modifier_id);
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
            return View(shelf);
        }

        // POST: Shelves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shelf shelf = db.Shelves.Find(id);
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
