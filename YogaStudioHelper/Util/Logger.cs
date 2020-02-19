using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.Util
{
    public class Logger
    {


        public static void Log(string logMessage)
        {
            string error;
            error ="\r\nLog Entry : \n"+DateTime.Now.ToLongTimeString()+" "+DateTime.Now.ToLongDateString()+"  \n"+logMessage;
            EmailSender.sendEmail(error);
        }

    }
}