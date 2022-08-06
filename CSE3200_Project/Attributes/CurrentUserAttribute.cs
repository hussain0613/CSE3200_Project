using System;
using System.Web;
using System.Web.Mvc;
using CSE3200_Project.Models;

namespace CSE3200_Project.Attributes
{
    public class CurrentUserAttribute : FilterAttribute, IAuthorizationFilter
    {
        private DRIEntities db = new DRIEntities();
        public bool login_required { get; set; } = false;
        public bool anonymous_required { get; set; } = false;
        public string[] roles_required { get; set; } = { };

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpCookie token = filterContext.HttpContext.Request.Cookies.Get("token");
            
            if(token != null)
            {
                int id = int.Parse(token["id"]);
                User user = db.Users.Find(id);
                if(user != null)
                {
                    filterContext.HttpContext.Items["current_user"] = user;
                    
                    if (anonymous_required)
                    {
                        HttpCookie responseCookie = new HttpCookie("Message");
                        responseCookie["message"] = "You are already logged in!";
                        filterContext.HttpContext.Response.Cookies.Add(responseCookie);
                        filterContext.HttpContext.Response.StatusCode = 400;
                        filterContext.Result = new RedirectResult("/");
                        return;
                    }
                    
                    if (roles_required.Length > 0 && Array.IndexOf(roles_required, user.role.ToLower()) < 0)
                    {
                        HttpCookie responseCookie = new HttpCookie("Message");
                        responseCookie["message"] = "Not Authorized to view the target page!";
                        filterContext.HttpContext.Response.Cookies.Add(responseCookie);
                        filterContext.HttpContext.Response.StatusCode = 403;
                        filterContext.Result = new RedirectResult("/");
                    }
                    return;
                }
            }

            if (login_required)
            {
                HttpCookie responseCookie = new HttpCookie("Message");
                responseCookie["message"] = "To view this page you need to Login first!";
                filterContext.HttpContext.Response.Cookies.Add(responseCookie);
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.Result = new RedirectResult($"/auth/login/?next={filterContext.HttpContext.Request.Url}");
            }
        }

        public CurrentUserAttribute()
        {
        }

        public CurrentUserAttribute(string role)
        {
            login_required = true;
            roles_required = new string[] { role };
        }
        public CurrentUserAttribute(string[] rr)
        {
            login_required = true;
            roles_required = rr;
        }
    }
}