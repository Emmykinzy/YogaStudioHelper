using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace YogaStudioHelper.Models
{
    public class Class
    {
        [Key]
        public int class_id { get; set; }
        public string class_name { get; set; }
        public string class_desc { get; set; }
        public DateTime class_length { get; set; }
        public bool active { get; set; }
    }
}