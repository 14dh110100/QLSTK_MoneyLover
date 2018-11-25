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
            var withdraws = db.Withdraws.Include(w => w.PassBook);
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
            ViewBag.PassBookId = new SelectList(db.PassBooks, "Id", "Acronym");
            return View();
        }

        // POST: Admin/Withdraws/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PassBookId,Acronym,WithdrawDate,Amount,Status")] Withdraw withdraw)
        {
            if (ModelState.IsValid)
            {
                db.Withdraws.Add(withdraw);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PassBookId = new SelectList(db.PassBooks, "Id", "Acronym", withdraw.PassBookId);
            return View(withdraw);
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
            db.Withdraws.Remove(withdraw);
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
