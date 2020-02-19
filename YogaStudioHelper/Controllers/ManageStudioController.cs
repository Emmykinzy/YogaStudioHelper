using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Database;
using YogaStudioHelper.Util;
using YogaStudioHelper.ViewModels;
using Database.ModelsDB;



namespace YogaStudioHelper.Controllers
{

    [Filters.AuthorizeAdmin]
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

        [HttpGet]
        public ActionResult RoomList()
        {
            IEnumerable<Room> roomList = db.getRooms();
            IEnumerable<Room> orderedList = (from room in roomList
                                                  orderby room.Room_Name
                                                  select room);
            return View(roomList);
        }

        [HttpPost]
        public ActionResult RoomList(FormCollection form)
        {
            IEnumerable<Room> roomList = db.getRooms();
            IEnumerable<Room> orderedList = (from room in roomList
                                             orderby room.Room_Name
                                             select room);
            IEnumerable<Room> newList;
            if (form["back"] == null)
            {
                if (form["position"] == null)
                {
                    newList = orderedList.Skip(1 * 10).Take(10);
                    TempData["position"] = 2;
                }
                else
                {
                    int position = Int32.Parse(form["position"]);
                    newList = orderedList.Skip(position * 10).Take(10);
                    TempData["position"] = position + 1;
                }

            }
            else
            {

                int position = Int32.Parse(form["position"]);
                newList = orderedList.Skip(position - 1 * 10).Take(10);
                TempData["position"] = position - 1;

            }


            return View(newList);
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

            return View(roomEdit);
        }


        [HttpPost]
        public ActionResult EditRoom(Room roomEdit)
        {

            db.UpdateRoom(roomEdit);
            return RedirectToAction("RoomList");
        }
        // todo 

        /*
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
        */

        public ActionResult DeleteRoom(int id)
        {
            db.DeleteRoom(id);


            return RedirectToAction("RoomList");
        }

        //todo 
        public ActionResult ArchiveRoom(int id)
        {
            db.ArchiveRoom(id);


            return RedirectToAction("RoomList");
        }

