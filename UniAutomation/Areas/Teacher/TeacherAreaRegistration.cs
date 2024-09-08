﻿using System.Web.Mvc;

namespace UniAutomation.Areas.Teacher
{
    public class TeacherAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Teacher";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Teacher_default",
                "Teacher/{controller}/{action}/{id}",
                new { controller = "TeacherLogin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
