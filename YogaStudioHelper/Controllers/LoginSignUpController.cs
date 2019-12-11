using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YogaStudioHelper.ViewModels;

namespace YogaStudioHelper.Controllers
{
    
    public class LoginSignUpController : Controller
    {
        DBMaster db = new DBMaster();
        [HttpGet]
        public ActionResult LogInSignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogInSignUp(FormCollection collection)
        {
             
            string email = collection["Email"];
            string pass = collection["Password"];

            bool valid = db.ValidateUser(email, pass);

            if(valid)
            {
                IEnumerable<Yoga_User> list = db.getUserByEmail(email);
                int id = list.First().Roles_Id;
                string roleName = db.getRoleName(id);
                if (roleName.Equals("ADMINISTRATOR"))
                {
                    Session["Auth"] = 1;
                }
                else if (roleName.Equals("TEACHER"))
                {
                    Session["Auth"] = 2;
                }
                else if (roleName.Equals("RECEPTIONIST"))
                {
                    Session["Auth"] = 3;
                }
                else if (roleName.Equals("STUDENT"))
                {
                    Session["Auth"] = 4;
                }
                else 
                {
                    Session["Auth"] = null;
                }

                return RedirectToAction("Homepage", "Home");
            }
            else
            {
                ViewBag.message = "Invalid Login Credentials";
                return View();
            }
           

           
        }

        public ActionResult LogOut()
        {
            Session["Auth"] = null;

            return RedirectToAction("Homepage", "Home");
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(LoginVM loginVM)
        {
            return View();
        }
    }
}