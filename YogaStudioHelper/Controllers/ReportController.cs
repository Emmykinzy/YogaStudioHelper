using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YogaStudioHelper.ViewModels;

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
            var teachers = db.getTeacherList();

            var model = new HoursWorkedViewModel
            {
                Teachers = teachers
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult HoursWorked(FormCollection collection)
        {

            var teacherId = Convert.ToInt32(collection["Teachers"]);


            DateTime startDate = DateTime.Parse(collection["startDate"]);

            DateTime endDate = DateTime.Parse(collection["endDate"]);

            // call db method
            IEnumerable<Schedule> schedList = db.GetHoursWorkedReport(startDate, endDate, teacherId);
            TempData["schedList"] = schedList;
            
            return RedirectToAction("HoursWorkedList");
        }


        [HttpGet]
        public ActionResult HoursWorkedList(FormCollection collection)
        {
            IEnumerable<Schedule> schedList = TempData["schedList"] as IEnumerable<Schedule>;

            List<HoursWorkedViewModel> reportList = new List<HoursWorkedViewModel>();

            foreach (Schedule sched in schedList)
            {
                HoursWorkedViewModel item = new HoursWorkedViewModel();
                var classe = db.getClass(sched.Class_Id);
                var teacher = db.getUserById(sched.Teacher_Id);

                item.Class_Date = sched.Class_Date;
                item.U_First_Name = teacher.U_First_Name;
                item.U_Last_Name = teacher.U_Last_Name;
                item.Class_Name = classe.Class_Name;
                item.Class_Length = classe.Class_Length;
                reportList.Add(item);
            }


            return View(reportList);
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