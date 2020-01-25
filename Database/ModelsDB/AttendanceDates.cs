using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.ModelsDB
{
    public class AttendanceDates
    {
        public int classId { get; set; }
        public string className { get; set; }

        public int SignUp { get; set; }
        public int attended { get; set; }
        public int missed { get; set; }
    }
}
