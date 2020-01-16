using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Database;
using YogaStudioHelper.ViewModels;

namespace YogaStudioHelper.Controllers
{
    public class ManageStudioController : Controller
    {
        DBMaster db = new DBMaster();
        // GET: ManageStudio
        public ActionResult ManageStudio()
        {
            return View();
        }
        public ActionResult Room()
        {
            return View();
        }
        public ActionResult RoomList()
        {
            IEnumerable<Room> roomList = db.getRooms();
            return View(roomList);
        }
        [HttpGet]
        public ActionResult CreateRoom()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRoom(FormCollection collection)
        {
            string roomname = collection["roomName"];
            int capacity = Int32.Parse(collection["capacity"]);

            Room r = new Room();

            r.Room_Name = roomname;
            r.Room_Capacity = capacity;

            db.CreateRoom(r);
            return RedirectToAction("RoomList");
        }

        //Ben todo 


        [HttpGet]
        public ActionResult EditRoom(int id)
        {
            var roomEdit = db.getRoom(id);


            ViewBag.EditRoom = roomEdit;

            return View();
        }
       // todo 
        [HttpPost]
        public ActionResult EditRoom(FormCollection collection)
        {

            int id = (int)TempData["EditRoomId"];


            var roomEdit = db.getRoom(id);

            string roomname = collection["roomName"];
            int capacity = Int32.Parse(collection["capacity"]);


            roomEdit.Room_Name = roomname;
            roomEdit.Room_Capacity = capacity;

            db.UpdateRoom(roomEdit);
            return RedirectToAction("RoomList");

        }

        public ActionResult DeleteRoom(int id)
        {
            db.DeleteRoom(id);


            return RedirectToAction("RoomList");
        }

        //todo 
        public ActionResult ArchiveRoom(int id)
        {
            db.DeleteRoom(id);


            return RedirectToAction("RoomList");
        }


        public ActionResult Class()
        {
            return View();
        }
        [HttpGet]
        public ActionResult FindClass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FindClass(FormCollection collection)
        {
            string name = collection["ClassName"];
            IEnumerable<Class> classList = db.getClassesByName(name);
            if(classList.Count() == 0)
            {
                ViewBag.FindClassMessage = "No class with the name " + name + " was found";
                return View();
            }
            else
            {
                TempData["classList"] = classList;
                return RedirectToAction("FindClassList");
            }
            
        }
        
        public ActionResult FindClassList()
        {
            if (TempData["List"] == null)
            {
                IEnumerable<Class> c = TempData["classList"] as IEnumerable<Class>;
                return View(c);
            }
            else
            {
                IEnumerable<Class> c = TempData["List"] as IEnumerable<Class>;
                return View(c);
            }
            
        }
        public ActionResult ClassList()
        {
           IEnumerable<Class> classList = db.getClasses();
            return View(classList);
        }

        [HttpGet]
        public ActionResult CreateClass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateClass(FormCollection collection)
        {
            string name =collection["ClassName"];
            string desc = collection["ClassDesc"];
            int minutes = Int32.Parse(collection["ClassLength"]);

            TimeSpan length = new TimeSpan();

            if (minutes < 60)
            {
                TimeSpan min = TimeSpan.FromMinutes(minutes);
                length += min;
            }
            else
            {
                int remainder = minutes % 60;
                if(remainder == 0)
                {
                    int hour = minutes / 60;
                    TimeSpan hr = TimeSpan.FromHours(hour);
                    length += hr;
                }
                else
                {
                    TimeSpan min = TimeSpan.FromMinutes(remainder);
                    TimeSpan hr = TimeSpan.FromHours((minutes-remainder)/60);

                    length = hr + min;
                   
                }
            }

            Class c = new Class();

            c.Active = true;
            c.Class_Name = name;
            c.Class_Desc = desc;
            c.Class_Length = length;

            db.CreateClass(c);

            return RedirectToAction("ClassList");
        }

        // TODO

