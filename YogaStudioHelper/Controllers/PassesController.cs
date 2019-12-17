using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace YogaStudioHelper.Controllers
{
    public class PassesController : Controller
    {

        DBMaster db = new DBMaster();

        // GET: Passes

        //View hardcoded 
        public ActionResult OnlineStore()
        {

            //String message = Util.EmailSender.sendEmail();
            //Response.Write(message);

            //IEnumerable<Promotion> promoList = db.getPromotions();
            //ViewBag.PromoList = promoList; 

            
            IEnumerable<Class_Passes> class_Pass_List = db.getClassPasses();
            return View(class_Pass_List);


        }





    }




}