using System;
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
        public ActionResult EditRoom()
        {
            return View();
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

        public ActionResult EditClass()
        {
            return View();
        }

        public ActionResult ClassPass()
        {
            return View();
        }

        public ActionResult FindClassPass()
        {
            return View();
        }

        public ActionResult ClassPassList()
        {
            return View();
        }

        public ActionResult CreateClassPass()
        {
            return View();
        }

        public ActionResult EditClassPass()
        {
            return View();
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
            return View();
        }

        public ActionResult CreatePromotion()
        {
            return View();
        }

        public ActionResult EditPromotion()
        {
            return View();
        }
    }
}