using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YogaStudioHelper.ViewModels
{
    public class ScheduleViewModel
    {
        public List<Yoga_User> Teachers { get; set; }
        public int SelectedTeacherId { get; set; }
        public List<Class> Classes{ get; set; }
        public SelectList ClasseEdit{ get; set; }

        public int SelectedClassId { get; set; }

        public List<Room> Rooms { get; set; }
        public int SelectedRoomId { get; set; }

        //public List<Class> Classes { get; set; }

    }
}