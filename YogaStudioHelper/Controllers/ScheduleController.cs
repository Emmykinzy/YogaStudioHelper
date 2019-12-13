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
            return View(list);
        }

        public ActionResult ClassSignUp()
        {
            return View();
        }
    }
}