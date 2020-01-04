using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Database;

namespace YogaStudioHelper.Controllers
{
    public class ScheduleController : Controller
    {
        DBMaster db = new DBMaster();
        // GET: Schedule
        public ActionResult Schedule()
        {
            IEnumerable<Schedule> list = db.getSchedulesNext7Days();

            IEnumerable<Schedule> orderedList = (from schedule in list
                                                 orderby schedule.Class_Date
                                                 orderby schedule.Start_Time
                                                 select schedule);

            return View(orderedList);
        }

        public ActionResult ClassSignUp()
        {


            var test = 1; 
            //Need to check student passes available 
            
            //if so call db method 

            // increment sign up to schedule 

            // Create class log 
            // add upcoming class to the student profile 
            // remove 1 pass from the student 
            


            return View();
        }
    }
}