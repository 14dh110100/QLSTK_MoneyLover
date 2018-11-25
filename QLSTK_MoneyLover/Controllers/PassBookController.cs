using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLSTK_MoneyLover.Filters;
using QLSTK_MoneyLover.Models;

namespace QLSTK_MoneyLover.Controllers
{
    public class PassBookController : Controller
    {
        private DbMoneyLoverEntities db = new DbMoneyLoverEntities();
        // GET: PassBook
        [CheckUser]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Bank()
        {
            decimal total = 0;
            int count = 0;
            if (Session["userid"] != null)
            {
                int userid = int.Parse(Session["userid"].ToString());
                AutoCheck(userid);
                var userpb = db.PassBooks.Where(n => n.CustomerId == userid && n.Status == 1);
                count = userpb.Count();
                foreach (var item in userpb)
                {
                    total += item.Principal;
                }
            }
            ViewBag.Total = total.ToString("#,##0") + " (" + count + " sổ)";
            var banks = db.Banks;
            return View(banks.ToList());
        }

        public ActionResult PassBookList(int id)
        {
            var passBooks = db.PassBooks;
            ViewBag.BankAcronym = "Passbook";
            ViewBag.BankTotal = 0;
            if (Session["userid"] != null)
            {
                int userid = int.Parse(Session["userid"].ToString());
                if (id != 0)
                {
                    var pbuser = passBooks.Where(n => n.BankId == id && n.CustomerId == userid && n.Status == 1);
                    ViewBag.BankAcronym = db.Banks.Find(id).Acronym;
                    decimal total = 0;
                    foreach (var item in pbuser)
                    {
                        total += item.Principal;
                    }
                    ViewBag.BankTotal = total.ToString("#,##0") + " đ";
                    return View(pbuser.ToList());
                }
                else if (id == 0)
                {
                    var pbclosed = db.PassBooks.Where(n => n.CustomerId == userid && n.Status == 2);
                    ViewBag.BankAcronym = "Đã tất toán";
                    ViewBag.BankTotal = pbclosed.Count() + " sổ";
                    return View(pbclosed.ToList());
                }
            }
            return View(passBooks.ToList());
        }
        
        public dynamic BuildList()
        {
            List<SelectListItem> iplist = new List<SelectListItem>();
            iplist.Add(new SelectListItem { Text = "Cuối kỳ", Value = "1", Selected = true });
            iplist.Add(new SelectListItem { Text = "Đầu kỳ", Value = "2" });
            iplist.Add(new SelectListItem { Text = "Định kỳ hàng tháng", Value = "3" });

            List<SelectListItem> termendlist = new List<SelectListItem>();
            termendlist.Add(new SelectListItem { Text = "Tái tục gốc và lãi", Value = "1", Selected = true });
            termendlist.Add(new SelectListItem { Text = "Tái tục gốc", Value = "2" });
            termendlist.Add(new SelectListItem { Text = "Tất toán sổ", Value = "3" });

            var res = new { iplist, termendlist };
            return res;
        }

