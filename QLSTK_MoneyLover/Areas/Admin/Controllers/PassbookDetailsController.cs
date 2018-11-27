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
    public class PassbookDetailsController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();

        // GET: Admin/PassbookDetails
        public ActionResult Index()
        {
            var passbookDetails = db.PassbookDetails.Include(p => p.PassBook);
            return View(passbookDetails.ToList());
        }

        // GET: Admin/PassbookDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassbookDetail passbookDetail = db.PassbookDetails.Find(id);
            if (passbookDetail == null)
            {
                return HttpNotFound();
            }
            return View(passbookDetail);
        }

        // GET: Admin/PassbookDetails/Create
        public ActionResult Create()
        {
            ViewBag.PassbookId = new SelectList(db.PassBooks, "Id", "Acronym");
            return View();
        }

        // POST: Admin/PassbookDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PassbookId,Action,ActionDate,Balance,Amount,Surplus,Status")] PassbookDetail passbookDetail)
        {
            if (ModelState.IsValid)
            {
                db.PassbookDetails.Add(passbookDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PassbookId = new SelectList(db.PassBooks, "Id", "Acronym", passbookDetail.PassbookId);
            return View(passbookDetail);
        }

        // GET: Admin/PassbookDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassbookDetail passbookDetail = db.PassbookDetails.Find(id);
            if (passbookDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.PassbookId = new SelectList(db.PassBooks, "Id", "Acronym", passbookDetail.PassbookId);
            return View(passbookDetail);
        }

        // POST: Admin/PassbookDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PassbookId,Action,ActionDate,Balance,Amount,Surplus,Status")] PassbookDetail passbookDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passbookDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PassbookId = new SelectList(db.PassBooks, "Id", "Acronym", passbookDetail.PassbookId);
            return View(passbookDetail);
        }

        // GET: Admin/PassbookDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassbookDetail passbookDetail = db.PassbookDetails.Find(id);
            if (passbookDetail == null)
            {
                return HttpNotFound();
            }
            return View(passbookDetail);
        }

        // POST: Admin/PassbookDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PassbookDetail passbookDetail = db.PassbookDetails.Find(id);
            db.PassbookDetails.Remove(passbookDetail);
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
