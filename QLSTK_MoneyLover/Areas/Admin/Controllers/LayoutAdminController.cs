using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLSTK_MoneyLover.Areas.Admin.Controllers
{
    public class LayoutAdminController : Controller
    {
        // GET: Admin/LayoutAdmin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LeftPanel()
        {
            return View();
        }

        public ActionResult Header()
        {
            return View();
        }

        public ActionResult Footer()
        {
            return View();
        }

        public ActionResult Breadcome(string pagename)
        {
            ViewBag.PageName = pagename;
            return View();
        }

        public ActionResult MobileMenu()
        {
            return View();
        }
    }
}