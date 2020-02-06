using Database;
using Database.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YogaStudioHelper.Models;
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
        public ActionResult Attendance()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Attendance(FormCollection collection)
        {

            return View();
        }

        [HttpGet]
        public ActionResult Sales()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SalesMonth()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SalesMonth(FormCollection collection)
        {
            // get the date in form collection 

            DateTime date = DateTime.Parse(collection["datepicker"]);

            DateTime date2 = date.AddMonths(1);

            //get list with this time constraint 
            IEnumerable<Database.Pass_Log> saleList = db.GetSaleReport(date, date2);


            List<SaleReportViewModel> list = new List<SaleReportViewModel>();

            foreach (var sale in saleList)
            {
                SaleReportViewModel saleReport = new SaleReportViewModel();

                var classPass = db.getClassPasse(sale.Pass_Id);
                var user = db.getUserById(sale.U_Id);

                saleReport.Pass_Log_Id = sale.Pass_Log_Id;
                saleReport.Pass_Id = sale.Pass_Id;
                saleReport.Pass_Name = classPass.Pass_Name;
                saleReport.U_Id = sale.U_Id;
                saleReport.U_First_Name = user.U_First_Name;
                saleReport.U_Last_Name = user.U_Last_Name;
                saleReport.Num_Classes = sale.Num_Classes.GetValueOrDefault();
                saleReport.Purchase_Price = Convert.ToDouble(sale.Purchase_Price);
                saleReport.Date_Purchased = sale.Date_Purchased;

                list.Add(saleReport);
            }




            TempData["saleList"] = list;

            // redirect view with list of passlog 
            return RedirectToAction("SaleList");
        }


        [HttpGet]
        public ActionResult SalesDates()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SalesDates(FormCollection collection)
        {
            // get the date in form collection 

            DateTime startDate = DateTime.Parse(collection["startDate"]);

            DateTime endDate = DateTime.Parse(collection["endDate"]);


            //get list with this time constraint 
            IEnumerable<Database.Pass_Log> saleList = db.GetSaleReport(startDate, endDate);

            List<SaleReportViewModel> list = new List<SaleReportViewModel>(); 

            foreach(var sale in saleList)
            {
                SaleReportViewModel saleReport = new SaleReportViewModel();

                var classPass = db.getClassPasse(sale.Pass_Id);
                var user = db.getUserById(sale.U_Id);

                saleReport.Pass_Log_Id = sale.Pass_Log_Id;
                saleReport.Pass_Id = sale.Pass_Id;
                saleReport.Pass_Name = classPass.Pass_Name;
                saleReport.U_Id = sale.U_Id;
                saleReport.U_First_Name = user.U_First_Name;
                saleReport.U_Last_Name = user.U_Last_Name;
                saleReport.Num_Classes = sale.Num_Classes.GetValueOrDefault();
                saleReport.Purchase_Price = Convert.ToDouble(sale.Purchase_Price);
                saleReport.Date_Purchased = sale.Date_Purchased;


                list.Add(saleReport);
            }


            TempData["saleList"] = list;

            // redirect view with list of passlog 
            return RedirectToAction("SaleList");
        }



        [HttpGet]
        public ActionResult SaleList()
        {
            
            List<SaleReportViewModel> saleList = TempData["saleList"] as List<SaleReportViewModel>;

            //IEnumerable<Database.Pass_Log> saleList = TempData["saleList"] as IEnumerable<Database.Pass_Log>; 
            return View(saleList);
        }


        //

        [HttpGet]
        public ActionResult AttendanceDaily()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AttendanceDaily(FormCollection collection)
        {
            // get the date in form collection 
            DateTime startDate = DateTime.Parse(collection["startDate"]);

            //IEnumerable<Schedule> scheduleList = db.GetAttendanceDailyReport(startDate);
            List<AttendanceDaily> list = db.GetAttendanceDailyReport(startDate);

            TempData["AttendanceDaily"] = list;
            //get list with this time constraint 
            //IEnumerable<Pass_Log> saleList = db.GetSaleReport(startDate, endDate);
            //TempData["saleList"] = saleList;

            // redirect view with list of passlog 
            return RedirectToAction("AttendanceDailyList");
        }

        [HttpGet]
        public ActionResult AttendanceDailyList()
        {
            List<AttendanceDaily> list = TempData["AttendanceDaily"] as List<AttendanceDaily>;
            return View(list);
        }


        [HttpGet]
        public ActionResult AttendanceDates()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AttendanceDates(FormCollection collection)
        {
            // get the date in form collection 

            DateTime startDate = DateTime.Parse(collection["startDate"]);

            DateTime endDate = DateTime.Parse(collection["endDate"]);


            //get list with this time constraint 
            List<AttendanceDates> list = db.GetAttendanceDatesReport(startDate, endDate);
            TempData["AttendanceDates"] = list;

            // redirect view with list of passlog 
            return RedirectToAction("AttendanceDatesList");
        }

        [HttpGet]
        public ActionResult AttendanceDatesList()
        {
            List<AttendanceDates> list = TempData["AttendanceDates"] as List<AttendanceDates>;
            return View(list);
        }

    }
}