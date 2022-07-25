using System;
using System.Web;
using System.Web.Mvc;

namespace CSE3200_Project.Models
{
    public class AuthHelpers
    {
        private DRIEntities db = new DRIEntities();
        HttpRequestBase Request;

        public AuthHelpers(HttpRequestBase req)
        {
            this.Request = req;
        }
        public bool isAuthenticated()
        {
            HttpCookie token = Request.Cookies.Get("token");
            if (token == null)
            {
                return false;
            }

            int? id = null;
            try
            {
                id = int.Parse(token["id"]);
            }
            catch
            {
                return false;
            }

            if (id == null)
            {
                return false;
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return false;
            }
            return true;
        }


        public User get_current_user()
        {
            HttpCookie token = Request.Cookies.Get("token");
            if (token == null)
            {
                return null;
            }

            int? id = null;
            try
            {
                id = int.Parse(token["id"]);
            }
            catch
            {
                return null;
            }

            if (id == null)
            {
                return null;
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return null;
            }

            return user;
        }
    }
}