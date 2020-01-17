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
            int userId = Convert.ToInt32(Session["Uid"]);

 
            var yogaUser = db.getUserById(userId);
            var sched = db.getScheduleById(scheduleId);

            // Validate if student already sign in  
            var checkIfSignin = db.CheckIfSignedUp(scheduleId, userId);

            if (checkIfSignin)
            {
                TempData["Message"] = "<h5 style=\"color:red;\">Error: You are already signed up to this course</h5>";
                return RedirectToAction("Schedule");

            }
            // Validation: student has passes 
            if(yogaUser.U_Tokens < 1)
            {
                TempData["Message"] = "<h5 style=\"color:red;\">Error: Out of passes</h5>";
                return RedirectToAction("Schedule");
            }
            // Validation: classrom has places 
            var room = db.getRoom(sched.Room_Id); 
            if(sched.Signed_Up >= room.Room_Capacity)
            {
                TempData["Message"] = "<h5 style=\"color:red;\">Error: Sorry this class is full</h5>";
                return RedirectToAction("Schedule");
            }

            db.ScheduleSignUp(scheduleId);
            // Creating Class log 

            db.CreateClass_Log(scheduleId, userId);

            db.RemoveToken(userId);

 

            //Message
            ViewBag.Message = "<h5>Sucessfully Enrolled</h5>";

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