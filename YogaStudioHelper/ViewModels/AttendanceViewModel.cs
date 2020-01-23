using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.ViewModels
{
    public class AttendanceViewModel
    {


        public int scheduleId;
        public string className;

        public TimeSpan startTime;

        public int SignUp;
        public int attended;
        public int missed; 




        public List<Yoga_User> Students { get; set; }

        /*
         * 
         * 
         * dayli 
         * 
         * each class attendance 
         * 
         * each month 
         * total for each class type 
         * 
         *  
         * 
         * 
         * 
         * 
         */
    }
}