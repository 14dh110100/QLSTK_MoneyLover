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
            var passBooks = db.PassBooks.Include(p => p.Bank).Include(p => p.Customer).Include(p => p.Term);
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
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym");
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Acronym");
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym");
            return View();
        }

        // POST: Admin/PassBooks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BankId,TermId,CustomerId,Acronym,Balance,OpenDate,InterestPayment,TermEnd,Status,Principal,ChangeDate,InterestRate,DemandInterestRate")] PassBook passBook)
        {
            if (ModelState.IsValid)
            {
                db.PassBooks.Add(passBook);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym", passBook.BankId);
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Acronym", passBook.CustomerId);
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym", passBook.TermId);
            return View(passBook);
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
            return View(passBook);
        }

        // POST: Admin/PassBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PassBook passBook = db.PassBooks.Find(id);
            db.PassBooks.Remove(passBook);
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
