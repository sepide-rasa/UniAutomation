using System.Web.Mvc;

namespace UniAutomation.Areas.faces
{
    public class facesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "faces";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "faces_default",
                "faces/{controller}/{action}/{id}",
                new { controller="admin",action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
