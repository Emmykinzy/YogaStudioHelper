using Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.ViewModels
{
    public class HoursWorkedViewModel
    {
        public List<Yoga_User> Teachers { get; set; }


        // date 
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime Class_Date { get; set; }

        // name 
        public string U_First_Name { get; set; }
        public string U_Last_Name { get; set; }


        // class name 
        public string Class_Name { get; set; }

        // Lenght 
        public TimeSpan Class_Length { get; set; }


    }
}