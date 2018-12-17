using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLSTK_MoneyLover.Areas.Admin.Models.Security;
using QLSTK_MoneyLover.Models;

namespace QLSTK_MoneyLover.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();

        // GET: Admin/Customers
        public ActionResult Index()
        {
            var customers = db.Customers.Where(n => n.Status != 0);
            return View(customers.ToList());
        }

        // GET: Admin/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Admin/Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Acronym,Name,IdentityCard,Adress,UserName,Password,Encrypted,Status")] Customer customer)
        {
            string msg = null, msgacronym = null, msgname = null, msgidcard = null, msgadress = null;
            DateTime mindate = new DateTime(2000, 01, 01);

            if (String.IsNullOrEmpty(customer.Acronym))
            {
                msgacronym = "Nhập mã khách hàng !";
            }
            else
            {
                int depcount = db.Customers.Where(n => n.Acronym == customer.Acronym).Count();
                if (depcount > 0)
                {
                    msgacronym = "Mã khách hàng này đã tồn tại !";
                }
            }
            if (String.IsNullOrEmpty(customer.Name))
            {
                msgname = "Nhập tên khách hàng !";
            }
            if (String.IsNullOrEmpty(customer.IdentityCard))
            {
                msgidcard = "Nhập CMND !";
            }
            else if (customer.IdentityCard.Length < 9)
            {
                msgidcard = "CMND phải từ 9 chữ số trờ lên !";
            }
            else
            {
                int depcount = db.Customers.Where(n => n.IdentityCard == customer.IdentityCard).Count();
                if (depcount > 0)
                {
                    msgidcard = "CMND này đã đăng ký !";
                }
            }
            if (String.IsNullOrEmpty(customer.Adress))
            {
                msgadress = "Nhập địa chỉ khách hàng !";
            }
            
            if (msgacronym == null && msgname == null && msgidcard == null && msgadress == null)
            {
                msg = "completed";
                customer.UserName = customer.IdentityCard;
                customer.Password = customer.Acronym + "_" + customer.IdentityCard;
                customer.Encrypted = MD5EncryptAdmin.ConvertMD5Admin(customer.Password);
                customer.Status = 1;
                db.Customers.Add(customer);
                db.SaveChanges();
                int custid = 0;
                custid = customer.Id;
                return Json(new { custid, msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msgacronym, msgname, msgidcard, msgadress }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Acronym,Name,IdentityCard,Adress,UserName,Password,Encrypted,Status")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            customer.Status = 0;
            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
