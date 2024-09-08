using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniAutomation.Areas.faces.Models
{
    public class BookStock
    {
        public int fldId { set; get; }
        public int fldBookId { set; get; }
        public string fldNashr { set; get; }
        public string fldRadeBandi_Kongere { set; get; }
        public int fldTirazh { set; get; }
        public byte fldNobateChap { set; get; }
        public Boolean fldActive_DeActive { set; get; }
    }
}