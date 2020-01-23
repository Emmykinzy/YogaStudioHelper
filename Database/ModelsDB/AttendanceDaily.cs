using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.Models
{
    public class AttendanceDaily
    {

        public int scheduleId { get; set; }
        public string className { get; set; }

        public TimeSpan startTime { get; set; }

        public int SignUp { get; set; }
        public int attended { get; set; }
        public int missed { get; set; }


    }
}