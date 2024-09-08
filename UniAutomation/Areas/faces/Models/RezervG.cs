using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniAutomation.Areas.faces.Models
{
    public class RezervG
    {
        public int fldID { set; get; }
        public string fldDesc { set; get; }
        public long fldFoodCartsID { set; get; }
        public int fldFoodProgramingID { set; get; }
        public int fldPrice { set; get; }
        public int fldCount { set; get; }
        public int fldReservId { set; get; }
        public string fldFromDate { set; get; }
        public string fldToDate { set; get; }
        public string fldNobat { set; get; }
    }
}