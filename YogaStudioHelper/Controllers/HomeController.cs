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
            Session["Auth"] = 1;
            Session["Uid"] = 1;
            

            //Receptionist
            //Session["Auth"] = 3;

            //Admin
            
            //Session["Auth"] = 1;


            // student  
            //Session["Auth"] = 4;
            //Session["Uid"] = 4;

            //yoga user id 4 is a student id
            //Session["Uid"] = 5;



            // for upcoming classes feature 

            IEnumerable<Schedule> list = db.getSchedulesNext7Days();


            List<Schedule> weekList = list.Where(x => x.Class_Date.Date >= DateTime.Now.Date && x.Class_Date.Date <= DateTime.Now.AddDays(6)).ToList();

            //IEnumerable<Schedule> orderedList = list.OrderByDescending(x => x.Class_Date).OrderBy(x => x.Start_Time);

            ViewBag.UpcomingClasses = weekList; 

            foreach(Schedule sch in weekList)
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

    }
}