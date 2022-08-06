using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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

        private DRIEntities db = new DRIEntities();

        // GET: Profile
        [HttpGet]
        public ActionResult Index()
        {
            User current_user = (User)HttpContext.Items["current_user"];
            return View(current_user);
        }

        // GET: Profile/Edit/
        public ActionResult Edit()
        {
            User user = (User)HttpContext.Items["current_user"];
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "name,username,email,password")] User user)
        {
            if (ModelState.IsValid)
            {
                User current_user = (User)HttpContext.Items["current_user"];
                User db_user = db.Users.Find(current_user.id);
                if (db_user != null)
                {
                    db_user.modification_datetime = DateTime.Now;
                    db_user.name = user.name;
                    db_user.username = user.username;
                    db_user.email = user.email;
                    db_user.password = user.password;

                    try
                    {
                        db.SaveChanges();
                        HttpContext.Items["current_user"] = db_user;
                    }
                    catch (DbUpdateException exc)
                    {
                        string exc_msg = exc.InnerException.InnerException.Message;
                        if (exc_msg.Contains("Violation of UNIQUE KEY constraint"))
                        {
                            if (exc_msg.Contains("uq_User_email"))
                            {
                                ViewBag.Message = "There's already an account with the given email address!";
                            }
                            else if (exc_msg.Contains("uq_User_username"))
                            {
                                ViewBag.Message = "The username's been taken already!";

                            }
                            else
                            {
                                ViewBag.Message = exc_msg;
                            }
                            Response.StatusCode = 409;
                            return View();
                        }
                        else
                        {
                            ViewBag.Message = exc_msg;
                            Response.StatusCode = 418;
                            return View();
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    HttpCookie cookie = new HttpCookie("Message");
                    cookie["message"] = "User not found!";
                    Response.StatusCode = 404;
                    return Redirect("/");
                }
            }
            return View(user);
        }

    }
}