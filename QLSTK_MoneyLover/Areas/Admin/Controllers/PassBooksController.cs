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
    public class PassBooksController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();

        // GET: Admin/PassBooks
        public ActionResult Index()
        {
            //var passBooks = db.PassBooks.Include(p => p.Bank).Include(p => p.Customer).Include(p => p.Term);
            var passBooks = db.PassBooks.Where(n => n.Status != 0);
            return View(passBooks.ToList());
        }

        // GET: Admin/PassBooks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassBook passBook = db.PassBooks.Find(id);
            if (passBook == null)
            {
                return HttpNotFound();
            }
            return View(passBook);
        }

        // GET: Admin/PassBooks/Create
        public ActionResult Create()
        {
            var custlist = db.Customers.Where(n => n.Status != 0);
            var termlist = db.Terms.Where(n => n.Status == 2);
            ViewBag.CustomerId = new SelectList(custlist, "Id", "Acronym");
            ViewBag.TermId = new SelectList(termlist, "Id", "Acronym");
            return View();
        }

        // POST: Admin/PassBooks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,BankId,TermId,CustomerId,Acronym,Balance,OpenDate,InterestPayment,TermEnd,Status,Principal,ChangeDate,InterestRate,DemandInterestRate")] PassBook passBook)
        {
            string msg = null, msgcustid = null, msgtermid = null, msgacronym = null, msgprincipal = null, msgopendate = null;
            DateTime mindate = new DateTime(2000, 01, 01);
            
            if (passBook.TermId == 0)
            {
                msgtermid = "Chọn loại kỳ hạn !";
            }
            if (String.IsNullOrEmpty(passBook.Acronym))
            {
                msgacronym = "Nhập mã sổ !";
            }
            else
            {
                int pbcount = db.PassBooks.Where(n => n.Acronym == passBook.Acronym).Count();
                if (pbcount > 0)
                {
                    msgacronym = "Sổ này đã tồn tại !";
                }
            }
            if (passBook.Principal == 0)
            {
                msgprincipal = "Nhập số tiền gửi !";
            }
            else if (passBook.Principal < 1000000)
            {
                msgprincipal = "Số tiền gửi không thể nhỏ hơn 1.000.000 !";
            }
            else if (passBook.Principal % 50000 > 0)
            {
                msgprincipal = "Đơn vị tiền nhỏ nhất là 50.000 !";
            }
            if (passBook.OpenDate < mindate)
            {
                msgopendate = "Ngày gửi không thể nhỏ hơn 01/01/2000 !";
            }
            else if (passBook.OpenDate > DateTime.Now)
            {
                msgopendate = "Ngày gửi không thể lớn hơn ngày hiện tại !";
            }

            if (msgcustid == null && msgtermid == null && msgacronym == null && msgprincipal == null && msgopendate == null)
            {
                msg = "completed";
                var res = FindIR(passBook.TermId);
                passBook.BankId = db.Banks.FirstOrDefault(n => n.Status == 2).Id;
                passBook.InterestRate = res.rate;
                passBook.DemandInterestRate = res.demandrate;
                passBook.InterestPayment = 1;
                passBook.TermEnd = 1;
                passBook.Balance = passBook.Principal;
                passBook.ChangeDate = passBook.OpenDate;
                passBook.Status = 1;
                db.PassBooks.Add(passBook);
                db.SaveChanges();
                int pbid = 0;
                pbid = passBook.Id;
                return Json(new { pbid, msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msgcustid, msgtermid, msgacronym, msgprincipal, msgopendate }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/PassBooks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassBook passBook = db.PassBooks.Find(id);
            if (passBook == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym", passBook.BankId);
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Acronym", passBook.CustomerId);
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym", passBook.TermId);
            return View(passBook);
        }

        // POST: Admin/PassBooks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankId,TermId,CustomerId,Acronym,Balance,OpenDate,InterestPayment,TermEnd,Status,Principal,ChangeDate,InterestRate,DemandInterestRate")] PassBook passBook)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passBook).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym", passBook.BankId);
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Acronym", passBook.CustomerId);
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym", passBook.TermId);
            return View(passBook);
        }

        // GET: Admin/PassBooks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassBook passBook = db.PassBooks.Find(id);
            if (passBook == null)
            {
                return HttpNotFound();
            }
            int depcount = db.Deposits.Where(n => n.PassBookId == passBook.Id && n.Status == 1).Count();
            int witcount = db.Withdraws.Where(n => n.PassBookId == passBook.Id && n.Status == 1).Count();
            ViewBag.DepCount = depcount;
            ViewBag.WitCount = witcount;
            return View(passBook);
        }

        // POST: Admin/PassBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PassBook passBook = db.PassBooks.Find(id);
            passBook.Status = 0;
            db.Entry(passBook).State = EntityState.Modified;
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

        public JsonResult CheckCustomer(string custid)
        {
            int a = 0;
            string adress = "", idcard = "";
            if (!String.IsNullOrEmpty(custid))
            {
                a = int.Parse(custid);
                Customer customer = db.Customers.Find(a);
                adress = customer.Adress;
                idcard = customer.IdentityCard;
            }
            return Json(new { adress, idcard }, JsonRequestBehavior.AllowGet);
        }

        private dynamic FindIR(int termid)
        {
            double rate = 0, demandrate = 0;
            int bankid = db.Banks.FirstOrDefault(n => n.Status == 2).Id;
            InterestRate demandIr = db.InterestRates.FirstOrDefault(n => n.BankId == bankid && n.Term.Acronym == "KKH");
            if (demandIr != null)
            {
                demandrate = demandIr.Rate;
            }
            if (termid != 0)
            {
                InterestRate ir = db.InterestRates.FirstOrDefault(n => n.BankId == bankid && n.TermId == termid);
                if (ir != null)
                {
                    rate = ir.Rate;
                }
            }
            var res = new { rate, demandrate };
            return res;
        }
    }
}
