using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSE3200_Project.Models;
using CSE3200_Project.Attributes;

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
                token.Expires.AddHours(1);
                // DateTime d = DateTime.Now + new TimeSpan(0,1,0);

                string from = Request.QueryString["next"];
                Response.Cookies.Add(token);
                if (from != null)
                {
                    return Redirect(from);
                }
                return Redirect($"/auth/profile?from={from}");
            }
            Response.StatusCode = 404;
            ViewBag.Message = "User not Found!";

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
            if(reg_info.Password != reg_info.ConfirmPassword)
            {
                ViewBag.Message = $"Passwords do not match!";
                return View();
            }
            //ViewBag.Message = $"Created new user: {reg_info.Username}";
            if (ModelState.IsValid)
            {
                User user = new User();

                user.name = reg_info.Name;
                user.email = reg_info.Email;
                user.username = reg_info.Username;
                user.password = reg_info.Password;
                user.role = "general";
                
                db.Users.Add(user);
                db.SaveChanges();

                HttpCookie message_cookie = new HttpCookie("Message");
                message_cookie["message"] = "User created Successfully. Please Login.";
                Response.Cookies.Add(message_cookie);
                return RedirectToAction("Login");
            }

            return View();
        }

        [CurrentUser(login_required = true)]
        [HttpGet]
        public ActionResult Profile()
        {
            User current_user = (User)HttpContext.Items["current_user"];
            return View(current_user);
        }

        public ActionResult Logout()
        {
            HttpCookie token = new HttpCookie("token");
            token.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Set(token);

            return Redirect("/");
        }
    }
}
