﻿using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace YogaStudioHelper.Controllers
{
    public class ProfileController : Controller
    {
        DBMaster db = new DBMaster();

        [Filters.AuthorizeStudent]
        public ActionResult Classes()
        {
          //  var  c = db.getScheduleById(2);
           // c.Cl
            return View();
        }


        // test class log 
        [Filters.AuthorizeStudent]
        public ActionResult ClassLogList()
        {
            IEnumerable<Class_Log> list = db.GetClass_LogsByUId((int)Session["Uid"]).Where(x => x.Schedule.Class_Date >= DateTime.Now.Date);

            List<Class_Log> newList = new List<Class_Log>();
            foreach(Class_Log log in list)
            {
                if(log.Schedule.Class_Date == DateTime.Now.Date)
                {
                    if(log.Schedule.Start_Time.Hours > DateTime.Now.Hour)
                    {
                        newList.Add(log);
                    }
                    else if(log.Schedule.Start_Time.Hours == DateTime.Now.Hour)
                    {
                        if(log.Schedule.Start_Time.Minutes > DateTime.Now.Minute)
                        {
                            newList.Add(log);
                        }
                    }
                }
                else 
                {
                    newList.Add(log);
                }
            }
    
            return View(newList.OrderByDescending(x => x.Schedule.Class_Date).OrderByDescending(x => x.Schedule.Start_Time).Take(10));


        }

        [HttpPost]
        public ActionResult ClassLogList(FormCollection form)
        {
            IEnumerable<Class_Log> logList = db.GetClass_LogsByUId((int)Session["Uid"]);


            IEnumerable<Class_Log> newList;
            if (form["back"] == null)
            {
                if (form["position"] == null)
                {
                    newList = logList.Skip(1 * 10).Take(10);
                    TempData["position"] = 2;
                }
                else
                {
                    int position = Int32.Parse(form["position"]);
                    newList = logList.Skip(position * 10).Take(10);
                    TempData["position"] = position + 1;
                }

            }
            else
            {

                int position = Int32.Parse(form["position"]);
                newList = logList.Skip(position - 1 * 10).Take(10);
                TempData["position"] = position - 1;

            }


            return View(newList.OrderByDescending(x => x.Schedule.Class_Date).OrderBy(x => x.Schedule.Start_Time));
        }

        [HttpGet]
        public ActionResult ClassLogListPast()
        {

            IEnumerable<Class_Log> logList = db.GetClass_LogsByUId((int)Session["Uid"]);

            return View(logList.OrderBy(x => x.Schedule.Start_Time).OrderByDescending(x => x.Schedule.Class_Date));
        }

        [HttpPost]
        public ActionResult ClassLogListPast(FormCollection form)
        {

            IEnumerable<Class_Log> logList = db.GetClass_LogsByUId((int)Session["Uid"]);


            IEnumerable<Class_Log> newList;
            if (form["back"] == null)
            {
                if (form["position"] == null)
                {
                    newList = logList.Skip(1 * 10).Take(10);
                    TempData["position"] = 2;
                }
                else
                {
                    int position = Int32.Parse(form["position"]);
                    newList = logList.Skip(position * 10).Take(10);
                    TempData["position"] = position + 1;
                }

            }
            else
            {

                int position = Int32.Parse(form["position"]);
                newList = logList.Skip(position - 1 * 10).Take(10);
                TempData["position"] = position - 1;

            }


            return View(newList.OrderByDescending(x => x.Schedule.Class_Date).OrderBy(x => x.Schedule.Start_Time));
        }


        // cancel upcoming class 
        [Filters.AuthorizeStudent]

        public ActionResult Cancel(int id)
        {

            //db remove or archive class log 
            db.DeleteClass_Log(id);

            //Give back token to student 
            int userId = Int32.Parse(Session["Uid"].ToString());
            db.AddTokens(userId, 1);

            // give confirmation 


            return RedirectToAction("ClassLogList");
        }


        [HttpGet]
        [Filters.AuthorizeStudent]
        public ActionResult PassLogList()
        {
            // User id 
            int userId = Int32.Parse(Session["Uid"].ToString());

            IEnumerable<Pass_Log> pass_Log_List = db.getPass_LogsByUId(userId);

            return View(pass_Log_List.OrderByDescending(x => x.Date_Purchased).Take(10)); 
        }

        [Filters.AuthorizeStudent]
        [HttpPost]
        public ActionResult PassLogList(FormCollection form)
        {
            // User id 
            int userId = Int32.Parse(Session["Uid"].ToString());

            IEnumerable<Pass_Log> pass_Log_List = db.getPass_LogsByUId(userId);
            IEnumerable<Pass_Log> orderedList = pass_Log_List.OrderByDescending(x => x.Date_Purchased);


            IEnumerable<Pass_Log> newList;
            if (form["back"] == null)
            {
                if (form["position"] == null)
                {
                    newList = orderedList.Skip(1 * 10).Take(10);
                    TempData["position"] = 2;
                }
                else
                {
                    int position = Int32.Parse(form["position"]);
                    newList = orderedList.Skip(position * 10).Take(10);
                    TempData["position"] = position + 1;
                }

            }
            else
            {

                int position = Int32.Parse(form["position"]);
                newList = orderedList.Skip(position - 1 * 10).Take(10);
                TempData["position"] = position - 1;

            }


            return View(newList);
        }



        [Filters.AuthorizeStudent]

        public ActionResult Passes()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        [Filters.AuthorizeTeacher]
        public ActionResult ViewAvailabilities()
        {
            int id = Int32.Parse(Session["Uid"].ToString());
            XDocument xd = db.getAvailability(id);
            return View(xd);
        }

        [HttpGet]
        [Filters.AuthorizeTeacher]
        public ActionResult CreateAvailabilities()
        {
            int id = Int32.Parse(Session["Uid"].ToString());
            XDocument xd = db.getAvailability(id);
            return View(xd);
        }

        [HttpPost]
        public ActionResult CreateAvailabilities(FormCollection collection)
        {
            // "N/A"
            string ss = collection["sundayStart"];
            XDocument availabilities = new XDocument
           (
           new XElement("Root",
                   new XElement("Sunday",
                   new XElement("Start", collection["sundayStart"]),
                   new XElement("End", collection["sundayEnd"])),
               new XElement("Monday",
                   new XElement("Start", collection["mondayStart"]),
                   new XElement("End", collection["mondayEnd"])),
               new XElement("Tuesday",
                   new XElement("Start", collection["tuesdayStart"]),
                   new XElement("End", collection["tuesdayEnd"])),
               new XElement("Wednesday",
                   new XElement("Start", collection["wednesdayStart"]),
                   new XElement("End", collection["wednesdayEnd"])),
               new XElement("Thursday",
                   new XElement("Start", collection["thursdayStart"]),
                   new XElement("End", collection["thursdayEnd"])),
               new XElement("Friday",
                   new XElement("Start", collection["fridayStart"]),
                   new XElement("End", collection["fridayEnd"])),
               new XElement("Saturday",
                   new XElement("Start", collection["saturdayStart"]),
                   new XElement("End", collection["saturdayEnd"]))
           ));
            DateTime s1 = Convert.ToDateTime(collection["sundayStart"]);
            DateTime e1 = Convert.ToDateTime(collection["sundayEnd"]);

            DateTime s2 = Convert.ToDateTime(collection["mondayStart"]);
            DateTime e2 = Convert.ToDateTime(collection["mondayEnd"]);

            DateTime s3 = Convert.ToDateTime(collection["tuesdayStart"]);
            DateTime e3 = Convert.ToDateTime(collection["tuesdayEnd"]);

            DateTime s4 = Convert.ToDateTime(collection["wednesdayStart"]);
            DateTime e4 = Convert.ToDateTime(collection["wednesdayEnd"]);

            DateTime s5 = Convert.ToDateTime(collection["thursdayStart"]);
            DateTime e5 = Convert.ToDateTime(collection["thursdayEnd"]);

            DateTime s6 = Convert.ToDateTime(collection["fridayStart"]);
            DateTime e6 = Convert.ToDateTime(collection["fridayEnd"]);

            DateTime s7 = Convert.ToDateTime(collection["saturdayStart"]);
            DateTime e7 = Convert.ToDateTime(collection["saturdayEnd"]);


            if (s1 > e1|| s2 > e2 || s3 > e3 || s4 > e4 || s5 > e5 || s6 > e6 || s7 > e7)
            {
                ViewBag.message = "End time set before start time!";
                
                return View(availabilities);
            }
            else
            {
                int id = Int32.Parse(Session["Uid"].ToString());
                db.AddAvailability(id, availabilities);
                return RedirectToAction("ViewAvailabilities");
            }
        }
    }
}
 