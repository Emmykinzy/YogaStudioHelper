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

       
        public ActionResult Classes()
        {
          //  var  c = db.getScheduleById(2);
           // c.Cl
            return View();
        }


        // test class log 

        public ActionResult ClassLogList()
        {

            IEnumerable<Class_Log> class_Log_List = db.GetClass_Logs();

            /* 
             
            ? more efficienct way to do ? less search in view etc ?

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                        .ThenInclude(post => post.Author)
                    .ToList();
            }
            */


            return View(class_Log_List);
        }


        // cancel upcoming class 
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


        public ActionResult PassLogList()
        {
            IEnumerable<Pass_Log> pass_Log_List = db.getPass_Logs();


            return View(pass_Log_List); 
        }




        public ActionResult Passes()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult ViewAvailabilities()
        {
            int id = Int32.Parse(Session["Uid"].ToString());
            XDocument xd = db.getAvailability(id);
            return View(xd);
        }

        [HttpGet]
        public ActionResult CreateAvailabilities()
        {
            int id = Int32.Parse(Session["Uid"].ToString());
            XDocument xd = db.getAvailability(id);
            return View(xd);
        }

        [HttpPost]
        public ActionResult CreateAvailabilities(FormCollection collection)
        {
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
 