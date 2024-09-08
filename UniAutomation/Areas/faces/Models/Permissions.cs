using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniAutomation.Areas.faces.Models
{
    public class Permissions
    {
        public int fldId { set; get; }
        public int GroupId { set; get; }
        public int RolId { set; get; }
        public string fldDesc { set; get; }
        public int fldUserID { set; get; }
    }
}
