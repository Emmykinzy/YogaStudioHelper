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
            IEnumerable<Schedule> list = db.getSchedulesNext7Days().Where(x => x.Schedule_Status == "ACTIVE");

            IEnumerable<Schedule> orderedList = (from schedule in list
                                                 orderby schedule.Class_Date
                                                 orderby schedule.Start_Time
                                                 select schedule);

            return View(orderedList);
        }

       // [Filters.AuthorizeStudent]
        [HttpPost]
        public ActionResult Schedule(FormCollection collection)
        {
            if (collection["next"] == null && collection["back"] == null)
            {
                int scheduleId = Convert.ToInt32(collection["scheduleId"]);
                if (Session["Auth"] == null)
                {
                    return RedirectToAction("LogInSignUp", "LoginSignUp");
                }
                if ((int)Session["Auth"] == 4 || (int)Session["Auth"] == 2)
                {

                    int userId = Convert.ToInt32(Session["Uid"]);


                    var yogaUser = db.getUserById(userId);
                    var sched = db.getScheduleById(scheduleId);

                    // Validate if student already sign in  
                    var checkIfSignin = db.CheckIfSignedUp(scheduleId, userId);
                    if(sched.Class_Date < DateTime.Now.Date)
                    {
                        TempData["Message"] = "Error: Class is no longer available";
                        return RedirectToAction("Schedule");
                    }
                    if (checkIfSignin)
                    {
                        //TempData["Message"] = "<h5 style=\"color:red;\">Error: You are already signed up to this course</h5>";
                        TempData["Message"] = "Error: You have already signed up to this course";
                        return RedirectToAction("Schedule");

                    }
                    // Validation: student has passes 
                    if (yogaUser.U_Tokens < 1)
                    {
                        //TempData["Message"] = "<h5 style=\"color:red;\">Error: Out of passes</h5>";
                        TempData["Message"] = "Error: Out of passes";
                        return RedirectToAction("Schedule");
                    }
                    // Validation: classrom has places 
                    var room = db.getRoom(sched.Room_Id);
                    if (db.getSignedUp(sched.Schedule_Id) >= room.Room_Capacity)
                    {
                        //TempData["Message"] = "<h5 style=\"color:red;\">Error: Sorry this class is full</h5>";
                        TempData["Message"] = "Error: Sorry this class is full";
                        return RedirectToAction("Schedule");
                    }

                    db.ScheduleSignUp(scheduleId);
                    // Creating Class log 

                    db.CreateClass_Log(scheduleId, userId);

                    db.RemoveToken(userId);



                    //Message
                    //TempData["Message"] = "<h5>Sucessfully Enrolled</h5>";
                    TempData["Message"] = "Sucessfully Enrolled";

                    return RedirectToAction("Schedule");
                }
                else
                {
                    TempData["class"] = scheduleId;
                    List<Yoga_User> yu = db.getScheduleSignUpList(scheduleId);
                    TempData["yu"] = yu;
                    return RedirectToAction("StudentSignIn");

                }
            }
            else
            {

                //if (collection["next"] != null)
                //{
                //    IEnumerable<Schedule> list;
                //    if (ViewBag.date == null && TempData["date"] == null)
                //    {
                //        ViewBag.date = DateTime.Now.AddDays(7);
                //        list = db.getSchedulesNextWeek(DateTime.Now);
                //    }
                //    else
                //    {
                //        DateTime date = (DateTime)TempData["date"];
                //        list = db.getSchedulesNextWeek(date);
                //        ViewBag.date = date.AddDays(7);
                //    }
                //    orderedList = (from schedule in list
                //                   orderby schedule.Class_Date
                //                   orderby schedule.Start_Time
                //                   select schedule);
                //}
                //else
                //{
                //    IEnumerable<Schedule> list;
                //    if (ViewBag.date == null && TempData["date"] == null)
                //    {
                //        ViewBag.date = DateTime.Now.AddDays(-7);
                //         list = db.getSchedulesBackWeek(DateTime.Now);
                //    }
                //    else
                //    {
                //        DateTime date = (DateTime)TempData["date"];
                //        list = db.getSchedulesBackWeek(date);
                //        ViewBag.date = date.AddDays(-7);
                //    }

                //    orderedList = (from schedule in list
                //                   orderby schedule.Class_Date
                //                   orderby schedule.Start_Time
                //                   select schedule);
                //}
                //    return View(orderedList);

                IEnumerable<Schedule> list;
 

                if (collection["next"] != null)
                {
                    int position = Int32.Parse(collection["position"]);
                    list = db.getSchedulesNextWeek(DateTime.Now.AddDays(position));
                    position += 7;
                    TempData["position"] = position;
                    TempData["date"] = DateTime.Now.AddDays(position);

                    return View(list);

                }
                


                if (collection["back"] != null)
                {
                    int position = Int32.Parse(collection["position"]);
                    list = db.getSchedulesBackWeek(DateTime.Now.AddDays(position));
                    position -= 7;
                    TempData["position"] = position;
                    TempData["date"] = DateTime.Now.AddDays(position);

                    return View(list);

                }

                IEnumerable<Schedule> sche = db.getSchedulesNext7Days().Where(x => x.Schedule_Status == "ACTIVE");
                IEnumerable<Schedule> orderedList = (from schedule in sche
                                                     orderby schedule.Class_Date
                                                     orderby schedule.Start_Time
                                                     select schedule);
                return View(orderedList);

            }
        }

        [HttpGet]
        public ActionResult StudentSignIn()
        {
            int scheduleId = (int)TempData["class"];
            List<Yoga_User> yu = (List<Yoga_User>)TempData["yu"];

            TempData["class"] = db.getScheduleById(scheduleId);

            yu = yu.OrderBy(x => x.U_Last_Name).ThenBy(x => x.U_First_Name).ToList();
            
            return View(yu);
        }

        [HttpPost]
        public ActionResult StudentSignIn(FormCollection form)
        {
            int scheduleId = (int)TempData["class"];
            TempData["class"] = db.getScheduleById(scheduleId);
            List<Yoga_User> yu = db.getScheduleSignUpList(scheduleId);

            foreach(Yoga_User user in yu)
            {
                 
                String attendance = form[user.U_Id.ToString()];
                db.changeClass_LogStatus(user.U_Id, scheduleId, attendance);
   
            }
            yu = yu.OrderBy(x => x.U_Last_Name).ThenBy(x => x.U_First_Name).ToList();
            return View(yu);

        }
    }
}