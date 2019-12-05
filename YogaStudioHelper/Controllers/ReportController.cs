using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YogaStudioHelper.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult HoursWorked()
        {
            return View();
        }

        [HttpPost]
        public ActionResult HoursWorked(FormCollection collection)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Attendence()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Attendence(FormCollection collection)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Sales()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Sales(FormCollection collection)
        {
            return View();
        }
    }
}