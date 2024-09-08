using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniAutomation.Areas.faces.Models
{
    public class Limit
    {
        public int fldID { set; get; }
        public string fldDesc { set; get; }
        public string fldRoleDate { set; get; }
        public Boolean fldPersonal { set; get; }
        public Boolean fldStudent { set; get; }
        public Boolean fldTeacher { set; get; }
    }
}