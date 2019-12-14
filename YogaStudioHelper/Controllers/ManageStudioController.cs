﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Database;

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
            db.ArchiveRoom(id);


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

        public ActionResult ClassPass()
        {
            return View();
        }

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

        public ActionResult CreatePromotion()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EditPromotion(int id)
        {
            var promotionEdit = db.getPromotion(id);


            ViewBag.EditPromotion = promotionEdit;


            return View();

        }

        [HttpPost]
        public ActionResult EditPromotion(FormCollection collection)
        {
            var classPassEdit = db.getClassPasse(2);


            ViewBag.EditClassPass = classPassEdit;


            return View();

        }
    }
}