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
    public class InterestRatesController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();

        // GET: Admin/InterestRates
        public ActionResult Index()
        {
            var interestRates = db.InterestRates.Include(i => i.Bank).Include(i => i.Term);
            return View(interestRates.ToList());
        }

        // GET: Admin/InterestRates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InterestRate interestRate = db.InterestRates.Find(id);
            if (interestRate == null)
            {
                return HttpNotFound();
            }
            return View(interestRate);
        }

        // GET: Admin/InterestRates/Create
        public ActionResult Create()
        {
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym");
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym");
            return View();
        }

        // POST: Admin/InterestRates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BankId,TermId,Acronym,InterestRate1,Status")] InterestRate interestRate)
        {
            if (ModelState.IsValid)
            {
                db.InterestRates.Add(interestRate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym", interestRate.BankId);
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym", interestRate.TermId);
            return View(interestRate);
        }

        // GET: Admin/InterestRates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InterestRate interestRate = db.InterestRates.Find(id);
            if (interestRate == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym", interestRate.BankId);
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym", interestRate.TermId);
            return View(interestRate);
        }

        // POST: Admin/InterestRates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankId,TermId,Acronym,InterestRate1,Status")] InterestRate interestRate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(interestRate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym", interestRate.BankId);
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym", interestRate.TermId);
            return View(interestRate);
        }

        // GET: Admin/InterestRates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InterestRate interestRate = db.InterestRates.Find(id);
            if (interestRate == null)
            {
                return HttpNotFound();
            }
            return View(interestRate);
        }

        // POST: Admin/InterestRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InterestRate interestRate = db.InterestRates.Find(id);
            db.InterestRates.Remove(interestRate);
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
