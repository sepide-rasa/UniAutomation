using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniAutomation.Areas.faces.Models
{
    public class User
    {
        public int fldID { set; get; }
        public string fldName { set; get; }
        public string fldFamily { set; get; }
        public string fldUserName { set; get; }
        public string fldPassword { set; get; }
        public bool fldStatus { set; get; }
        public string fldDesc { set; get; }
        public string fldImage { set; get; }
    }
}
