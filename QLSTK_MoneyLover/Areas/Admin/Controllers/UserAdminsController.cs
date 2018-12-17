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
    public class UserAdminsController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();

        // GET: Admin/UserAdmins
        public ActionResult Index()
        {
            //var userAdmins = db.UserAdmins.Include(u => u.RoleAdmin);
            var userAdmins = db.UserAdmins.Where(n => n.Status != 0);
            return View(userAdmins.ToList());
        }

        // GET: Admin/UserAdmins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAdmin userAdmin = db.UserAdmins.Find(id);
            if (userAdmin == null)
            {
                return HttpNotFound();
            }
            return View(userAdmin);
        }

        // GET: Admin/UserAdmins/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.RoleAdmins, "Id", "Acronym");
            return View();
        }

        // POST: Admin/UserAdmins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RoleId,Acronym,Name,UserName,Password,Encrypted,Status")] UserAdmin userAdmin)
        {
            if (ModelState.IsValid)
            {
                db.UserAdmins.Add(userAdmin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.RoleAdmins, "Id", "Acronym", userAdmin.RoleId);
            return View(userAdmin);
        }

        // GET: Admin/UserAdmins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAdmin userAdmin = db.UserAdmins.Find(id);
            if (userAdmin == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.RoleAdmins, "Id", "Acronym", userAdmin.RoleId);
            return View(userAdmin);
        }

        // POST: Admin/UserAdmins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RoleId,Acronym,Name,UserName,Password,Encrypted,Status")] UserAdmin userAdmin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userAdmin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.RoleAdmins, "Id", "Acronym", userAdmin.RoleId);
            return View(userAdmin);
        }

        // GET: Admin/UserAdmins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAdmin userAdmin = db.UserAdmins.Find(id);
            if (userAdmin == null)
            {
                return HttpNotFound();
            }
            return View(userAdmin);
        }

        // POST: Admin/UserAdmins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserAdmin userAdmin = db.UserAdmins.Find(id);
            userAdmin.Status = 0;
            db.Entry(userAdmin).State = EntityState.Modified;
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
