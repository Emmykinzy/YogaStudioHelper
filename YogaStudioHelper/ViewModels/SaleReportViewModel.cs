using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.ViewModels
{
    public class SaleReportViewModel
    {

        public int Pass_Log_Id { get; set; }
       
        //pass
        public int Pass_Id { get; set; }
        public string Pass_Name { get; set; }

        // user
        public int U_Id { get; set; }
        public string U_First_Name { get; set; }
        public string U_Last_Name { get; set; }


        public int Num_Classes { get; set; }
        public double Purchase_Price { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime Date_Purchased { get; set; }

        //public Nullable<int> Invoice_Number { get; set; }


    }
}