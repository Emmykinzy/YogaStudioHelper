using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace YogaStudioHelper.Models
{
    public class Pass_Log
    {
        [Key]
        public int pass_log_id { get; set; }
        public int pass_id { get; set; }
        public int u_id { get; set; }
        public int num_classes { get; set; }
        public float purchase_price { get; set; }
        public DateTime date_purchased { get; set; }
    }
}