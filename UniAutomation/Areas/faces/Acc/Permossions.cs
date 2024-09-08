using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniAutomation.Areas.faces.Controllers.Acc
{
    public class Permossions
    {
        public static bool haveAccess(int userId, int RolId)
        {
            Models.UniAutomationEntities1 p = new Models.UniAutomationEntities1();
            var q = p.sp_tblPermissionSelect("HaveAcces", RolId, userId).Any();
            return q;
        }   
    }
}