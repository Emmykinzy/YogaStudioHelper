﻿using System;
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
            //Session["Auth"] = 1;
            //Session["Uid"] = 1;

            //Teacher
            //Session["Auth"] = 2;
            //Session["Uid"] = 2;

            //Receptionist
            //Session["Auth"] = 3;

            //Admin

           //Session["Auth"] = 1;


            // student  
            //Session["Auth"] = 4;
            //Session["Uid"] = 1;

            //yoga user id 4 is a student id
            //Session["Uid"] = 5;



            // for upcoming classes feature 

            IEnumerable<Schedule> list = db.getSchedulesNext7Days();


            List<Schedule> weekList = list.Where(x => x.Class_Date.Date >= DateTime.Now.Date && x.Class_Date.Date <= DateTime.Now.AddDays(6)).ToList();

            //IEnumerable<Schedule> orderedList = list.OrderByDescending(x => x.Class_Date).OrderBy(x => x.Start_Time);

            ViewBag.UpcomingClasses = weekList.ToList(); 

            foreach(Schedule sch in weekList.ToList())
            {
                if(sch.Class_Date == DateTime.Now.Date)
                {
                    if(sch.Start_Time.Hours < DateTime.Now.Hour)
                    {

                        if (sch.Start_Time.Minutes < DateTime.Now.Minute)
                        {
                            weekList.Remove(sch);
                        }
                    }
                    else
                    {

                    }

                }
            }

            return View(weekList.Take(10)); 
        }

        public ActionResult MessageView()
        {
            return View();
        }
        
        public ActionResult Contact()
        {


            return View();
        }

        [HttpPost]
        public ActionResult Contact(FormCollection collection)
        {

            string name= collection["txt_name"];
            string email= collection["txt_email"];
            string phone= collection["txt_phone"];
            string subject= collection["txt_subject"];
            string message= collection["txt_message"];



            string mail_message = "From: " + name + "<br/>";
            mail_message += "Email: " + email + "<br/>";
            mail_message += "Subject: " + subject + "<br/>";
            mail_message += "Phone: " + phone + "<br/>";
            mail_message += "Message: " + message + "<br/>";

            // call util methods 
            Util.EmailSender.sendContactForm(email, subject, mail_message);

            Util.EmailSender.sendConfirmation(email, subject, name);

            return View();
        }
    }
}