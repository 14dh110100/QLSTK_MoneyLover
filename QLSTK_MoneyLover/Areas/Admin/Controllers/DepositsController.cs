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
    public class DepositsController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();

        // GET: Admin/Deposits
        public ActionResult Index()
        {
            //var deposits = db.Deposits.Include(d => d.PassBook);
            var deposits = db.Deposits.Where(n => n.Status != 0);
            return View(deposits.ToList());
        }

        // GET: Admin/Deposits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            return View(deposit);
        }

        // GET: Admin/Deposits/Create
        public ActionResult Create()
        {
            var pblist = db.PassBooks.Where(n => n.Status != 0);
            ViewBag.PassBookId = new SelectList(pblist, "Id", "Acronym");
            return View();
        }

        // POST: Admin/Deposits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,PassBookId,Acronym,DepositDate,Amount,Status")] Deposit deposit)
        {
            string msg = null, msgacronym = null, msgpbid = null, msgdepdate = null, msgamount = null;
            DateTime mindate = new DateTime(2000, 01, 01);

            if (String.IsNullOrEmpty(deposit.Acronym))
            {
                msgacronym = "Nhập mã phiếu gởi !";
            }
            else
            {
                int depcount = db.Deposits.Where(n => n.Acronym == deposit.Acronym).Count();
                if (depcount > 0)
                {
                    msgacronym = "Phiếu gởi này đã tồn tại !";
                }
            }
            if (deposit.PassBookId == 0)
            {
                msgpbid = "Chưa có sổ tiết kiệm !";
            }
            if (deposit.Amount == 0)
            {
                msgamount = "Nhập số tiền gửi !";
            }
            else if (deposit.Amount < 100000)
            {
                msgamount = "Số tiền gửi tối thiểu là 100.000 !";
            }
            else if (deposit.Amount % 50000 > 0)
            {
                msgamount = "Đơn vị tiền nhỏ nhất là 50.000 !";
            }
            if (deposit.DepositDate < mindate)
            {
                msgdepdate = "Ngày gửi không thể nhỏ hơn 01/01/2000 !";
            }
            else if (deposit.DepositDate > DateTime.Now)
            {
                msgdepdate = "Ngày gửi không thể lớn hơn ngày hiện tại !";
            }
            else if (deposit.PassBookId != 0)
            {
                PassBook passBook = db.PassBooks.Find(deposit.PassBookId);
                if (deposit.DepositDate < passBook.OpenDate)
                {
                    msgdepdate = "Ngày gửi không thể nhỏ hơn ngày mở sổ !";
                }
            }

            if (msgacronym == null && msgpbid == null && msgdepdate == null && msgamount == null)
            {
                msg = "completed";
                deposit.Status = 1;
                db.Deposits.Add(deposit);
                PassBook passBook = db.PassBooks.Find(deposit.PassBookId);
                passBook.Balance += deposit.Amount;
                db.SaveChanges();
                int depid = 0;
                depid = deposit.Id;
                return Json(new { depid, msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msgacronym, msgpbid, msgdepdate, msgamount }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Deposits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            ViewBag.PassBookId = new SelectList(db.PassBooks, "Id", "Acronym", deposit.PassBookId);
            return View(deposit);
        }

        // POST: Admin/Deposits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PassBookId,Acronym,DepositDate,Amount,Status")] Deposit deposit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deposit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PassBookId = new SelectList(db.PassBooks, "Id", "Acronym", deposit.PassBookId);
            return View(deposit);
        }

        // GET: Admin/Deposits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            return View(deposit);
        }

        // POST: Admin/Deposits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Deposit deposit = db.Deposits.Find(id);
            deposit.Status = 0;
            db.Entry(deposit).State = EntityState.Modified;
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

        public JsonResult CheckTerm(string pbid, string strdepdate)
        {
            int a = 0;
            string result = "";
            DateTime depdate = new DateTime(2000, 01, 01);
            if (!String.IsNullOrEmpty(pbid))
            {
                a = int.Parse(pbid);
                PassBook passBook = db.PassBooks.Find(a);
                if (!String.IsNullOrEmpty(strdepdate))
                {
                    depdate = DateTime.Parse(strdepdate);
                }
                int checkDays = (depdate - passBook.OpenDate).Days;
                if (checkDays == 0 || checkDays == passBook.Term.MinDate)
                {
                    result = "yes";
                }
                else
                {
                    result = "no";
                }
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
    }
}
