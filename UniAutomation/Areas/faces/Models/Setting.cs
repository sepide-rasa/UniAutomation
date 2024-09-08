using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniAutomation.Areas.faces.Models
{
    public class Setting
    {
        public int fldId { set; get; }
        public string fldName { set; get; }
        public string fldImage { set; get; }
        public TimeSpan fldEndTime { set; get; }
        public TimeSpan fldStartTime { set; get; }
        public byte fldReservDay { set; get; }
        public string fldRais { set; get; }
        public string fldRaisKol { set; get; }
    }
}