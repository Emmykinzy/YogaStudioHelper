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
        // GET: Passes
        public ActionResult OnlineStore()
        {

            String message = Util.EmailSender.sendEmail();

            Response.Write(message);
            return View();
        }



    }



  
}