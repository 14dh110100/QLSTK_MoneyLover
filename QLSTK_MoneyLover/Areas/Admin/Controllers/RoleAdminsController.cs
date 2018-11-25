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
    public class RoleAdminsController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();

        // GET: Admin/RoleAdmins
        public ActionResult Index()
        {
            return View(db.RoleAdmins.ToList());
        }

        // GET: Admin/RoleAdmins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleAdmin roleAdmin = db.RoleAdmins.Find(id);
            if (roleAdmin == null)
            {
                return HttpNotFound();
            }
            return View(roleAdmin);
        }

        // GET: Admin/RoleAdmins/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/RoleAdmins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Acronym,Name,Status")] RoleAdmin roleAdmin)
        {
            if (ModelState.IsValid)
            {
                db.RoleAdmins.Add(roleAdmin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roleAdmin);
        }

        // GET: Admin/RoleAdmins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleAdmin roleAdmin = db.RoleAdmins.Find(id);
            if (roleAdmin == null)
            {
                return HttpNotFound();
            }
            return View(roleAdmin);
        }

        // POST: Admin/RoleAdmins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Acronym,Name,Status")] RoleAdmin roleAdmin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roleAdmin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roleAdmin);
        }

        // GET: Admin/RoleAdmins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleAdmin roleAdmin = db.RoleAdmins.Find(id);
            if (roleAdmin == null)
            {
                return HttpNotFound();
            }
            return View(roleAdmin);
        }

        // POST: Admin/RoleAdmins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoleAdmin roleAdmin = db.RoleAdmins.Find(id);
            db.RoleAdmins.Remove(roleAdmin);
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
