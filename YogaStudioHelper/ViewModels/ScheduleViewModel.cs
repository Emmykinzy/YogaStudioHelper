using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.ViewModels
{
    public class ScheduleViewModel
    {
        public List<Yoga_User> Teachers { get; set; }

        public List<Class> Classes { get; set; }

        public List<Room> Rooms { get; set; }
        //public List<Class> Classes { get; set; }

    }
}