        [CheckUser]
        public ActionResult Create()
        {
            var res = BuildList();

            ViewBag.UserId = int.Parse(Session["userid"].ToString());
            ViewBag.IpList = res.iplist;
            ViewBag.TermEndList = res.termendlist;
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym");
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym");
            return View();
        }
        
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,BankId,TermId,CustomerId,Acronym,Balance,InterestRate,DemandInterestRate,OpenDate,InterestPayment,TermEnd,Status,Principal,ChangeDate")] PassBook passBook, string acrbank)
        {
            string msg = null, msgbankid = null, msgtermid = null, msgir = null, msgdemandir = null, msgacronym = null, msgprincipal = null, msgopendate = null;
            DateTime mindate = new DateTime(2000, 01, 01);

            if (acrbank.Length < 2)
            {
                msgbankid = "Nhập tên ngân hàng viết tắt từ 2 đến 20 ký tự";
            }
            else
            {
                var bank = db.Banks.FirstOrDefault(n => n.Acronym == acrbank);
                if (bank != null)
                {
                    passBook.BankId = bank.Id;
                }
                else
                {
                    Bank newbank = new Bank();
                    newbank.Acronym = acrbank;
                    newbank.Name = acrbank;
                    newbank.Status = 1;
                    db.Banks.Add(newbank);
                    passBook.BankId = newbank.Id;
                }
            }
            if (passBook.TermId == 0)
            {
                msgtermid = "Chọn loại kỳ hạn !";
            }
            if (passBook.InterestRate == 0)
            {
                msgir = "Nhập lãi suất !";
            }
            else if (passBook.InterestRate > 100)
            {
                msgir = "Lãi suất không thể lớn hơn 100 %";
            }
            if (passBook.DemandInterestRate == 0)
            {
                passBook.DemandInterestRate = 0.05;
                if (passBook.TermId != 0)
                {
                    if (passBook.Term.Acronym == "KKH")
                    {
                        passBook.DemandInterestRate = passBook.InterestRate;
                    }
                }
            }
            else if (passBook.DemandInterestRate > 100)
            {
                msgdemandir = "Lãi suất không thể lớn hơn 100 %";
            }
            if (String.IsNullOrEmpty(passBook.Acronym))
            {
                msgacronym = "Nhập mã sổ !";
            }
            else
            {
                int userid = int.Parse(Session["userid"].ToString());
                int pbcount = db.PassBooks.Where(n => n.Acronym == passBook.Acronym && n.Status == 1 && n.CustomerId == userid).Count();
                if (pbcount > 0)
                {
                    msgacronym = "Sổ này đang mở !";
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

            if (msgbankid == null && msgtermid == null && msgir == null && msgdemandir == null && msgacronym == null && msgprincipal == null && msgopendate == null)
            {
                msg = "completed";
                passBook.Balance = passBook.Principal;
                passBook.ChangeDate = passBook.OpenDate;
                passBook.Status = 1;
                db.PassBooks.Add(passBook);
                db.SaveChanges();
                int pbid = 0;
                pbid = passBook.Id;
                return Json(new { pbid, msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msgbankid, msgtermid, msgir, msgdemandir, msgacronym, msgprincipal, msgopendate }, JsonRequestBehavior.AllowGet);
        }

        [CheckUser]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassBook passBook = db.PassBooks.Find(id);
            int userid = 0;
            if (Session["userid"] != null)
            {
                userid = int.Parse(Session["userid"].ToString());
            }
            if (passBook == null || passBook.CustomerId != userid)
            {
                return HttpNotFound();
            }

            var res = BuildList();

            ViewBag.IpList = res.iplist;
            ViewBag.TermEndList = res.termendlist;
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Acronym", passBook.BankId);
            ViewBag.TermId = new SelectList(db.Terms, "Id", "Acronym", passBook.TermId);
            return View(passBook);
        }
        
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,BankId,TermId,CustomerId,Acronym,Balance,InterestRate,DemandInterestRate,OpenDate,InterestPayment,TermEnd,Status,Principal,ChangeDate")] PassBook passBook, string acrbank)
        {
            string msg = null, msgbankid = null, msgtermid = null, msgir = null, msgdemandir = null, msgacronym = null, msgprincipal = null, msgopendate = null, msgchangedate = null;
            DateTime mindate = new DateTime(2000, 01, 01);

            if (acrbank.Length < 2)
            {
                msgbankid = "Nhập tên ngân hàng viết tắt từ 2 đến 20 ký tự";
            }
            else
            {
                var bank = db.Banks.FirstOrDefault(n => n.Acronym == acrbank);
                if (bank != null)
                {
                    passBook.BankId = bank.Id;
                }
                else
                {
                    Bank newbank = new Bank();
                    newbank.Acronym = acrbank;
                    newbank.Name = acrbank;
                    newbank.Status = 1;
                    db.Banks.Add(newbank);
                    passBook.BankId = newbank.Id;
                }
            }
            if (passBook.TermId == 0)
            {
                msgtermid = "Chọn loại kỳ hạn !";
            }
            if (passBook.InterestRate == 0)
            {
                msgir = "Nhập lãi suất !";
            }
            else if (passBook.InterestRate > 100)
            {
                msgir = "Lãi suất không thể lớn hơn 100 %";
            }
            if (passBook.DemandInterestRate == 0)
            {
                passBook.DemandInterestRate = 0.05;
                if (passBook.TermId != 0)
                {
                    if (passBook.Term.Acronym == "KKH")
                    {
                        passBook.DemandInterestRate = passBook.InterestRate;
                    }
                }
            }
            else if (passBook.DemandInterestRate > 100)
            {
                msgdemandir = "Lãi suất không thể lớn hơn 100 %";
            }
            if (String.IsNullOrEmpty(passBook.Acronym))
            {
                msgacronym = "Nhập mã sổ !";
            }
            else
            {
                int userid = int.Parse(Session["userid"].ToString());
                var oldPb = db.PassBooks.Where(n => n.Id == passBook.Id);
                var pbexc = db.PassBooks.Except(oldPb);
                int pbcount = pbexc.Where(n => n.Acronym == passBook.Acronym && n.Status == 1 && n.CustomerId == userid).Count();
                if (pbcount > 0)
                {
                    msgacronym = "Sổ này đang mở !";
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
            if (passBook.ChangeDate < mindate)
            {
                msgchangedate = "Ngày bắt đầu tính lãi không thể nhỏ hơn 01/01/2000 !";
            }
            else if (passBook.ChangeDate > DateTime.Now)
            {
                msgchangedate = "Ngày bắt đầu tính lãi không thể lớn hơn ngày hiện tại !";
            }
            else if (passBook.ChangeDate < passBook.OpenDate)
            {
                msgchangedate = "Ngày bắt đầu tính lãi không thể nhỏ hơn ngày gửi !";
            }

            if (msgbankid == null && msgtermid == null && msgir == null && msgdemandir == null && msgacronym == null && msgprincipal == null && msgopendate == null && msgchangedate == null)
            {
                msg = "completed";
                passBook.Balance = passBook.Principal;
                passBook.Status = 1;
                db.Entry(passBook).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msgbankid, msgtermid, msgir, msgdemandir, msgacronym, msgprincipal, msgopendate, msgchangedate }, JsonRequestBehavior.AllowGet);
        }

        [CheckUser]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassBook passBook = db.PassBooks.Find(id);
            int userid = 0;
            if (Session["userid"] != null)
            {
                userid = int.Parse(Session["userid"].ToString());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            if (passBook == null || passBook.CustomerId != userid)
            {
                return HttpNotFound();
            }
            var res = BuildList();
            string status = null;
            switch (passBook.Status)
            {
                case 1:
                    status = "Đang mở";
                    break;
                case 2:
                    status = "Đã đóng";
                    break;
                default:
                    status = "Chưa xác định";
                    break;
            }
            var ir = FindIR(passBook.BankId, passBook.TermId);

            ViewBag.Status = status;
            ViewBag.IRP = res.iplist[passBook.InterestPayment - 1].Text;
            ViewBag.TermEnd = res.termendlist[(passBook.TermEnd ?? 1) - 1].Text;
            ViewBag.Rate = ir.rate;
            ViewBag.DemandRate = ir.demandrate;
            return View(passBook);
        }

        public ActionResult PbDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pbDetails = db.PassbookDetails.Where(n => n.PassbookId == id);
            decimal deptotal = 0, wdtotal = 0;
            foreach (var item in pbDetails)
            {
                if (item.Status == 1)
                {
                    deptotal += item.Amount;
                }
                if (item.Status == 2)
                {
                    wdtotal += item.Amount;
                }
            }
            decimal principal = db.PassBooks.Find(id).Principal;
            ViewBag.DepTotal = deptotal.ToString("#,##0");
            ViewBag.WdTotal = wdtotal.ToString("#,##0");
            ViewBag.Total = (deptotal + wdtotal).ToString("#,##0");
            ViewBag.Principal = principal.ToString("#,##0");
            ViewBag.Balance = (principal + deptotal + wdtotal).ToString("#,##0");
            return View(pbDetails.ToList());
        }

        [CheckUser]
        public ActionResult Deposit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassBook passBook = db.PassBooks.Find(id);
            int userid = 0;
            if (Session["userid"] != null)
            {
                userid = int.Parse(Session["userid"].ToString());
            }
            if (passBook == null || passBook.CustomerId != userid)
            {
                return HttpNotFound();
            }

            int checkDays = (DateTime.Now - passBook.ChangeDate).Days;
            ViewBag.Active = "false";
            if (checkDays >= passBook.Term.MinDate || checkDays == 0) {
                ViewBag.Active = "true";
            }
            Session["passbookid"] = id;
            ViewBag.Acronym = passBook.Acronym;
            return View();
        }

        [HttpPost]
        public ActionResult Deposit([Bind(Include = "Id,PassBookId,Acronym,DepositDate,Amount,Status")] Deposit deposit)
        {
            deposit.PassBookId = int.Parse(Session["passbookid"].ToString());
            deposit.Acronym = "D_TESTING";
            deposit.DepositDate = DateTime.Now;
            deposit.Status = 1;
            string msgamount = null, msg = null;

            if (deposit.Amount == 0)
            {
                msgamount = "Nhập số tiền gửi !";
            }
            else if (deposit.Amount < 100000)
            {
                //Error message
                msgamount = "Số tiền gửi không thể nhỏ hơn 100.000 !";
            }
            else if (deposit.Amount % 50000 > 0)
            {
                msgamount = "Đơn vị tiền nhỏ nhất là 50.000 !";
            }
            else
            {
                msg = "completed";
                db.Deposits.Add(deposit);

                PassBook passBook = db.PassBooks.SingleOrDefault(n => n.Id == deposit.PassBookId);
                decimal interest = 0;
                int checkDays = (DateTime.Now - passBook.ChangeDate).Days;

                if (checkDays == passBook.Term.MinDate && passBook.TermEnd == 1)
                {
                    interest = decimal.Parse(Calculate(deposit.PassBookId, "deposit").interest.ToString("F"));

                    PassbookDetail pbDetail2 = new PassbookDetail();
                    pbDetail2.PassbookId = passBook.Id;
                    pbDetail2.ActionDate = passBook.ChangeDate.AddDays(passBook.Term.MinDate);
                    pbDetail2.Action = "Lãi kỳ hạn";
                    pbDetail2.Balance = passBook.Balance;
                    pbDetail2.Amount = interest;
                    pbDetail2.Surplus = pbDetail2.Balance + interest;
                    pbDetail2.Status = 1;
                    db.PassbookDetails.Add(pbDetail2);

                    passBook.ChangeDate = passBook.ChangeDate.AddDays(passBook.Term.MinDate);
                }

                PassbookDetail pbDetail = new PassbookDetail();
                pbDetail.PassbookId = passBook.Id;
                pbDetail.ActionDate = DateTime.Now;
                pbDetail.Action = "Gửi thêm";
                pbDetail.Balance = passBook.Balance + interest;
                pbDetail.Amount = deposit.Amount;
                pbDetail.Surplus = pbDetail.Balance + deposit.Amount;
                pbDetail.Status = 1;
                db.PassbookDetails.Add(pbDetail);

                passBook.Balance += (interest + deposit.Amount);
                db.SaveChanges();
                return Json(new { msg }, JsonRequestBehavior.AllowGet);
            }
            
            return Json(new { msgamount }, JsonRequestBehavior.AllowGet);
        }

        [CheckUser]
        public ActionResult Withdraw(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassBook passBook = db.PassBooks.Find(id);
            int userid = 0;
            if (Session["userid"] != null)
            {
                userid = int.Parse(Session["userid"].ToString());
            }
            if (passBook == null || passBook.CustomerId != userid)
            {
                return HttpNotFound();
            }
            
            int checkDays = (DateTime.Now - passBook.ChangeDate).Days;
            if (checkDays < 15)
            {
                //Error message
            }
            ViewBag.PbId = id;
            ViewBag.Acronym = passBook.Acronym;
            ViewBag.PassBookId = new SelectList(db.PassBooks, "Id", "Acronym");
            return View();
        }

        [HttpPost]
        public ActionResult Withdraw([Bind(Include = "Id,PassBookId,Acronym,WithdrawDate,Amount,Status")] Withdraw withdraw)
        {
            withdraw.Acronym = "W_TESTING";
            withdraw.WithdrawDate = DateTime.Now;
            withdraw.Status = 1;

            string msgamount = null, msg = null;
            PassBook passBook = db.PassBooks.SingleOrDefault(n => n.Id == withdraw.PassBookId);
            int checkDays = (DateTime.Now - passBook.ChangeDate).Days;

            if (withdraw.Amount == 0)
            {
                msgamount = "Nhập số tiền rút !";
            }
            else if (withdraw.Amount > passBook.Balance)
            {
                msgamount = "Số tiền rút không thể lớn hơn số dư hiện tại !";
            }
            else if (withdraw.Amount < 100000)
            {
                //Error message
                msgamount = "Số tiền rút không thể nhỏ hơn 100.000 !";
            }
            else if (withdraw.Amount % 50000 > 0)
            {
                msgamount = "Đơn vị tiền nhỏ nhất là 50.000 !";
            }
            else
            {
                msg = "completed";
                db.Withdraws.Add(withdraw);

                decimal interest = 0;

                if (checkDays == passBook.Term.MinDate && passBook.TermEnd == 1)
                {
                    interest = decimal.Parse(Calculate(withdraw.PassBookId, "withdraw").interest.ToString("F"));

                    PassbookDetail pbDetail2 = new PassbookDetail();
                    pbDetail2.PassbookId = passBook.Id;
                    pbDetail2.ActionDate = passBook.ChangeDate.AddDays(passBook.Term.MinDate);
                    pbDetail2.Action = "Lãi kỳ hạn";
                    pbDetail2.Balance = passBook.Balance;
                    pbDetail2.Amount = interest;
                    pbDetail2.Surplus = pbDetail2.Balance + interest;
                    pbDetail2.Status = 1;
                    db.PassbookDetails.Add(pbDetail2);

                    passBook.ChangeDate = passBook.ChangeDate.AddDays(passBook.Term.MinDate);
                }

                PassbookDetail pbDetail = new PassbookDetail();
                pbDetail.PassbookId = passBook.Id;
                pbDetail.ActionDate = DateTime.Now;
                pbDetail.Action = "Rút một phần";
                pbDetail.Balance = passBook.Balance + interest;
                pbDetail.Amount = - withdraw.Amount;
                pbDetail.Surplus = pbDetail.Balance - withdraw.Amount;
                pbDetail.Status = 2;
                db.PassbookDetails.Add(pbDetail);
                
                decimal demandinterest = decimal.Parse((((double)withdraw.Amount * (passBook.DemandInterestRate / 100) / 360) * checkDays).ToString("F"));

                PassbookDetail pbDetail3 = new PassbookDetail();
                pbDetail3.PassbookId = passBook.Id;
                pbDetail3.ActionDate = DateTime.Now;
                pbDetail3.Action = "Lãi khi rút";
                pbDetail3.Balance = passBook.Balance + interest - withdraw.Amount;
                pbDetail3.Amount = demandinterest;
                pbDetail3.Surplus = pbDetail3.Balance + demandinterest;
                pbDetail3.Status = 2;
                db.PassbookDetails.Add(pbDetail3);

                passBook.Balance += (interest - withdraw.Amount + demandinterest);
                db.SaveChanges();
                return Json(new { msg }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { msgamount }, JsonRequestBehavior.AllowGet);
        }

        [CheckUser]
        public ActionResult ConfirmWithdraw(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassBook passBook = db.PassBooks.Find(id);
            int userid = 0;
            if (Session["userid"] != null)
            {
                userid = int.Parse(Session["userid"].ToString());
            }
            if (passBook == null || passBook.CustomerId != userid)
            {
                return HttpNotFound();
            }
            
            ViewBag.PassbookId = id;
            ViewBag.Acronym = passBook.Acronym;
            ViewBag.TermendDate = passBook.ChangeDate.AddDays(passBook.Term.MinDate);
            ViewBag.DemandRate = passBook.DemandInterestRate;

            return View();
        }

        [CheckUser]
        public ActionResult Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassBook passBook = db.PassBooks.Find(id);
            int userid = 0;
            if (Session["userid"] != null)
            {
                userid = int.Parse(Session["userid"].ToString());
            }
            if (passBook == null || passBook.CustomerId != userid)
            {
                return HttpNotFound();
            }
            ViewBag.PbId = id;
            
            var res = Calculate(passBook.Id, "close");

            ViewBag.Acronym = passBook.Acronym;
            ViewBag.Principal = passBook.Principal;
            ViewBag.Balance = passBook.Balance;
            ViewBag.Interest = res.interest;
            ViewBag.InterestKKH = res.interestKKH;
            ViewBag.Total = passBook.Balance + (decimal)res.interest + (decimal)res.interestKKH;
            return View();
        }

        [HttpPost]
        public ActionResult Close([Bind(Include = "Id,PassBookId,Acronym,WithdrawDate,Amount,Status")] Withdraw withdraw)
        {
            withdraw.Acronym = "C_TESTING";
            withdraw.WithdrawDate = DateTime.Now;
            withdraw.Status = 1;

            string msg = null;
            PassBook passBook = db.PassBooks.SingleOrDefault(n => n.Id == withdraw.PassBookId);
            if (ModelState.IsValid)
            {
                int checkDays = (DateTime.Now - passBook.ChangeDate).Days;
                //Tính lãi suất
                var res = Calculate(withdraw.PassBookId, "close");

                decimal interest = 0, demandinterest = 0;
                interest = decimal.Parse(res.interest.ToString("F"));
                demandinterest = decimal.Parse(res.interestKKH.ToString("F"));

                if (checkDays == passBook.Term.MinDate)
                {
                    PassbookDetail pbDetail2 = new PassbookDetail();
                    pbDetail2.PassbookId = passBook.Id;
                    pbDetail2.ActionDate = passBook.ChangeDate.AddDays(passBook.Term.MinDate);
                    pbDetail2.Action = "Lãi kỳ hạn";
                    pbDetail2.Balance = passBook.Balance;
                    pbDetail2.Amount = interest;
                    pbDetail2.Surplus = pbDetail2.Balance + interest;
                    pbDetail2.Status = 1;
                    db.PassbookDetails.Add(pbDetail2);
                }
                else if (checkDays < passBook.Term.MinDate)
                {
                    PassbookDetail pbDetail3 = new PassbookDetail();
                    pbDetail3.PassbookId = passBook.Id;
                    pbDetail3.ActionDate = DateTime.Now;
                    pbDetail3.Action = "Lãi KKH";
                    pbDetail3.Balance = passBook.Balance;
                    pbDetail3.Amount = demandinterest;
                    pbDetail3.Surplus = pbDetail3.Balance + demandinterest;
                    pbDetail3.Status = 1;
                    db.PassbookDetails.Add(pbDetail3);
                }
                
                //Add PassbookDetail
                PassbookDetail pbDetail = new PassbookDetail();
                pbDetail.PassbookId = passBook.Id;
                pbDetail.ActionDate = DateTime.Now;
                pbDetail.Action = "Tất toán";
                pbDetail.Balance = passBook.Balance + interest + demandinterest;
                pbDetail.Amount = - pbDetail.Balance;
                pbDetail.Surplus = 0;
                pbDetail.Status = 2;
                db.PassbookDetails.Add(pbDetail);
                
                withdraw.Amount = pbDetail.Balance;

                //Edit Passbook details
                passBook.Balance = 0;
                passBook.ChangeDate = DateTime.Now;
                passBook.TermEnd = 3;
                passBook.Status = 2;

                msg = "completed";

                db.Withdraws.Add(withdraw);
                db.SaveChanges();
                return Json(new { msg }, JsonRequestBehavior.AllowGet);
            }
            
            return Json(new { msg }, JsonRequestBehavior.AllowGet);
        }
        
        private dynamic Calculate(int id, string action) {
            PassBook passBook = db.PassBooks.SingleOrDefault(n => n.Id == id);
            double rate = passBook.InterestRate, demandrate = passBook.DemandInterestRate;
            double interest = 0, interestKKH = 0;
            int checkDays = (DateTime.Now - passBook.ChangeDate).Days;
            if (checkDays < passBook.Term.MinDate)
            {
                //Tính lãi suất không kỳ hạn
                interestKKH = ((double)passBook.Balance * (demandrate/100) / 360) * checkDays;
            }
            else if (checkDays == passBook.Term.MinDate)
            {
                //Tính lãi suất đúng kỳ hạn
                interest = ((double)passBook.Balance * (rate/100) / 360) * passBook.Term.MinDate;
            }
            else {
                //Tính lãi suất đúng kỳ hạn + lãi suất không kỳ hạn
                int checkTerms = checkDays / passBook.Term.MinDate;
                double currentBal = (double)passBook.Balance;
                DateTime changeDate = passBook.ChangeDate;
                //Vòng lặp tính lãi suất mỗi lần đúng kỳ hạn
                for (int i = 0; i < checkTerms; i++)
                {
                    interest = (currentBal * (rate / 100) / 360) * passBook.Term.MinDate;
                    changeDate = changeDate.AddDays(passBook.Term.MinDate);

                    if (action == "AutoTermEnd") {
                        PassbookDetail pbDetail = new PassbookDetail();
                        pbDetail.PassbookId = id;
                        pbDetail.Action = "Lãi kỳ hạn";
                        pbDetail.ActionDate = changeDate;
                        pbDetail.Balance = decimal.Parse(currentBal.ToString("F"));
                        pbDetail.Amount = decimal.Parse(interest.ToString("F"));
                        pbDetail.Surplus = decimal.Parse((currentBal + interest).ToString("F"));
                        pbDetail.Status = 1;

                        db.PassbookDetails.Add(pbDetail);
                    }
                    
                    currentBal += interest;
                }
                passBook.ChangeDate = changeDate;
                passBook.Balance = decimal.Parse(currentBal.ToString("F"));
                passBook.TermEnd = 1;
                interestKKH = (currentBal * (demandrate/100) / 360) * (checkDays - (passBook.Term.MinDate * checkTerms));
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

        private ActionResult AutoCheck(int id)
        {
            var passBooks = db.PassBooks.Where(n => n.CustomerId == id && n.Status == 1);
            foreach (var item in passBooks) {
                int checkDays = (DateTime.Now - item.ChangeDate).Days;
                if (checkDays > item.Term.MinDate)
                {
                    Calculate(item.Id, "AutoTermEnd");
                }
            }
            db.SaveChanges();
            return null;
        }

        public JsonResult CheckIR(string bankid, string termid)
        {
            int a = 0, b = 0;
            double rate = 0, demandrate = 0;
            if (!String.IsNullOrEmpty(bankid))
            {
                var bank = db.Banks.FirstOrDefault(n => n.Acronym == bankid);
                if (bank != null)
                {
                    a = bank.Id;
                }
            }
            if (!String.IsNullOrEmpty(termid))
            {
                b = int.Parse(termid);
            }
            var ir = FindIR(a, b);
            rate = ir.rate;
            demandrate = ir.demandrate;
            return Json(new { rate, demandrate }, JsonRequestBehavior.AllowGet);
        }

        private dynamic FindIR(int bankid, int termid)
        {
            double rate = 0, demandrate = 0;
            if (bankid != 0)
            {
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
            }
            var res = new { rate, demandrate };
            return res;
        }
    }
}