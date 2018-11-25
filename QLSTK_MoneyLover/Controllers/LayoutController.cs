using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLSTK_MoneyLover.Controllers
{
    public class LayoutController : Controller
    {
        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Header()
        {
            return View();
        }

        public ActionResult Banner(string banner)
        {
            ViewBag.Banner = banner;
            return View();
        }

        public ActionResult Footer()
        {
            return View();
        }

        public ActionResult Modal()
        {
            return View();
        }
    }
}