using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLSTK_MoneyLover.Models;

namespace QLSTK_MoneyLover.Areas.Admin.Controllers
{
    public class TermsController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();

        // GET: Admin/Terms
        public ActionResult Index()
        {
            var termlist = db.Terms.Where(n => n.Status == 2);
            return View(termlist.ToList());
        }

        // GET: Admin/Terms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term term = db.Terms.Find(id);
            if (term == null)
            {
                return HttpNotFound();
            }
            return View(term);
        }

        // GET: Admin/Terms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Terms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Acronym,Name,MinDate,MinPayIn,Status")] Term term, string ir, string mindate)
        {
            string msg = null, msgacronym = null, msgname = null, msgmindate = null, msgminpayin = null, msgir = null;

            if (String.IsNullOrEmpty(term.Name))
            {
                msgname = "Nhập tên loại kỳ hạn !";
            }
            if (String.IsNullOrEmpty(term.Acronym))
            {
                msgacronym = "Nhập mã loại kỳ hạn !";
            }
            else
            {
                int termcount = db.Terms.Where(n => n.Acronym == term.Acronym && n.Status == 2).Count();
                if (termcount > 0)
                {
                    msgacronym = "Loại kỳ hạn này đã tồn tại !";
                }
            }
            if (String.IsNullOrEmpty(mindate))
            {
                msgmindate = "Nhập số ngày gửi tối thiểu !";
            }
            else if (term.MinDate < 0)
            {
                msgmindate = "Số ngày gửi tối thiểu không thể nhỏ hơn 0 !";
            }
            else if (term.MinDate > 3650)
            {
                msgmindate = "Số ngày gửi tối thiểu không thể lớn hơn 3650 !";
            }
            if (term.MinPayIn == 0)
            {
                msgminpayin = "Nhập số tiền gửi tối thiểu !";
            }
            else if (term.MinPayIn < 0)
            {
                msgminpayin = "Số tiền gửi tối thiểu không thể nhỏ hơn 0 !";
            }
            else if (term.MinPayIn % 50000 > 0)
            {
                msgminpayin = "Đơn vị tiền nhỏ nhất là 50.000 !";
            }
            if (String.IsNullOrEmpty(ir))
            {
                msgir = "Nhập lãi suất !";
            }
            else if (Convert.ToDouble(ir) > 100)
            {
                msgir = "Lãi suất không thể lớn hơn 100 %";
            }
            else if (Convert.ToDouble(ir) <= 0)
            {
                msgir = "Lãi suất phải lớn hơn 0 %";
            }

            if (msgacronym == null && msgname == null && msgmindate == null && msgminpayin == null && msgir == null)
            {
                msg = "completed";
                term.Status = 2;
                db.Terms.Add(term);
                Bank bank = db.Banks.FirstOrDefault(n => n.Status == 2);
                InterestRate interestRate = new InterestRate();
                interestRate.BankId = bank.Id;
                interestRate.TermId = term.Id;
                interestRate.Acronym = bank.Acronym + "_" + term.Acronym;
                interestRate.Rate = Convert.ToDouble(ir);
                interestRate.Status = 2;
                db.InterestRates.Add(interestRate);
                db.SaveChanges();
                int termid = 0;
                termid = term.Id;
                return Json(new { termid, msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msgacronym, msgname, msgmindate, msgminpayin, msgir }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Terms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term term = db.Terms.Find(id);
            if (term == null)
            {
                return HttpNotFound();
            }
            return View(term);
        }

        // POST: Admin/Terms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Acronym,Name,MinDate,MinPayIn,Status")] Term term)
        {
            if (ModelState.IsValid)
            {
                db.Entry(term).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(term);
        }

        // GET: Admin/Terms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term term = db.Terms.Find(id);
            if (term == null)
            {
                return HttpNotFound();
            }
            return View(term);
        }

        // POST: Admin/Terms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Term term = db.Terms.Find(id);
            term.Status = 3;
            db.Entry(term).State = EntityState.Modified;
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
