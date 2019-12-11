using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;

namespace YogaStudioHelper.Models
{
    public class YogaUser
    {
        [Key]
        public int u_id { get; set; }
        public int roles_id { get; set; }
        [Required(ErrorMessage = "Role Id Required")]
        public string u_first_name { get; set; }
        [Required(ErrorMessage = "First Name Required")]
        [MinLength(1, ErrorMessage = "Customer Name Minimum Length 1")]
        [MaxLength(35, ErrorMessage = "Customer Name Maximum Length 35")]
        public string u_last_name { get; set; }
        [Required(ErrorMessage = "Last Name Required")]
        [MinLength(1, ErrorMessage = "Customer Name Minimum Length 1")]
        [MaxLength(35, ErrorMessage = "Customer Name Maximum Length 35")]
        public string u_email { get; set; }
        [Required(ErrorMessage = "Email Required")]
        public string u_phone { get; set; }
        public XDocument availability { get; set; }
        public DateTime u_birthday { get; set; } 
        public bool active { get; set; }

        public string u_password { get; set; }


    }

    /*
     * 
     *     
        public int U_Id { get; set; }
        public int Roles_Id { get; set; }
        public string U_First_Name { get; set; }
        public string U_Last_Name { get; set; }
        public string U_Email { get; set; }
        public string U_Password { get; set; }
        public string U_Phone { get; set; }
        public string Availability { get; set; }
        public Nullable<System.DateTime> U_Birthday { get; set; }
        public Nullable<bool> Active { get; set; }

    */
}
 