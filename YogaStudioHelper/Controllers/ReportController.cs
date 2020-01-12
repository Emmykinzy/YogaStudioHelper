using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YogaStudioHelper.Controllers
{
    public class ReportController : Controller
    {
        DBMaster db = new DBMaster();

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
            // get the date in form collection 

            DateTime date = DateTime.Parse(collection["datepicker"]);

            DateTime date2 = date.AddMonths(1);

            //get list with this time constraint 
            IEnumerable<Pass_Log> saleList = db.GetSaleReport(date, date2);
            TempData["saleList"] = saleList;

            // redirect view with list of passlog 
            return RedirectToAction("SaleList");
        }

        [HttpGet]
        public ActionResult SaleList()
        {
            IEnumerable<Pass_Log> saleList = TempData["saleList"] as IEnumerable<Pass_Log>; 
            return View(saleList);
        }

    }
}