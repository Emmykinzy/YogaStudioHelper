using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.ViewModels
{
    public class HoursWorkedMonthly
    {

        public string U_First_Name { get; set; }

        public string U_Last_Name { get; set; }

        public TimeSpan totalHours { get; set; }
        public int totalClasses { get; set; }

    }
}