        [HttpGet]
        public ActionResult EditClass(int id)
        {
            var classEdit = db.getClass(id);


            ViewBag.EditClass = classEdit; 


            return View();
        }

        [HttpPost]
        public ActionResult EditClass(FormCollection collection)
        {
            // TempData["EditClassId"] 
            //int id = 1; 
            //int id = Int32.Parse(TempData["EditClassId"].ToString());
            int id = (int)TempData["EditClassId"];


            var classEdit = db.getClass(id);

            string name = collection["ClassName"];
            string desc = collection["ClassDesc"];
            int minutes = Int32.Parse(collection["ClassLength"]);

            TimeSpan length = new TimeSpan();

            if (minutes < 60)
            {
                TimeSpan min = TimeSpan.FromMinutes(minutes);
                length += min;
            }
            else
            {
                int remainder = minutes % 60;
                if (remainder == 0)
                {
                    int hour = minutes / 60;
                    TimeSpan hr = TimeSpan.FromHours(hour);
                    length += hr;
                }
                else
                {
                    TimeSpan min = TimeSpan.FromMinutes(remainder);
                    TimeSpan hr = TimeSpan.FromHours((minutes - remainder) / 60);

                    length = hr + min;

                }
            }

            Class c = new Class();

            


            classEdit.Class_Name = name;
            classEdit.Class_Desc = desc;
            classEdit.Class_Length = length;

            //TODO check checkbox 

            //c.Active = true;


            db.EditClass(classEdit);
            //db.CreateClass(c);

            return RedirectToAction("ClassList");


          
        }


        public ActionResult ArchiveClass(int id)
        {
            
            //SHould implement archive instead
            db.DeleteClass(id);

            return RedirectToAction("ClassList");
        }

        public ActionResult ClassPass()
        {
            return View();
        }



        /// <summary>
        ///         ClassPass
        /// </summary>
        /// <returns></returns>

        public ActionResult FindClassPass()
        {
            return View();
        }

        // not use
        public ActionResult ClassPassList()
        {
            return View();
        }
        // use
        public ActionResult ClassPassList2()
        {

            IEnumerable<Class_Passes> class_Pass_List = db.getClassPasses();
            return View(class_Pass_List);
           
        }
        [HttpGet]
        public ActionResult CreateClassPass()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreateClassPass(FormCollection collection)
        {

            string passname = collection["ClassPassName"];
            int passqty = Int32.Parse(collection["ClassPassQty"]);
            
            double passprice= Convert.ToDouble(collection["ClassPassPrice"]);

            Class_Passes pass = new Class_Passes();

            pass.Active = true;
            pass.Pass_Name = passname;
            pass.Pass_Size = passqty;
            pass.Pass_Price = Convert.ToDecimal(passprice);

            db.CreateClassPass(pass);

            return RedirectToAction("ClassPassList2");
           
        }

        [HttpGet]
        public ActionResult EditClassPass(int id)
        {
            var classPassEdit = db.getClassPasse(id);


            ViewBag.EditClassPass = classPassEdit;


            return View();
        }

        [HttpPost]
        public ActionResult EditClassPass(FormCollection collection)
        {
            int id = (int)TempData["EditClassPassId"];

            var passEdit = db.getClassPasse(id);



            string passname = collection["ClassPassName"];

            int passqty = Int32.Parse(collection["ClassPassQty"]);

            double passprice = Double.Parse(collection["ClassPassPrice"]);

            passEdit.Pass_Name = passname;
            passEdit.Pass_Size = passqty;
            // decimal? 
            passEdit.Pass_Price = Convert.ToDecimal(passprice); 


            db.UpdateClassPass(passEdit);
            return RedirectToAction("ClassPassList2");

       
        }

        public ActionResult ArchiveClassPass(int id)
        {
            //SHould implement archive instead
            db.DeleteClassPass(id);

            return RedirectToAction("ClassPassList2");
        }

        /// <summary>
        ///         Promotion 
        /// </summary>
        /// <returns></returns>


