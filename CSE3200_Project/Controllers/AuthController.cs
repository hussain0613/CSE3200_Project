using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSE3200_Project.Models;
using CSE3200_Project.Attributes;
using System.Data.Entity.Infrastructure;

namespace CSE3200_Project.Controllers
{
    [CurrentUser]
    public class AuthController : Controller
    {
        private DRIEntities db = new DRIEntities();

        // GET: Auth
        [HttpGet]
        [CurrentUser(anonymous_required = true)]
        public ActionResult Login()
        {
            return View();
        }

        // Post: Login
        [HttpPost]
        [CurrentUser(anonymous_required = true)]
        public ActionResult Login(LoginViewModel creds)
        {
            User user = db.Users.Where(u => u.username == creds.Username && u.password == creds.Password).FirstOrDefault();
            if (user != null)
            {
                HttpCookie token = new HttpCookie("token");
                token["username"] = user.username;
                token["id"] = user.id.ToString();
                token["user-agent"] = Request.UserAgent;
                if (creds.RememberMe)
                {
                    token.Expires = DateTime.Now;
                    token.Expires = token.Expires.AddMonths(3);
                }
                // DateTime d = DateTime.Now + new TimeSpan(0,1,0);

                string from = Request.QueryString["next"];
                Response.Cookies.Add(token);
                if (from != null)
                {
                    return Redirect(from);
                }
                return Redirect("/");
            }
            Response.StatusCode = 404;
            ViewBag.Message = "Invalid credentials!";

            return View();
        }

        [HttpGet]
        [CurrentUser(anonymous_required = true)]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [CurrentUser(anonymous_required = true)]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel reg_info)
        {
            if (ModelState.IsValid)
            {
                User user = new User();

                user.creation_datetime = DateTime.Now;
                user.name = reg_info.Name;
                user.email = reg_info.Email;
                user.username = reg_info.Username;
                user.password = reg_info.Password;
                user.role = "general";
                
                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }catch(DbUpdateException exc)
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
                            ViewBag.Message = "The username has been taken already!";

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

                HttpCookie message_cookie = new HttpCookie("Message");
                message_cookie["message"] = "User created Successfully. Please Login.";
                Response.Cookies.Add(message_cookie);
                return RedirectToAction("Login");
                
            }

            return View();
        }

        public ActionResult Logout()
        {
            if(HttpContext.Items["current_user"] != null)
                HttpContext.Items.Remove("current_user");
            HttpCookie token = new HttpCookie("token");
            token.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Set(token);

            return Redirect("/");
        }
    }
}
