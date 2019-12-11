using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace YogaStudioHelper.Models
{
    public class Schedule
    { 
        [Key]
        public int schedule_id { get; set; }
        public int teacher_id { get; set; }
        public int class_id { get; set; }
        public int room_id { get; set; }
        public DateTime start_time { get; set; }
        public int signed_up { get; set; }
        public int group_id { get; set; }
        public string schedule_status { get; set; }
    }
}