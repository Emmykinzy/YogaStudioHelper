using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace YogaStudioHelper.Models
{
    public class Promotion
    {
        [Key]
        public int promotion_id { get; set; }
        public int pass_id { get; set; }
        public string promo_desc { get; set; }
        public int discount { get; set; }
        public int num_classes { get; set; }
        public DateTime promo_end { get; set; }
    }
}