        public ActionResult Promotion()
        {

            return View();

        }

        public ActionResult FindPromotion()
        {
            return View();
        }

        public ActionResult PromotionList()
        {
            IEnumerable<Promotion> promoList = db.getPromotions();
            return View(promoList);
        
        }

        // todo dropdown
        public ActionResult CreatePromotion()
        {

            var passes = db.getClassPasses();
            var viewTest = new DropdownListPassViewModel
            {
                Passes = passes
            };
        
            return View(viewTest);
        }

        [HttpPost]
        public ActionResult CreatePromotion(FormCollection collection)
        {
            string promoDesc = collection["PromotionDescription"];

            double discount;
            int extraPasses;
            try
            {
                 discount = Convert.ToDouble(collection["discount"])/100;
            }
            catch
            {
                 discount = 0;
            }

            try
            {
                 extraPasses = Int32.Parse(collection["passes"]);
            }
            catch
            {
                 extraPasses = 0; 
            }

             
            DateTime promoEnd = Convert.ToDateTime(collection["promoEnd"]); 

            Promotion promo = new Promotion();


            promo.Promo_Desc = promoDesc;
            promo.Discount = Convert.ToDecimal(discount);
            promo.Num_Classes = extraPasses;
            promo.Promo_End = promoEnd;
            
            //Fix todo add class pass dropdown option
            promo.Pass_Id = 1;
            String KeyTest = collection["passList"]; 


            db.CreatePromotion(promo);
            return RedirectToAction("PromotionList");
        }

        [HttpGet]
        public ActionResult EditPromotion(int id)
        {
            var promotionEdit = db.getPromotion(id);


            ViewBag.EditPromotion = promotionEdit;

            //dropdown 
            var passes = db.getClassPasses();
            var viewTest = new DropdownListPassViewModel
            {
                Passes = passes
            };

            return View(viewTest);

        }

        [HttpPost]
        public ActionResult EditPromotion(FormCollection collection)
        {

            int id = (int)TempData["EditPromotionId"];

            var promo = db.getPromotion(id);


            string promoDesc = collection["PromotionName"];

            double discount;
            int extraPasses;
            try
            {                discount = Convert.ToDouble(collection["discount"]) / 100; }
            catch
            {                discount = 0;            }

            try
            {   extraPasses = Int32.Parse(collection["passes"]);   }
            catch
            {    extraPasses = 0; }

            DateTime promoEnd = Convert.ToDateTime(collection["promoEnd"]);

            //Promotion promo = new Promotion();

            promo.Promo_Desc = promoDesc;
            promo.Discount = Convert.ToDecimal(discount);
            promo.Num_Classes = extraPasses;
            promo.Promo_End = promoEnd;

            //Fix todo add class pass dropdown option
            // TODO dropdown to select class pass
            //promo.Pass_Id = 1;

            db.UpdatePromotion(promo);



            return RedirectToAction("PromotionList");

        }

        public ActionResult ArchivePromotion(int id)
        {
            db.DeletePromotion(id);


            return RedirectToAction("PromotionList");

        }


        public ActionResult ScheduleList()
        {
            IEnumerable<Schedule> scheduleList = db.getSchedules();
            return View(scheduleList);


        }

