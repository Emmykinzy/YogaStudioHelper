using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.Filters
{
    // AuthorizeAdmin
    public class AuthorizeAdmin : System.Web.Mvc.ActionFilterAttribute, System.Web.Mvc.IActionFilter
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            //             if (HttpContext.Current.Session["Auth"] != 1)


            //
            if (Convert.ToInt32(HttpContext.Current.Session["Auth"]) != 1)
            {
                filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    {"Controller", "LoginSignUp"},
                    {"Action", "LogInSignUp"}
                });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}