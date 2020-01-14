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

            /*
            // For testing purposes teachers
            Session["Auth"] = 2;
            Session["Uid"] = 2;
            */

            //Receptionist
            //Session["Auth"] = 3;

            //Admin
            //Session["Auth"] = 1;


            // student  
             //Session["Auth"] = 4;

            //yoga user id 4 is a student id
           // Session["Uid"] = 4;

            return View(); 
        }

    }
}