using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniAutomation.Areas.faces.Models
{
    public class Teacher
    {
        public int fldID { set; get; }
        public string fldName { set; get; }
        public string fldFamily { set; get; }
        public string fldMeliCode { set; get; }
        public short fldYear { set; get; }
        public string fldMobile { set; get; }
        public string fldEmail { set; get; }
        public string fldDesc { set; get; }
        public string fldPic { set; get; }
        public byte fldCourceId { set; get; }
    }
}