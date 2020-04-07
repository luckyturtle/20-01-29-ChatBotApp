/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using System;
using System.Security.Cryptography;
using System.Text;

namespace Link.Extensions
{
    //this class need to clean
    public static class Code
    {
        public static string TruncateLongString(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static string TimePassed(DateTime date1, DateTime date2)
        {
            TimeSpan different = (date1 - date2);
            if (different.TotalSeconds == 1)
            {
                return string.Format(" {0} second ago", (int)different.TotalSeconds);
            }
            else if (different.TotalSeconds <= 59)
            {
                return string.Format(" {0} seconds ago", (int)different.TotalSeconds);
            }
            else if (different.TotalMinutes == 1)
            {
                return string.Format(" {0} minute ago", (int)different.TotalMinutes);
            }
            else if (different.TotalMinutes <= 59)
            {
                return string.Format(" {0} minutes ago", (int)different.TotalMinutes);
            }
            else if (different.TotalHours == 1)
            {
                return string.Format(" {0} hour ago", (int)different.TotalHours);
            }
            else if (different.TotalHours <= 23)
            {
                return string.Format(" {0} hours ago", (int)different.TotalHours);
            }
            else if (different.TotalDays == 1)
            {
                return string.Format(" {0} Day ago", (int)different.TotalDays);
            }
            else if (different.TotalDays <= 30)
            {
                return string.Format(" {0} Days ago", (int)different.TotalDays);
            }
            else
            {
                return date2.ToShortDateString();
            }

        }

        private static string HexEncode(byte[] aby)
        {
            string hex = "0123456789abcdef";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < aby.Length; i++)
            {
                sb.Append(hex[(aby[i] & 0xf0) >> 4]);
                sb.Append(hex[aby[i] & 0x0f]);
            }
            return sb.ToString();
        }

        public static string Hash(string str)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] aby = utf8.GetBytes(str);


            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] binMD5 = md5.ComputeHash(aby);
                return HexEncode(binMD5);
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateProfileGroupId(string currentUserId, string id)
        {
            var arr = new string[] { currentUserId, id };
            Array.Sort(arr, StringComparer.InvariantCulture);

            return arr[0] + "__" + arr[1];
        }
    }
}
