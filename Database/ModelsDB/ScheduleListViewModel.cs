using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.ModelsDB
{
    public class ScheduleListViewModel
    {

        public int Schedule_Id { get; set; }

        public string U_First_Name { get; set; }
        public string U_Last_Name { get; set; }
        public string Class_Name { get; set; }
        public string Room_Name { get; set; }
        public TimeSpan Start_Time { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime Class_Date { get; set; }
        public int Signed_Up { get; set; }
        public Nullable<int> Group_Id { get; set; }

        public string Schedule_Status { get; set; }



    }
}
