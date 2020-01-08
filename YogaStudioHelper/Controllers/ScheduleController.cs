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
        [HttpGet]
        public ActionResult Schedule()
        {
            IEnumerable<Schedule> list = db.getSchedulesNext7Days();

            IEnumerable<Schedule> orderedList = (from schedule in list
                                                 orderby schedule.Class_Date
                                                 orderby schedule.Start_Time
                                                 select schedule);

            return View(orderedList);
        }

        [HttpPost]
        public ActionResult Schedule(FormCollection collection)
        {
      

            int scheduleId = Convert.ToInt32(collection["scheduleId"]);

            // call create class log method 

            // call increment schedule count 
            db.ScheduleSignUp(scheduleId);


            int userId = Convert.ToInt32(Session["Uid"]);



            Class_Log newClassLog = new Class_Log();

            newClassLog.Schedule_Id = scheduleId;

            newClassLog.U_Id = userId;

            // incase not null etc 
            //newClassLog.Log_Status = "Upcoming";

            db.CreateClass_Log(newClassLog); 

            //db.DeleteAllClass_Log();
            

            // Create class log 




            return RedirectToAction("Schedule");
        }

            public ActionResult ClassSignUp(FormCollection collection)
        {


            var test = 1;

            var test2 = TempData["ScheduleId"];

            var test3 = collection["scheduleId"];


          



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