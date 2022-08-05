﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSE3200_Project.Models;

namespace CSE3200_Project.Controllers
{
    public class AuthController : Controller
    {
        private DRIEntities db = new DRIEntities();

        // GET: Auth
        [HttpGet]
        public ActionResult Login()
        {
            if (Request.Cookies["Message"] != null && Request.Cookies["Message"]["message"] != null)
            {
                ViewBag.Message = Request.Cookies["Message"]["message"];
                Request.Cookies.Remove("Message");
            }

            AuthHelpers ah = new AuthHelpers(Request);
            User cu = ah.get_current_user();
            ViewBag.IsAuthenticated = false;
            if (cu != null)
            {
                ViewBag.current_user = cu;
                ViewBag.IsAuthenticated = true;
            }

            return View();
        }

        // Post: Login
        [HttpPost]
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
                Response.Cookies.Add(token);
                return RedirectToAction("Profile");
            }
            Response.StatusCode = 404;
            ViewBag.Message = "User not Found!";

            AuthHelpers ah = new AuthHelpers(Request);
            User cu = ah.get_current_user();
            ViewBag.IsAuthenticated = false;
            if (cu != null)
            {
                ViewBag.current_user = cu;
                ViewBag.IsAuthenticated = true;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (Request.Cookies["Message"] != null && Request.Cookies["Message"]["message"] != null)
            {
                ViewBag.Message = Request.Cookies["Message"]["message"];
                Request.Cookies.Remove("Message");
            }

            AuthHelpers ah = new AuthHelpers(Request);
            User cu = ah.get_current_user();
            ViewBag.IsAuthenticated = false;
            if (cu != null)
            {
                ViewBag.current_user = cu;
                ViewBag.IsAuthenticated = true;
            }

            return View();
        }

        [HttpPost]
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

            AuthHelpers ah = new AuthHelpers(Request);
            User cu = ah.get_current_user();
            ViewBag.IsAuthenticated = false;
            if (cu != null)
            {
                ViewBag.current_user = cu;
                ViewBag.IsAuthenticated = true;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Profile()
        {
            AuthHelpers authHelpers = new AuthHelpers(Request);
            User current_user = authHelpers.get_current_user();
            if(current_user == null)
            {
                Request.Cookies.Remove("token");
                Response.Cookies["Message"]["message"] = "Invalid token. Please Login First!";
                return Redirect("/auth/login");
            }
            if (Request.Cookies["Message"] != null && Request.Cookies["Message"]["message"] != null)
            {
                ViewBag.Message = Request.Cookies["Message"]["message"];
                Request.Cookies.Remove("Message");
            }

            AuthHelpers ah = new AuthHelpers(Request);
            User cu = ah.get_current_user();
            ViewBag.IsAuthenticated = false;
            if (cu != null)
            {
                ViewBag.current_user = cu;
                ViewBag.IsAuthenticated = true;
            }

            return View(current_user);
        }

        public ActionResult Logout()
        {
            HttpCookie token = new HttpCookie("token");
            token.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Set(token);

            AuthHelpers ah = new AuthHelpers(Request);
            User cu = ah.get_current_user();
            ViewBag.IsAuthenticated = false;
            if (cu != null)
            {
                ViewBag.current_user = cu;
                ViewBag.IsAuthenticated = true;
            }

            return Redirect("/");
        }
    }
}