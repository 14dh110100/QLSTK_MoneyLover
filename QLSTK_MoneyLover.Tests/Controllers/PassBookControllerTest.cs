using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using QLSTK_MoneyLover.Controllers;
using QLSTK_MoneyLover.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QLSTK_MoneyLover.Tests
{
    [TestClass()]
    public class PassBookControllerTest
    {
        PassBookController pbcontroller = new PassBookController();
        private DbMoneyLoverEntities dbtest = new DbMoneyLoverEntities();

        [TestMethod()]
        public void WithdrawTests()
        {
            int pbid = 0;
            PassBook pbtest = dbtest.PassBooks.FirstOrDefault(n => n.Balance > 100005 && n.Status == 1);
            if (pbtest != null)
            {
                pbid = pbtest.Id;

                Withdraw withdraw1 = new Withdraw();
                withdraw1.PassBookId = pbid;
                withdraw1.Amount = 0;
                string err1 = "Nhập số tiền rút !";
                JsonResult result1 = pbcontroller.Withdraw(withdraw1) as JsonResult;
                string msg1 = GetLogMsg(result1).msg;
                Assert.AreEqual(err1, msg1);
                System.Diagnostics.Debug.WriteLine("Case 1: " + msg1);

                Withdraw withdraw2 = new Withdraw();
                withdraw2.PassBookId = pbid;
                withdraw2.Amount = dbtest.PassBooks.Find(withdraw2.PassBookId).Balance + 1;
                string err2 = "Số tiền rút không thể lớn hơn số dư hiện tại !";
                JsonResult result2 = pbcontroller.Withdraw(withdraw2) as JsonResult;
                string msg2 = GetLogMsg(result2).msg;
                Assert.AreEqual(err2, msg2);
                System.Diagnostics.Debug.WriteLine("Case 2: " + msg2);

                Withdraw withdraw3 = new Withdraw();
                withdraw3.PassBookId = pbid;
                withdraw3.Amount = 10000;
                string err3 = "Số tiền rút không thể nhỏ hơn 100.000 !";
                JsonResult result3 = pbcontroller.Withdraw(withdraw3) as JsonResult;
                string msg3 = GetLogMsg(result3).msg;
                Assert.AreEqual(err3, msg3);
                System.Diagnostics.Debug.WriteLine("Case 3: " + msg3);

                Withdraw withdraw4 = new Withdraw();
                withdraw4.PassBookId = pbid;
                withdraw4.Amount = 100005;
                string err4 = "Đơn vị tiền nhỏ nhất là 50.000 !";
                JsonResult result4 = pbcontroller.Withdraw(withdraw4) as JsonResult;
                string msg4 = GetLogMsg(result4).msg;
                Assert.AreEqual(err4, msg4);
                System.Diagnostics.Debug.WriteLine("Case 4: " + msg4);

                Withdraw withdraw5 = new Withdraw();
                withdraw5.PassBookId = pbid;
                withdraw5.Amount = 100000;
                string err5 = "completed";
                JsonResult result5 = pbcontroller.Withdraw(withdraw5) as JsonResult;
                string msg5 = GetLogMsg(result5).msg;
                Assert.AreEqual(err5, msg5);
                System.Diagnostics.Debug.WriteLine("Case 5: " + msg5);
            }
            else
            {
                Assert.IsNull(pbtest);
            }
        }

        public dynamic GetLogMsg(JsonResult json)
        {
            string replace = json.Data.ToString().Replace(" = ", "\":\"");
            replace = replace.Replace("{ ", "{\"");
            replace = replace.Replace(" }", "\"}");
            JObject jObject = JObject.Parse(replace);
            string msg = (string)jObject.SelectToken("msgamount");
            if (msg == null)
            {
                msg = (string)jObject.SelectToken("msg");
            }
            var res = new { msg };
            return res;
        }
    }
}