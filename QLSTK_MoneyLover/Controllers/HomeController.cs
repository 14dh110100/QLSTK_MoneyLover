using QLSTK_MoneyLover.Filters;
using QLSTK_MoneyLover.Models;
using QLSTK_MoneyLover.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLSTK_MoneyLover.Controllers
{
    public class HomeController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();
        public ActionResult Index()
        {
            if (Session["userid"] != null)
            {
                return RedirectToAction("Index", "PassBook");
            }
            return View();
        }

        public ActionResult Login(string msg)
        {
            ViewBag.Msg = "";
            if (msg == "SessionMissing")
            {
                ViewBag.Msg = "Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại !";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            string msg = null, msgun = null, msgpw = null;
            if (String.IsNullOrEmpty(username))
            {
                msgun = "Nhập Email !";
            }
            if (String.IsNullOrEmpty(password))
            {
                msgpw = "Nhập mật khẩu !";
            }

            if (msgun == null && msgpw == null)
            {
                string pwEncrypted = MD5Encrypt.ConvertMD5(password);
                var user = db.Customers.SingleOrDefault(n => n.UserName == username && n.Encrypted == pwEncrypted);
                if (user != null)
                {
                    msg = "completed";
                    Session["userid"] = user.Id;
                    return Json(new { msg }, JsonRequestBehavior.AllowGet);
                }
                else {
                    msgun = "Email hoặc mật khẩu không chính xác !";
                }
            }
            return Json(new { msgun, msgpw }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Register([Bind(Include = "Id,Acronym,Name,IdentityCard,Adress,UserName,Password,Encrypted,Status")] Customer customer, string repass)
        {
            string msg = null, msgname = null, msgun = null, msgpw = null, msgrepw = null;
            if (String.IsNullOrEmpty(customer.Name))
            {
                msgname = "Nhập tên !";
            }
            if (String.IsNullOrEmpty(customer.UserName))
            {
                msgun = "Nhập Email !";
            }
            else if (db.Customers.Where(n => n.UserName == customer.UserName).Count() > 0)
            {
                msgun = "Email này đã được sử dụng !";
            }
            if (String.IsNullOrEmpty(customer.Password))
            {
                msgpw = "Nhập mật khẩu !";
            }
            if (String.IsNullOrEmpty(repass))
            {
                msgrepw = "Nhập mật khẩu !";
            }
            else if (!String.IsNullOrEmpty(customer.Password) && repass != customer.Password)
            {
                msgrepw = "Nhập lại mật khẩu chưa chính xác !";
            }

            if (msgname == null && msgun == null && msgpw == null && msgrepw == null)
            {
                msg = "completed";
                customer.Acronym = "Testing";
                customer.Encrypted = MD5Encrypt.ConvertMD5(customer.Password);
                customer.Status = 1;
                db.Customers.Add(customer);
                db.SaveChanges();
                Session["userid"] = customer.Id;
                return Json(new { msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msgname, msgun, msgpw, msgrepw }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session["userid"] = null;
            return RedirectToAction("Login", "Home");
        }
    }
}