using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Database;

namespace YogaStudioHelper.Controllers
{
    public class HomeController : Controller
    {
        DBMaster db = new DBMaster();
        [HttpGet]
        public ActionResult Homepage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Homepage(FormCollection collection)
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
            return View();
        }
    }
}