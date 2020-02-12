using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.ViewModels
{
    public class SaleReportMonthly
    {

        //pass
        public int Pass_Id { get; set; }
        public string Pass_Name { get; set; }

        public int count { get; set; }
        public int? Total_Num_Classes { get; set; }
        public decimal? Total_Purchase_Price { get; set; }

        [System.ComponentModel.DataAnnotations.DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime Date_Purchased { get; set; }
    }
}