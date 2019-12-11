using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace YogaStudioHelper.Models
{
    public class Class_Passes
    {
        [Key]
        public int pass_id { get; set; }
        public string pass_name { get; set; }
        public int pass_size { get; set; }
        public float pass_price { get; set; }
        public bool active { get; set; }
    }
}