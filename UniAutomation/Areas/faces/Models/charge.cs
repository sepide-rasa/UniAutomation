using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniAutomation.Areas.faces.Models
{
    public class charge
    {
        public int fldID { set; get; }
        public string fldDesc { set; get; }     
        public long fldFoodCartsID { set; get; }
        public byte fldChargeType { set; get; }
        public int fldPrice { set; get; }
    }
}