        public ActionResult ReactivateRoom(int id)
        {
            db.ReactivateRoom(id);


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

        [HttpGet]
        public ActionResult ClassList()
        {
            IEnumerable<Class> classList = db.getClasses();
            IEnumerable<Class> orderedList = (from classes in classList
                                              orderby classes.Class_Name
                                              orderby classes.Class_Length
                                              select classes);    

            return View(orderedList.Take(10));
        }

        [HttpPost]
        public ActionResult ClassList(FormCollection form)
        {
            IEnumerable<Class> classList = db.getClasses();
            IEnumerable<Class> orderedList = (from classes in classList
                                              orderby classes.Class_Name
                                              orderby classes.Class_Length
                                              select classes);

            IEnumerable<Class> newList;
            if (form["back"] == null)
            {
                if (form["position"] == null)
                {
                    newList = orderedList.Skip(1 * 10).Take(10);
                    TempData["position"] = 2;
                }
                else
                {
                    int position = Int32.Parse(form["position"]);
                    newList = orderedList.Skip(position * 10).Take(10);
                    TempData["position"] = position + 1;
                }

            }
            else
            {

                int position = Int32.Parse(form["position"]);
                newList = orderedList.Skip(position - 1 * 10).Take(10);
                TempData["position"] = position - 1;

            }


            return View(newList);
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


            return View(classEdit);
        }

        [HttpPost]
        public ActionResult EditClass(Class classEdit)
        {

      
            
            db.EditClass(classEdit);

            return RedirectToAction("ClassList");
        }

        /*
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
        */


        public ActionResult ArchiveClass(int id)
        {

            //SHould implement archive instead
            //db.DeleteClass(id);
            /*try
            {
                db.DeleteClass(id);
            }catch(Exception e)
            {
                db.ArchiveClass(id);
            }*/

            db.ArchiveClass(id);

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


            return View(classPassEdit);
        }

        [HttpPost]
        public ActionResult EditClassPass(Class_Passes classPassEdit)
        {

            db.UpdateClassPass(classPassEdit);

            return RedirectToAction("ClassPassList2");

        }

        public ActionResult ArchiveClassPass(int id)
        {
            //SHould implement archive instead
            //db.DeleteClassPass(id);
            db.ArchiveClassPass(id);
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

        [HttpGet]
        public ActionResult PromotionList()
        {
            IEnumerable<Promotion> promoList = db.getPromotions();

            return View(promoList.OrderByDescending(x => x.Promo_End));
        
        }

        [HttpPost]
        public ActionResult PromotionList(FormCollection form)
        {
            IEnumerable<Promotion> promoList = db.getPromotions();
            IEnumerable<Promotion> orderedList = (from promo in promoList
                                                  orderby promo.Promo_End
                                                  orderby promo.Promo_Desc
                                                  select promo);
            IEnumerable<Promotion> newList;
            if (form["back"] == null)
            {
                if (form["position"] == null)
                {
                    newList = orderedList.Skip(1 * 10).Take(10);
                    TempData["position"] = 2;
                }
                else
                {
                    int position = Int32.Parse(form["position"]);
                    newList = orderedList.Skip(position * 10).Take(10);
                    TempData["position"] = position + 1;
                }

            }
            else
            {

                int position = Int32.Parse(form["position"]);
                newList = orderedList.Skip(position - 1 * 10).Take(10);
                TempData["position"] = position - 1;

            }


            return View(newList);

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
            int promotedPass = Convert.ToInt32(collection["Passes"]);

            string promoDesc;
            double discount;
            int extraPasses;
            DateTime promoEnd;
            Promotion promo;
            // Check first if pass already has one 
            if (db.CheckIfPromoExist(promotedPass))
            {
                
                if (db.CheckIfActivePromoExist(promotedPass))
                {
                    TempData["Message"] = "This class pass already has a promotion assigned.";
                    return RedirectToAction("CreatePromotion");
                }
                else
                {

                    
                    promoDesc = collection["PromotionDescription"];

                    
                    try
                    {
                        discount = Convert.ToDouble(collection["discount"]) / 100;
                    }
                    catch
                    {
                        discount = 0;
                    }

                    try
                    {
                        extraPasses = Int32.Parse(collection["extra_passes"]);
                    }
                    catch
                    {
                        extraPasses = 0;
                    }


                    promoEnd = Convert.ToDateTime(collection["promoEnd"]);

                    promo = new Promotion();


                    promo.Promo_Desc = promoDesc;
                    promo.Discount = Convert.ToDecimal(discount);
                    promo.Num_Classes = extraPasses;
                    promo.Promo_End = promoEnd;
                    promo.Pass_Id = promotedPass;


                    // validation (one or the other type of promo but not both 
                    if (discount > 0 && extraPasses > 0)
                    {
                        TempData["Message"] = "The promotion can have either a discount or extra classes but not both.";

                        //ViewBag.StickyEmail = email;
                        return RedirectToAction("CreatePromotion");
                    }
                    // validation (one or the other type of promo but not both 
                    if (discount == 0 && extraPasses == 0)
                    {
                        TempData["Message"] = "The promotion needs to have either a discount or extra classes but not both.";

                        return RedirectToAction("CreatePromotion");
                    }


                    db.CreatePromotion(promo);
                    return RedirectToAction("PromotionList");
                }
            }

            

             promoDesc = collection["PromotionDescription"];

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
                 extraPasses = Int32.Parse(collection["extra_passes"]);
            }
            catch
            {
                 extraPasses = 0; 
            }

             
             promoEnd = Convert.ToDateTime(collection["promoEnd"]); 

             promo = new Promotion();


            promo.Promo_Desc = promoDesc;
            promo.Discount = Convert.ToDecimal(discount);
            promo.Num_Classes = extraPasses;
            promo.Promo_End = promoEnd;
            promo.Pass_Id = promotedPass;


            // validation (one or the other type of promo but not both 
            if(discount>0 && extraPasses > 0)
            {
                TempData["Message"] = "The promotion can have either a discount or extra classes but not both.";

                //ViewBag.StickyEmail = email;
                return RedirectToAction("CreatePromotion");
            }
            // validation (one or the other type of promo but not both 
            if (discount == 0 && extraPasses == 0)
            {
                TempData["Message"] = "The promotion needs to have either a discount or extra classes but not both.";

                return RedirectToAction("CreatePromotion");
            }


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
            {   extraPasses = Int32.Parse(collection["extra_passes"]);   }
            catch
            {    extraPasses = 0; }

            DateTime promoEnd = Convert.ToDateTime(collection["promoEnd"]);

            //var passs = collection["Passes"]; 
            int promotedPass = Convert.ToInt32(collection["Passes"]);

            //Promotion promo = new Promotion();

            promo.Promo_Desc = promoDesc;
            promo.Discount = Convert.ToDecimal(discount);
            promo.Num_Classes = extraPasses;
            promo.Promo_End = promoEnd;
            promo.Pass_Id = promotedPass;
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

        [HttpGet]
        public ActionResult ScheduleList()
        {
            List<ScheduleListViewModel> scheduleList = db.getScheduleViewModelList();
            IEnumerable<ScheduleListViewModel> orderedList = (from classes in scheduleList
                                              orderby classes.Start_Time
                                              orderby classes.Class_Date
                                              select classes);

            return View(scheduleList.OrderByDescending(x => x.Class_Date).Take(10));
        }

        [HttpPost]
        public ActionResult ScheduleList(FormCollection form)
        {
            List<ScheduleListViewModel> scheduleList = db.getScheduleViewModelList();

            IEnumerable<ScheduleListViewModel> orderedList = (from classes in scheduleList
                                                              orderby classes.Start_Time
                                                              orderby classes.Class_Date
                                                              select classes);

            IEnumerable<ScheduleListViewModel> newList;
            if (form["back"] == null)
            {
                if (form["position"] == null)
                {
                    newList = orderedList.Skip(1 * 10).Take(10);
                    TempData["position"] = 2;
                }
                else
                {
                    int position = Int32.Parse(form["position"]);
                    newList = orderedList.Skip(position * 10).Take(10);
                    TempData["position"] = position + 1;
                }

            }
            else
            {

                int position = Int32.Parse(form["position"]);
                newList = orderedList.Skip(position - 1 * 10).Take(10);
                TempData["position"] = position - 1;

            }


            return View(newList.OrderByDescending(x=>x.Class_Date));
        }

        [HttpGet]
        public ActionResult CreateSchedule()
        {

            // DropDown 
            //ScheduleViewModel

            //https://localhost:44332/ManageStudio/CreateSchedule 
            var classes = db.getClassActiveList();
            var teachers = db.getTeacherList();
            var rooms = db.getRoomActiveList();


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
            var classes = db.getClassActiveList();
            var teachers = db.getTeacherList();
            var rooms = db.getRoomList();

            var scheduleViewModel = new ScheduleViewModel
            {
                Classes = classes,
                Teachers = teachers,
                Rooms = rooms

            };

            Schedule schedule = new Schedule(); 


            var selectedTeacher = Convert.ToInt32(collection["SelectedTeacherId"]);
            var selectedCLass = Convert.ToInt32(collection["SelectedClassId"]);
            var selectedRoom = Convert.ToInt32(collection["SelectedRoomId"]);

            ViewBag.tid = selectedTeacher;
            ViewBag.cid = selectedCLass;
            ViewBag.rid = selectedRoom;

            DateTime classDate = Convert.ToDateTime(collection["classDate"]);

            ViewBag.date = classDate.ToString("yyyy-MM-dd");
            TimeSpan timePicker = TimeSpan.Parse(collection["picker"]);

            ViewBag.time = timePicker.Hours + ":" + timePicker.Minutes;
            XDocument xd = db.getAvailability(selectedTeacher);
            string dayOftheWeek = classDate.DayOfWeek.ToString();
            TimeSpan sTime;
            TimeSpan eTime;

            Yoga_User u = db.getUserById(selectedTeacher);

             if(DateTime.Now.Date > classDate)
            {
                ViewBag.message = "<p><span style=\"color:red\">Date Error: Can't Select Dates in the Past</span>";
                return View(scheduleViewModel);
            }


            try
            {
                 sTime = TimeSpan.Parse(xd.Root.Element(dayOftheWeek).Element("Start").Value);
                 eTime = TimeSpan.Parse(xd.Root.Element(dayOftheWeek).Element("End").Value);
            }
            catch
            {
                ViewBag.message = "<p><span style=\"color:red\">Availability Error: </span>" + u.U_First_Name + " " + u.U_Last_Name + " is unavailable " + dayOftheWeek.ToLower()+".";
                return View(scheduleViewModel);
            }
            Class c = db.getClass(selectedCLass);
            TimeSpan classEnd = sTime.Add(c.Class_Length);


            if (sTime > timePicker)
            {
                
                ViewBag.message = "<p><span style=\"color:red\">Availability Error: </span>" + u.U_First_Name+" "+u.U_Last_Name+" starts "+dayOftheWeek+" at "+sTime.Hours+":"+sTime.Minutes.ToString("00");
                return View(scheduleViewModel);
                
            }

            if(classEnd > eTime)
            {
                ViewBag.message = "<p><span style=\"color:red\">Availability Error: </span>" + u.U_First_Name + " " + u.U_Last_Name + " ends " + dayOftheWeek + " at " + eTime.Hours + ":" + eTime.Minutes.ToString("00")+"<br/>"+
                                  "Class End: "+classEnd.Hours+":"+ classEnd.Minutes.ToString("00")+"</p><br/>";
                return View(scheduleViewModel);
            }

            IEnumerable<Schedule> sList = db.getScheduleByRoomAndDay(selectedRoom, classDate);

            foreach(Schedule s in sList)
            {
                String date = s.Class_Date.ToString("dd/MM/yyyy");
                TimeSpan sEnd = s.Start_Time.Add(s.Class.Class_Length);
                if (timePicker >= s.Start_Time && timePicker < sEnd || classEnd > s.Start_Time && classEnd <= sEnd)
                {
                    ViewBag.message = "<p><span style=\"color:red\">Room Error: </span>" + s.Room.Room_Name + " is unavailable from " + s.Start_Time.Hours + ":" + s.Start_Time.Minutes.ToString("00") + " until " + sEnd.Hours + ":" + sEnd.Minutes.ToString("00") + " on " +date+"</p>";
                    return View(scheduleViewModel);
                }
                

            }

            IEnumerable<Schedule> sListbyTeacher = db.getScheduleByTeacherAndDay(selectedTeacher, classDate);

            foreach(Schedule s in sListbyTeacher)
            {
                String date = s.Class_Date.ToString("dd/MM/yyyy");
                TimeSpan sEnd = s.Start_Time.Add(s.Class.Class_Length);
                if (timePicker >= s.Start_Time && timePicker < sEnd || classEnd > s.Start_Time && classEnd <= sEnd)
                {
                    ViewBag.message = "<p><span style=\"color:red\">Availability Error: </span>" + u.U_First_Name + " "+u.U_Last_Name+" already scheduleed from " + s.Start_Time.Hours + ":" + s.Start_Time.Minutes.ToString("00") + " until " + sEnd.Hours + ":" + sEnd.Minutes.ToString("00") + " on " + date + "</p>";
                    return View(scheduleViewModel);
                }
            }



           // Add course length to timeSpan? no sure working 
            var classTime = db.getClass(selectedCLass).Class_Length;

            timePicker.Add(classTime);


            schedule.Teacher_Id = selectedTeacher;
            schedule.Class_Id = selectedCLass;
            schedule.Room_Id = selectedRoom;
            schedule.Class_Date = classDate;
            schedule.Schedule_Status = "ACTIVE";
            schedule.Start_Time = timePicker;
                       

            db.CreateSchedule(schedule);

            return RedirectToAction("ScheduleList");
        }

        [HttpGet]
        public ActionResult EditSchedule(int id)
        {
            var schedule = db.getScheduleById(id);
            ViewBag.EditSchedule = schedule;
            ViewBag.time = schedule.Start_Time.Hours + ":" + schedule.Start_Time.Minutes;
            // set dropdown model
            var classes = db.getClassList();
            var teachers = db.getTeacherList();
            var rooms = db.getRoomActiveList();

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
            var classes = db.getClassList();
            var teachers = db.getTeacherList();
            var rooms = db.getRoomList();

            var scheduleViewModel = new ScheduleViewModel
            {
                Classes = classes,
                Teachers = teachers,
                Rooms = rooms

            };

            int id = (int)TempData["EditScheduleId"];

            var schedule = db.getScheduleById(id);

            ViewBag.EditSchedule = schedule;

            // getg 

            var selectedTeacher = Convert.ToInt32(collection["SelectedTeacherId"]);
            //var selectedCLass = collection["SelectedClassId"];
            var selectedCLass = Convert.ToInt32(collection["SelectedClassId"]);
            var selectedRoom = Convert.ToInt32(collection["SelectedRoomId"]);

            DateTime classDate = Convert.ToDateTime(collection["classDate"]);
            TimeSpan timePicker = TimeSpan.Parse(collection["picker"]);

            var status = collection["status"];


            ViewBag.tid = selectedTeacher;
            ViewBag.cid = selectedCLass;
            ViewBag.rid = selectedRoom;

            XDocument xd = db.getAvailability(selectedTeacher);
            string dayOftheWeek = classDate.DayOfWeek.ToString();
            TimeSpan sTime;
            TimeSpan eTime;

            Yoga_User u = db.getUserById(selectedTeacher);

            if (DateTime.Now.Date > classDate)
            {
                ViewBag.message = "<p><span style=\"color:red\">Date Error: Can't Select Dates in the Past</span>";
                return View(scheduleViewModel);
            }

            try
            {
                sTime = TimeSpan.Parse(xd.Root.Element(dayOftheWeek).Element("Start").Value);
                eTime = TimeSpan.Parse(xd.Root.Element(dayOftheWeek).Element("End").Value);
            }
            catch
            {
                ViewBag.message = "<p><span style=\"color:red\">Availability Error: </span>" + u.U_First_Name + " " + u.U_Last_Name + " is unavailable " + dayOftheWeek.ToLower() + "s.";
                return View(scheduleViewModel);
            }

            Class c = db.getClass(selectedCLass);
            TimeSpan classEnd = timePicker.Add(c.Class_Length);

            ViewBag.time = timePicker.Hours + ":" + timePicker.Minutes;
            ViewBag.date = classDate.ToString("yyyy-MM-dd");

            // put db update method 
            if (sTime > timePicker)
            {

                ViewBag.message = "Availability Error: " + u.U_First_Name + " " + u.U_Last_Name + " starts " + dayOftheWeek + " at " + sTime.Hours + ":" + sTime.Minutes.ToString("00");
                return View(scheduleViewModel);

            }

            if (classEnd > eTime)
            {
                ViewBag.message = "<p><span style=\"color:red\">Availability Error: </span>" + u.U_First_Name + " " + u.U_Last_Name + " ends " + dayOftheWeek + " at " + eTime.Hours + ":" + eTime.Minutes.ToString("00") + "<br/>" +
                                  "Class End: " + classEnd.Hours + ":" + classEnd.Minutes.ToString("00") + "</p><br/>";
                return View(scheduleViewModel);
            }

            IEnumerable<Schedule> sList = db.getScheduleByRoomAndDay(selectedRoom, classDate);

            foreach (Schedule s in sList)
            {
                String date = s.Class_Date.ToString("dd/MM/yyyy");
                TimeSpan sEnd = s.Start_Time.Add(s.Class.Class_Length);
                if (timePicker >= s.Start_Time && timePicker < sEnd && s.Schedule_Id != schedule.Schedule_Id|| classEnd > s.Start_Time && classEnd <= sEnd && s.Schedule_Id != schedule.Schedule_Id)
                {
                    ViewBag.message = "<p><span style=\"color:red\">Room Error: </span>" + s.Room.Room_Name + " is unavailable from " + s.Start_Time.Hours + ":" + s.Start_Time.Minutes.ToString("00") + " until " + sEnd.Hours + ":" + sEnd.Minutes.ToString("00") + " on " + date + "</p>";
                    return View(scheduleViewModel);
                }

            }


            IEnumerable<Schedule> sListbyTeacher = db.getScheduleByTeacherAndDay(selectedTeacher, classDate);

            foreach (Schedule s in sListbyTeacher)
            {
                String date = s.Class_Date.ToString("dd/MM/yyyy");
                TimeSpan sEnd = s.Start_Time.Add(s.Class.Class_Length);
                if (timePicker >= s.Start_Time && timePicker < sEnd && s.Schedule_Id != schedule.Schedule_Id || classEnd > s.Start_Time && classEnd <= sEnd && s.Schedule_Id != schedule.Schedule_Id)
                {
                    ViewBag.message = "<p><span style=\"color:red\">Availability Error: </span>" + u.U_First_Name + " " + u.U_Last_Name + " is already scheduleed from " + s.Start_Time.Hours + ":" + s.Start_Time.Minutes.ToString("00") + " until " + sEnd.Hours + ":" + sEnd.Minutes.ToString("00") + " on " + date + "</p>";
                    return View(scheduleViewModel);
                }
            }

            if(schedule.Schedule_Status != status && status == "CANCELLED" && schedule.Class_Date.Date > DateTime.Now.Date)
            {
                List<Yoga_User> list = db.getScheduleSignUpList(schedule.Schedule_Id);
                db.CancelledScheduleRefund(id);
                EmailSender.ClassCancelledEmail(list, schedule);
            }

            if (schedule.Schedule_Status != status && status == "ACTIVE" && schedule.Class_Date.Date > DateTime.Now.Date)
            {
                List<Yoga_User> list = db.getScheduleSignUpList(schedule.Schedule_Id);
                db.RestoreScheduleRemoveUsers(id);
                EmailSender.ClassRestoreEmail(list, schedule);
            }

            schedule.Teacher_Id = selectedTeacher;
            schedule.Class_Id = selectedCLass;
            schedule.Room_Id = selectedRoom;
            schedule.Class_Date = classDate;
            schedule.Schedule_Status = status;

            db.UpdateSchedule(schedule); 



            return RedirectToAction("ScheduleList");
        }

        public ActionResult CancelSchedule(int id)
        {
            Schedule schedule = db.getScheduleById(id);
            db.CancelSchedule(id);
            db.CancelledScheduleRefund(id);
            List<Yoga_User> list = db.getScheduleSignUpList(schedule.Schedule_Id);
            if (list.Count >= 1)
            {
                EmailSender.ClassCancelledEmail(list, schedule);
            }

            return RedirectToAction("ScheduleList");

        }

        public ActionResult RestoreSchedule(int id)
        {
            Schedule schedule = db.getScheduleById(id);
            List<Yoga_User> list = db.getScheduleSignUpList(schedule.Schedule_Id);
            db.RestoreSchedule(id);
            db.RestoreScheduleRemoveUsers(id);
            if (list.Count >= 1)
            {
                EmailSender.ClassRestoreEmail(list, schedule);
            }

            return RedirectToAction("ScheduleList");
        }
    }
}