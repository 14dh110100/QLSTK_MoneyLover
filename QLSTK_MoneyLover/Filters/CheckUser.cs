using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QLSTK_MoneyLover.Filters
{
    public class CheckUser : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["userid"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                                    new RouteValueDictionary(new { controller = "Home", action = "Login", msg = "SessionMissing" })
                                );
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["userid"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                                    new RouteValueDictionary(new { controller = "Home", action = "Login", msg = "SessionMissing" })
                                );
            }

            base.OnResultExecuting(filterContext);
        }
    }
}