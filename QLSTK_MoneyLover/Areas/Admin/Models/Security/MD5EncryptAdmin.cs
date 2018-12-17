using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace QLSTK_MoneyLover.Areas.Admin.Models.Security
{
    public class MD5EncryptAdmin
    {
        public static string ConvertMD5Admin(string text)
        {
            MD5 mD5 = new MD5CryptoServiceProvider();
            mD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = mD5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
    }
}