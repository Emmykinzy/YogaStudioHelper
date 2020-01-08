using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace YogaStudioHelper.Controllers
{
    public class ProfileController : Controller
    {
        DBMaster db = new DBMaster();


        public ActionResult Classes()
        {
            return View();
        }


        // test class log 

        public ActionResult ClassLogList()
        {

            IEnumerable<Class_Log> class_Log_List = db.GetClass_Logs();

            /*
             * 
             * using (var context = new BloggingContext())
{
    var blogs = context.Blogs
        .Include(blog => blog.Posts)
            .ThenInclude(post => post.Author)
        .ToList();
}

    */


            return View(class_Log_List);
        }


        public ActionResult Passes()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult ViewAvailabilities()
        {
            int id = Int32.Parse(Session["Uid"].ToString());
            XDocument xd = db.getAvailability(id);
            return View(xd);
        }

        [HttpGet]
        public ActionResult CreateAvailabilities()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAvailabilities(FormCollection collection)
        {
            string ss = collection["sundayStart"];
            XDocument availabilities = new XDocument
           (
           new XElement("Root",
                   new XElement("Sunday",
                   new XElement("Start", collection["sundayStart"]),
                   new XElement("End", collection["sundayEnd"])),
               new XElement("Monday",
                   new XElement("Start", collection["mondayStart"]),
                   new XElement("End", collection["mondayEnd"])),
               new XElement("Tuesday",
                   new XElement("Start", collection["tuesdayStart"]),
                   new XElement("End", collection["tuesdayEnd"])),
               new XElement("Wednesday",
                   new XElement("Start", collection["wednesdayStart"]),
                   new XElement("End", collection["wednesdayEnd"])),
               new XElement("Thursday",
                   new XElement("Start", collection["thursdayStart"]),
                   new XElement("End", collection["thursdayEnd"])),
               new XElement("Friday",
                   new XElement("Start", collection["fridayStart"]),
                   new XElement("End", collection["fridayEnd"])),
               new XElement("Saturday",
                   new XElement("Start", collection["saturdayStart"]),
                   new XElement("End", collection["saturdayEnd"]))
           ));

            int id = Int32.Parse(Session["Uid"].ToString());
            db.AddAvailability(id, availabilities);
            return View();
        }
    }
}
 