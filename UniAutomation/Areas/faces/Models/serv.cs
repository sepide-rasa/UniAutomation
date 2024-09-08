using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniAutomation.Areas.faces.Models
{
    public class serv
    {
        public int fldID { set; get; }
        public TimeSpan fldStartTime { set; get; }
        public TimeSpan fldEndTime { set; get; }
        public string fldDesc { set; get; }
        public Boolean fldRezervType { set; get; }
        public int? fldNumber { set; get; }
        public int? fldFoodPrograminID { set; get; }
        public int? fldNobat { set; get; }
        public int? fldUserType { set; get; }
    }
}