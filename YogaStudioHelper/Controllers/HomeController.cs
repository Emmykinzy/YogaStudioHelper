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

            
            // For testing purposes teachers
            //Session["Auth"] = 3;
            //Session["Uid"] = 3;
            

            //Receptionist
            //Session["Auth"] = 3;

            //Admin
            
            Session["Auth"] = 1;


            // student  
            //Session["Auth"] = 4;
            //Session["Uid"] = 4;

            //yoga user id 4 is a student id
            //Session["Uid"] = 5;



            // for upcoming classes feature 

            IEnumerable<Schedule> list = db.getSchedulesNext7Days();

            IEnumerable<Schedule> orderedList = (from schedule in list
                                                 orderby schedule.Class_Date
                                                 orderby schedule.Start_Time
                                                 select schedule);
            List<Schedule> weekList = orderedList.Where(x => x.Class_Date.Date >= DateTime.Now && x.Class_Date.Date <= DateTime.Now.AddDays(6)).ToList();

            ViewBag.UpcomingClasses = weekList; 

            return View(weekList); 
        }

        public ActionResult MessageView()
        {
            return View();
        }

    }
}