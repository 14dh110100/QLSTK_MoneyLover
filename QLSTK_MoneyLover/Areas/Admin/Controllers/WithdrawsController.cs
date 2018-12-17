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
    public class WithdrawsController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();

        // GET: Admin/Withdraws
        public ActionResult Index()
        {
            //var withdraws = db.Withdraws.Include(w => w.PassBook);
            var withdraws = db.Withdraws.Where(n => n.Status != 0);
            return View(withdraws.ToList());
        }

        // GET: Admin/Withdraws/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Withdraw withdraw = db.Withdraws.Find(id);
            if (withdraw == null)
            {
                return HttpNotFound();
            }
            return View(withdraw);
        }

        // GET: Admin/Withdraws/Create
        public ActionResult Create()
        {
            var pblist = db.PassBooks.Where(n => n.Status == 1);
            ViewBag.PassBookId = new SelectList(pblist, "Id", "Acronym");
            return View();
        }

        // POST: Admin/Withdraws/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,PassBookId,Acronym,WithdrawDate,Amount,Status")] Withdraw withdraw)
        {
            string msg = null, msgacronym = null, msgpbid = null, msgwitdate = null, msgamount = null;
            DateTime mindate = new DateTime(2000, 01, 01);

            if (String.IsNullOrEmpty(withdraw.Acronym))
            {
                msgacronym = "Nhập mã phiếu rút !";
            }
            else
            {
                int witcount = db.Withdraws.Where(n => n.Acronym == withdraw.Acronym).Count();
                if (witcount > 0)
                {
                    msgacronym = "Phiếu rút này đã tồn tại !";
                }
            }
            if (withdraw.PassBookId == 0)
            {
                msgpbid = "Chưa có sổ tiết kiệm !";
            }
            if (withdraw.Amount == 0)
            {
                msgamount = "Nhập số tiền rút !";
            }
            else if (withdraw.Amount < 0)
            {
                msgamount = "Số tiền rút không thể nhỏ hơn 0 !";
            }
            //else if (withdraw.Amount % 50000 > 0)
            //{
            //    msgamount = "Đơn vị tiền nhỏ nhất là 50.000 !";
            //}
            else if (withdraw.PassBookId != 0)
            {
                PassBook passBook = db.PassBooks.Find(withdraw.PassBookId);
                if (withdraw.Amount > passBook.Balance)
                {
                    msgamount = "Số tiền rút không thể lớn hơn số dư hiện có : " + passBook.Balance;
                }
            }
            if (withdraw.WithdrawDate < mindate)
            {
                msgwitdate = "Ngày rút không thể nhỏ hơn 01/01/2000 !";
            }
            else if (withdraw.WithdrawDate > DateTime.Now)
            {
                msgwitdate = "Ngày rút không thể lớn hơn ngày hiện tại !";
            }
            else if (withdraw.PassBookId != 0)
            {
                PassBook passBook = db.PassBooks.Find(withdraw.PassBookId);
                if (withdraw.WithdrawDate < passBook.OpenDate)
                {
                    msgwitdate = "Ngày rút không thể nhỏ hơn ngày mở sổ !";
                }
            }

            if (msgacronym == null && msgpbid == null && msgwitdate == null && msgamount == null)
            {
                msg = "completed";
                withdraw.Status = 1;
                db.Withdraws.Add(withdraw);
                PassBook passBook = db.PassBooks.Find(withdraw.PassBookId);
                passBook.Balance -= withdraw.Amount;
                if (passBook.Balance == 0)
                {
                    passBook.Status = 2;
                }
                db.SaveChanges();
                var res = Calculate(withdraw.PassBookId, withdraw.WithdrawDate, withdraw.Amount);
                decimal interest = (decimal)res.interest;
                decimal interestKKH = (decimal)res.interestKKH;
                int witid = 0;
                witid = withdraw.Id;
                return Json(new { witid, msg, interest, interestKKH }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msgacronym, msgpbid, msgwitdate, msgamount }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Withdraws/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Withdraw withdraw = db.Withdraws.Find(id);
            if (withdraw == null)
            {
                return HttpNotFound();
            }
            ViewBag.PassBookId = new SelectList(db.PassBooks, "Id", "Acronym", withdraw.PassBookId);
            return View(withdraw);
        }

        // POST: Admin/Withdraws/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PassBookId,Acronym,WithdrawDate,Amount,Status")] Withdraw withdraw)
        {
            if (ModelState.IsValid)
            {
                db.Entry(withdraw).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PassBookId = new SelectList(db.PassBooks, "Id", "Acronym", withdraw.PassBookId);
            return View(withdraw);
        }

        // GET: Admin/Withdraws/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Withdraw withdraw = db.Withdraws.Find(id);
            if (withdraw == null)
            {
                return HttpNotFound();
            }
            return View(withdraw);
        }

        // POST: Admin/Withdraws/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Withdraw withdraw = db.Withdraws.Find(id);
            withdraw.Status = 0;
            db.Entry(withdraw).State = EntityState.Modified;
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

        public JsonResult CheckTerm(string pbid, string strwitdate)
        {
            int a = 0;
            string result = "", acronym = "";
            decimal balance = 0;
            DateTime depdate = new DateTime(2000, 01, 01);
            if (!String.IsNullOrEmpty(pbid))
            {
                a = int.Parse(pbid);
                PassBook passBook = db.PassBooks.Find(a);
                balance = passBook.Balance;
                if (!String.IsNullOrEmpty(strwitdate))
                {
                    depdate = DateTime.Parse(strwitdate);
                }
                int checkDays = (depdate - passBook.OpenDate).Days;
                if (checkDays >= passBook.Term.MinDate)
                {
                    result = "yes";
                    if (passBook.Term.Acronym == "KKH")
                    {
                        acronym = "KKH";
                    }
                }
                else
                {
                    result = "no";
                }
            }
            return Json(new { result, acronym, balance }, JsonRequestBehavior.AllowGet);
        }

        private dynamic Calculate(int id, DateTime witdate, decimal amount)
        {
            PassBook passBook = db.PassBooks.SingleOrDefault(n => n.Id == id);
            double rate = passBook.InterestRate, demandrate = passBook.DemandInterestRate;
            double interest = 0, interestKKH = 0;
            int checkDays = (witdate - passBook.ChangeDate).Days;
            if (checkDays < passBook.Term.MinDate)
            {
                //Tính lãi suất không kỳ hạn
                interestKKH = ((double)amount * (demandrate / 100) / 360) * checkDays;
            }
            else if (checkDays == passBook.Term.MinDate)
            {
                //Tính lãi suất đúng kỳ hạn
                interest = ((double)amount * (rate / 100) / 360) * passBook.Term.MinDate;
            }
            else
            {
                //Tính lãi suất đúng kỳ hạn + lãi suất không kỳ hạn
                if (passBook.Term.Acronym == "KKH")
                {
                    passBook.Term.MinDate = 360;
                }
                int checkTerms = checkDays / passBook.Term.MinDate;
                double currentBal = (double)amount;
                DateTime changeDate = passBook.ChangeDate;
                //Vòng lặp tính lãi suất mỗi lần đúng kỳ hạn
                for (int i = 0; i < checkTerms; i++)
                {
                    interest = (currentBal * (rate / 100) / 360) * passBook.Term.MinDate;
                    changeDate = changeDate.AddDays(passBook.Term.MinDate);
                    
                    currentBal += interest;
                }
                passBook.ChangeDate = changeDate;
                passBook.Balance = decimal.Parse(currentBal.ToString("F"));
                passBook.TermEnd = 1;
                interestKKH = (currentBal * (demandrate / 100) / 360) * (checkDays - (passBook.Term.MinDate * checkTerms));
            }
            interest = double.Parse(interest.ToString("F"));
            interestKKH = double.Parse(interestKKH.ToString("F"));
            var res = new
            {
                interest,
                interestKKH
            };
            return res;
        }

        public JsonResult ShowResult(string interest, string interestKKH)
        {
            string a = interest;
            string b = interestKKH;
            return Json(new { a, b }, JsonRequestBehavior.AllowGet);
        }
    }
}
