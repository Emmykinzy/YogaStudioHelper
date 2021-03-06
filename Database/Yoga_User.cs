//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Yoga_User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Yoga_User()
        {
            this.Class_Log = new HashSet<Class_Log>();
            this.Pass_Log = new HashSet<Pass_Log>();
            this.Schedules = new HashSet<Schedule>();
        }
    
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
        public Nullable<int> U_Tokens { get; set; }
        public string Email_Confirmation { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Class_Log> Class_Log { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pass_Log> Pass_Log { get; set; }
        public virtual Role Role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
