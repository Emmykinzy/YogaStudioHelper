using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace YogaStudioHelper.Models
{
    public class Class_Log
    {
        [Key]
        public int class_log_id { get; set; }
        public int schedule_id { get; set; }
        public int u_id { get; set; }
        public string log_status { get; set; }

    }
}