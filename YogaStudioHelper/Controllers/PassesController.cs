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

      
        public ActionResult Purchase(int passId)
        {

            // veryfy paypal successfull before 


            //

            int userId = Int32.Parse(Session["Uid"].ToString());


            // create purchase log 
            Pass_Log pass_Log = new Pass_Log();

            pass_Log.Pass_Id = passId;
            pass_Log.U_Id = userId;
            // num classes 
            // price 
            // date 

            //todo add the other field above 
            pass_Log.Date_Purchased = DateTime.Now;

            //db.CreatePass_Log(pass_Log);

                       
            // todo success message with receipt etc. 

            return RedirectToAction("OnlineStore"); 
        }





    }




}