        [HttpGet]
        public ActionResult CreateSchedule()
        {

            // DropDown 
            //ScheduleViewModel

            //https://localhost:44332/ManageStudio/CreateSchedule 
            var classes = db.getClassList();
            var teachers = db.getTeacherList();
            var rooms = db.getRoomList();

            /*

            List<Class> classListTest = new List<Class>();

            classListTest.Add(new Class { Class_Id = 10, Class_Name = "Math" });
            classListTest.Add(new Class { Class_Id = 11, Class_Name = "Francais" });
            classListTest.Add(new Class { Class_Id = 12, Class_Name = "English" });
            classListTest.Add(new Class { Class_Id = 13, Class_Name = "ASP.NET" });
           
            List<Yoga_User> teacherListTest = new List<Yoga_User>();

            teacherListTest.Add(new Yoga_User { U_Id = 10, U_Last_Name = "Wood" });
            teacherListTest.Add(new Yoga_User { U_Id = 11, U_Last_Name = "CL" });
            teacherListTest.Add(new Yoga_User { U_Id = 12, U_Last_Name = "Johnson" });

            List<Room> roomListTest = new List<Room>();

            roomListTest.Add(new Room { Room_Id = 10, Room_Name = "A-204" });
            roomListTest.Add(new Room { Room_Id = 11, Room_Name = "F-245" });
            roomListTest.Add(new Room { Room_Id = 12, Room_Name = "B-06" });

    */


            var scheduleViewModel = new ScheduleViewModel
            {
                Classes = classes,
                Teachers = teachers,
                Rooms = rooms

            };


            return View(scheduleViewModel); 
        }

        [HttpPost]
        public ActionResult CreateSchedule(FormCollection collection)
        {
            // how to get dropdown value 

            Schedule schedule = new Schedule(); 


            var selectedTeacher = Convert.ToInt32(collection["Teachers"]);
            var selectedCLass = Convert.ToInt32(collection["Classes"]);
            var selectedRoom = Convert.ToInt32(collection["Rooms"]);

            DateTime classDate = Convert.ToDateTime(collection["classDate"]);

            TimeSpan timePicker = TimeSpan.Parse(collection["picker"]);

           // Add course length to timeSpan? no sure working 
            var classTime = db.getClass(selectedCLass).Class_Length;

            timePicker.Add(classTime);


            var status = collection["status"]; 


            schedule.Teacher_Id = selectedTeacher;
            schedule.Class_Id = selectedCLass;
            schedule.Room_Id = selectedRoom;
            schedule.Class_Date = classDate;
            schedule.Schedule_Status = status;
            schedule.Start_Time = timePicker;




            db.CreateSchedule(schedule);

            return RedirectToAction("ScheduleList");
        }

        [HttpGet]
        public ActionResult EditSchedule(int id)
        {
            var schedule = db.getScheduleById(id);
            ViewBag.EditSchedule = schedule;

            // set dropdown model
            var classes = db.getClassList();
            var teachers = db.getTeacherList();
            var rooms = db.getRoomList();

            var scheduleViewModel = new ScheduleViewModel
            {
                Classes = classes,
                SelectedClassId = schedule.Class_Id,
                ClasseEdit = new SelectList(classes, "Class_Id", "Class_Name", schedule.Class_Id),
                Teachers = teachers,
                SelectedTeacherId = schedule.Teacher_Id,
                Rooms = rooms,
                SelectedRoomId = schedule.Room_Id
            };




            return View(scheduleViewModel); 

        }

        [HttpPost]
        public ActionResult EditSchedule(FormCollection collection)
        {
            int id = (int)TempData["EditScheduleId"];


            var schedule = db.getScheduleById(id);

            // getg 

            var selectedTeacher = Convert.ToInt32(collection["SelectedTeacherId"]);
            //var selectedCLass = collection["SelectedClassId"];
            var selectedCLass = Convert.ToInt32(collection["SelectedClassId"]);
            var selectedRoom = Convert.ToInt32(collection["SelectedRoomId"]);

            DateTime classDate = Convert.ToDateTime(collection["classDate"]);

            var status = collection["status"];


            schedule.Teacher_Id = selectedTeacher;
            schedule.Class_Id = selectedCLass;
            schedule.Room_Id = selectedRoom;
            schedule.Class_Date = classDate;
            schedule.Schedule_Status = status;


            // put db update method 


            db.UpdateSchedule(schedule); 


            return RedirectToAction("ScheduleList");
        }

        public ActionResult ArchiveSchedule(int id)
        {
            db.DeleteSchedule(id);


            return RedirectToAction("ScheduleList");

        }
    }
}