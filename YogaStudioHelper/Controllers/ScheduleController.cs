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
            return View();
        }
    }
}