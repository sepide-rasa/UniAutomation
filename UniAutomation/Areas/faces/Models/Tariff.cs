using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniAutomation.Areas.faces.Models
{
    public class Tariff
    {        
        public int fldID { set; get; }
        public string fldDesc { set; get; }     
        public string fldTariffDate { set; get; }
        public int fldUserType { set; get; }
        public short fldStartDate { set; get; } 
        public short fldEndDate { set; get; }
        public Boolean fldRezervType { set; get; }
        public int fldPrice { set; get; }
        public int fldFoodInfoID { set; get; }
        public byte? fldSectionID { set; get; }
        public int? fldStatusID { set; get; }
    }
}