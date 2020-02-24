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
    /// <summary>
    /// Fix monthly on publish
    /// 
    /// https://stackoverflow.com/questions/6062192/there-is-already-an-open-datareader-associated-with-this-command-which-must-be-c 
    /// 
    /// ToList()
    /// </summary>
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
            try
            {

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

                    TempData["saleList"] = list;
                }

            }catch(Exception e)
            {
                TempData["Message"] = e.ToString();
                return RedirectToAction("MessageView");
            }


            

            // redirect view with list of passlog 
            return RedirectToAction("SaleList");
        }

        [HttpGet]
        public ActionResult SalesMonthly()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SalesMonthly(FormCollection collection)
        {
            string month = collection["month"];

            DateTime startDate = DateTime.Parse(collection["month"]);

            DateTime endDate = startDate.AddMonths(1);


            //get list with this time constraint 
            IEnumerable<Database.Pass_Log> saleList = db.GetSaleReport(startDate, endDate).ToList();

            IEnumerable<SaleReportMonthly> monthList = (from log in saleList                                                        
                                                        group log by log.Class_Passes.Pass_Name
                                                        into grp
                                                        select new SaleReportMonthly()
                                                        {
                                                            Pass_Name = grp.Key,
                                                            count = grp.Count(),
                                                            Total_Num_Classes = grp.Sum(x => x.Num_Classes),
                                                            Total_Purchase_Price = grp.Sum(x => x.Purchase_Price)
                                                        }
                                                        ).ToList();
            TempData["month"] = startDate.ToString("MMMMyyyy");
             string mt = startDate.ToString("MMMM yyyy");
            TempData["monthtitle"] = mt;
            TempData["saleListMonth"] = monthList.ToList();

            return RedirectToAction("SaleListMonth");
        }

        [HttpGet]
        public ActionResult SaleListMonth()
        {

            List<SaleReportMonthly> saleList = TempData["saleListMonth"] as List<SaleReportMonthly>;
            string month = TempData["month"] as string;
            ViewData["month"] = month;
            return View(saleList.OrderByDescending(x => x.Total_Purchase_Price).ToList());
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
            IEnumerable<Database.Pass_Log> saleList = db.GetSaleReport(startDate, endDate).ToList();

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


            TempData["saleList"] = list.ToList();
            

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
            TempData["startDate"] = startDate.ToString("dd/MM/yyyy");
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
            ViewBag.startDate = TempData["startDate"] as string;
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
            List<AttendanceDates> list = db.GetAttendanceDatesReport(startDate, endDate).ToList();
            TempData["AttendanceDates"] = list.ToList();

            // redirect view with list of passlog 
            return RedirectToAction("AttendanceDatesList");
        }

        [HttpGet]
        public ActionResult AttendanceDatesList()
        {
            List<AttendanceDates> list = TempData["AttendanceDates"] as List<AttendanceDates>;
            return View(list);
        }



        // <h2>HoursWorkedMonthly</h2>

        [HttpGet]
        public ActionResult HoursWorkedMonthly()
        {
            return View();
        }


        [HttpPost]
        public ActionResult HoursWorkedMonthly(FormCollection collection)
        {

            try
            {
                string month = collection["month"];

                DateTime startDate = DateTime.Parse(collection["month"]);

                DateTime endDate = startDate.AddMonths(1);

                // call method to get list of data 
                List<HoursWorkedMonthly> list= GetHoursWorkedMonthlyReport(startDate, endDate).ToList();


                TempData["HoursWorkedMonthly"] = list.ToList();

                return RedirectToAction("HoursWorkedMonthlyList");
            }catch(Exception e)
            {
                TempData["Message"] = e.ToString();
                return RedirectToAction("MessageView", "Home");
            }
        }



        [HttpGet]
        public ActionResult HoursWorkedMonthlyList()
        {
            List<HoursWorkedMonthly> list = TempData["HoursWorkedMonthly"] as List<HoursWorkedMonthly>;
            return View(list);
        }




        /// <summary>
        /// 
        /// Method for Report List 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public List<HoursWorkedMonthly> GetHoursWorkedMonthlyReport(DateTime d1, DateTime d2)
        {
            List<HoursWorkedMonthly> list = new List<HoursWorkedMonthly>();

            var teacherList = db.getTeacherList().ToList();

            foreach(var teacher in teacherList)
            {
                HoursWorkedMonthly record = new HoursWorkedMonthly();

                record.U_First_Name = teacher.U_First_Name;
                record.U_Last_Name = teacher.U_Last_Name;

                // set myDb public to access it here, might not be secure to check back 

                var schedList = db.myDb.Schedules.Where(x => x.Class_Date >= d1 && x.Class_Date <= d2 && x.Teacher_Id == teacher.U_Id).ToList(); 

                foreach(var sched in schedList)
                {
                    // needed for length 
                    var classe = db.getClass(sched.Class_Id);

                    record.totalHours += classe.Class_Length;
                    record.totalClasses++; 


                }

                list.Add(record);

            }

            return list.ToList();
        }
    }
}