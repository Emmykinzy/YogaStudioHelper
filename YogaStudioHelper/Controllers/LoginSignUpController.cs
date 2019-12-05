using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YogaStudioHelper.Controllers
{
    public class LoginSignUpController : Controller
    {
        [HttpGet]
        public ActionResult LogInSignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogInSignUp(FormCollection collection)
        {
             
            string val = collection["Access"];
            if(val.Equals("Admin"))
            {
                Session["Auth"] = 1;
            }
            else if(val.Equals("Teacher"))
            {
                Session["Auth"] = 2;
            }
            else if(val.Equals("Receptionist"))
            {
                Session["Auth"] = 3;
            }
            else if(val.Equals("Student"))
            {
                Session["Auth"] = 4;
            }
            else if (val.Equals("None"))
            {
                Session["Auth"] = null;
            }

            return RedirectToAction("Homepage", "Home");
        }

        public ActionResult LogOut()
        {
            Session["Auth"] = null;

            return RedirectToAction("Homepage", "Home");
        }
    }
}