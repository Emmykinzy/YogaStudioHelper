using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YogaStudioHelper.Util;

namespace YogaStudioHelper.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Error()
        {
           Exception ex = Server.GetLastError();
            ViewBag.error = ex;
            return View();
        }
    }
}