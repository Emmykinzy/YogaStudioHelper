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
    using System.ComponentModel.DataAnnotations;

    public partial class Pass_Log
    {
        public int Pass_Log_Id { get; set; }
        public int Pass_Id { get; set; }
        public int U_Id { get; set; }
        public Nullable<int> Num_Classes { get; set; }
        public Nullable<decimal> Purchase_Price { get; set; }

        //        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime Date_Purchased { get; set; }
    
        public virtual Class_Passes Class_Passes { get; set; }
        public virtual Yoga_User Yoga_User { get; set; }
    }
}
