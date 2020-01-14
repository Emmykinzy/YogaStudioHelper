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
            var yogaUser = db.getUserById(userId);
            var sched = db.getScheduleById(scheduleId);

            // Validate if student already sign in  
            var checkIfSignin = db.CheckIfSignIn(scheduleId, userId);

            if (checkIfSignin)
            {
                TempData["Message"] = "Error: You are already sign to this course";
                return RedirectToAction("Schedule");

            }

            // Validation: student has passes 
            if(yogaUser.U_Tokens < 1)
            {
                TempData["Message"] = "Error: The course required a passe";
                return RedirectToAction("Schedule");
            }
            // Validation: classrom has places 
            var room = db.getRoom(sched.Room_Id); 
            if(sched.Signed_Up >= room.Room_Capacity)
            {
                TempData["Message"] = "Error: Sorry this class is full";
                return RedirectToAction("Schedule");
            }

            // Creating Class log 
            Class_Log newClassLog = new Class_Log();

            newClassLog.Schedule_Id = scheduleId;

            newClassLog.U_Id = userId;

        
            //newClassLog.Log_Status = "Upcoming";

            db.CreateClass_Log(newClassLog);

            // remove token 
            db.RemoveToken(userId);

 

            //Message
            ViewBag.Message = "Sucessfully Enrolled";

            Response.Write("<script language=javascript>alert('Succesfully Enroll');</script>");

            // window.location='~/Schedule/Schedule'
            Response.Write("<script>alert('" + "Test" + "')</script>");
            TempData["Message"] = "Error: The course required a passe";
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