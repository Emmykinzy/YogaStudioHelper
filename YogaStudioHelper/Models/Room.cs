using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace YogaStudioHelper.Models
{
    public class Room
    {
        [Key]
        public int room_id { get; set; }
        public string room_name { get; set; }
        public int room_capacity { get; set; }

    }
}