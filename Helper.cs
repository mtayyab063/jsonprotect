using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WebApplication16
{
    public static class Helper
    {
        public static string ApplicationRoot
        {
            get
            {
                return System.AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static bool IsValidMobile(string mobile)
        {
            mobile = mobile ?? "";
            return mobile.StartsWith("05") && mobile.Length >= 10;
        }     

        public static long GetCurrentUnixTimestampSeconds()
        {
            return ConvertToUnixTimestampSeconds(DateTime.UtcNow);
        }

        public static DateTime ConvertToUnixTimestampSecondsToDateTime(double timeStamp)
        {

            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime ConvertToDateTime(string date)
        {
            DateTime dt;
            if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, new CultureInfo("en-GB"), DateTimeStyles.None, out dt))
            {
                dt = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            }
            return dt;
        }

        public static string GetODate(this string dat)
        {
            if (!string.IsNullOrWhiteSpace(dat))
            {
                var dt = dat.Split('/');
                return $"{dt[2]}-{dt[1]}-{dt[0]}";
            }
            return dat;
        }


        public static string ConvertToUnixTimestampSecondsToDateTimeFormat(double timeStamp)
        {

            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timeStamp).ToLocalTime();
            return dtDateTime.ToShortDateString();
        }
        public static long ConvertToUnixTimestampSeconds(DateTime dateTime)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime - unixEpoch).TotalSeconds;
        }

        public static DateTime ConvertFromUnixTimestampSeconds(long unixTimeStamp)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return dt.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public static double GetNumberFromString(string value)
        {
            string returnVal = "0";
            value = value ?? "";
            MatchCollection collection = Regex.Matches(value, "\\d+");
            foreach (Match m in collection)
            {
                returnVal += m.ToString();
            }
            if (returnVal == "0")
            {
                return 0;
            }
            return Convert.ToDouble(returnVal);
        }

        public static bool IsNumaric(string value)
        {
            int n;
            return int.TryParse(value, out n);
        }

        public static bool IsDate(string value)
        {
            string[] format = new string[] { "dd-MM-yyyy HH:mm:ss" };
            DateTime datetime;

            if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out datetime))
            {
                return true;
            }
            return false;
        }
        public static void CreateFile(string contents, string filePath)
        {
            File.WriteAllText(filePath, contents);
        }
        public static void RunFile(string filePath)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = filePath;
            p.Start();
            var error = p.StandardError.ReadToEnd();
            var output = p.StandardOutput.ReadToEnd();
            //Logger.Log(string.Format("Running file {0}, StandardError: {1}, StandardOutput: {2}", filePath, error, output));
            p.WaitForExit();
        }

        public static string GetApplicationPath()
        {
            var request = HttpContext.Current.Request;
            return request.Url.Scheme + System.Uri.SchemeDelimiter + request.Url.Host + (request.Url.IsDefaultPort ? "" : ":" + request.Url.Port) +
                (request.ApplicationPath == "/" ? "" : request.ApplicationPath);
        }

        public static string GetLocalApplicationPath()
        {
            var request = HttpContext.Current.Request;
            return request.Url.Scheme + System.Uri.SchemeDelimiter + "localhost" + (request.Url.IsDefaultPort ? "" : ":" + request.Url.Port) +
                (request.ApplicationPath == "/" ? "" : request.ApplicationPath);
        }
        public static void DeleteFiles(params string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }
        public static string GetHash(string val)
        {
            return HttpUtility.UrlEncode(DataProtector.Protect(val));
        }

        public static bool ValidateAlphaNumeric(string inputText)
        {
            Regex pattern = new Regex("[^a-zA-Z0-9]");
            return !pattern.IsMatch(inputText);
        }

        public static bool ValidateOnlyArabicCharacters(string inputText)
        {
            Regex pattern = new Regex("[^\\u0600-\\u06ff' ']");
            return !pattern.IsMatch(inputText);
        }

        public static string RemoveSpecialCharactersEng(this string str)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                foreach (var c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ')
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
            return str;
        }

        public static string RemoveSpecialCharactersArb(this string str)
        {
            var charsToRemove = new string[] { "@", ",", ".", ";", "'", "//", "-", "_", "*", "-", "َ", "ً", "ُ", "ٌ", "،", "ْ", "ٌ", "0" };
            if (!string.IsNullOrEmpty(str))
            {
                foreach (var c in charsToRemove)
                {
                    str = str.Replace(c, string.Empty);
                }
                return str;
            }
            return str;
        }
        public static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, "\\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\\Z", RegexOptions.IgnoreCase);
        }
        public static bool IsValidUAEPhoneNumber(string phoneNo)
        {
            return Regex.IsMatch(phoneNo, @"^(?:\+971|00971|0)?(?:50|51|52|55|56|2|3|4|6|7|9)\d{7}$", RegexOptions.IgnoreCase);
        }
        public static bool IsValidUAEShortPhoneNumber(string phoneNo)
        {
            return Regex.IsMatch(phoneNo, @"^(0)(5)[0-9]{8}$", RegexOptions.IgnoreCase);
        }

        public static bool IsNumberOnly(string phoneNo)
        {
            return Regex.IsMatch(phoneNo, @"^[0-9]*$", RegexOptions.IgnoreCase);
        }

        public static T? GetValueOrNull<T>(this string valueAsString) where T : struct
        {
            if (string.IsNullOrEmpty(valueAsString))
            {
                return null;
            }

            return (T)Convert.ChangeType(valueAsString, typeof(T));
        }

        //public static string GetClientIPAddress()
        //{
        //    var context = System.Web.HttpContext.Current;
        //    var remoteAddress = string.Format("R={0}", context.Request.ServerVariables["REMOTE_ADDR"]);
        //    var forwardedFor = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    var cip = context.Request.Headers[Constants.Headers.CLIENT_IP_ADDRESS];
        //    return string.Format("{0};F={1};C={2}", remoteAddress, forwardedFor, cip);
        //    // For security reason keeping both ips // For security reason keeping both ips
        //}

        public static bool IsSkilledJob(string jobCode)
        {
            var code = (jobCode ?? "");
            return code.EndsWith("1") || code.EndsWith("2") || code.EndsWith("3");
        }

        public enum ReceiptSendType
        {
            Receipt = 1,
            Contract = 2
        }
        public enum WorkPermitPaymentType
        {
            WorkPermitPayment = 1,
            DubaiInsaurance = 2
        }
        public enum Emirates
        {
            AbuDhabi = 1,
            Dubai = 2,
            Sharjah = 3,
            Ajman = 4,
            UmAlQaiwain = 5,
            RasAlKhaima = 6,
            Fujairah = 7
        }
        public enum EnumInjuryStatus
        {
            Finalized = 1,
            Rejected = 2,
            AcceptedforUpdate = 3,
            AcceptedforFinalize = 4,
            Employerfinalized = 5,
            Employerupdated = 6,
            Reported = 7
        }
        public enum EnumRowStatus
        {
            Blank = 0,
            Active = 1,
            InActive = 2,
            Delete = 3
        }
        public enum SmartCardType
        {
            OldESign = 1,
            Eida = 2,
            NewESign = 3,
            PROCard = 4
        }
        public enum SmartCardStatus
        {
            NewTransaction = 3,
            ActiveTransaction = 6,
            DeactiveTransaction = 7,
            donotCheckPending = 9,
        }
        public static string NumberToWords(this int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        /// <summary>
        /// This function is used to get column value into desired dataType. Mainly for those sets where column is unknown to the table whether exists or not. 
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="rowNum">Row Number </param>
        /// <param name="dataType">Desired DataType. If not nullable, provide default value otherwise will return 0 / empty string / 01/01/1900 as defaults</param>
        /// <param name="columnName">Column Name in The given data table</param>
        /// <param name="defaultValue">default value to be return in case column value is null or column / row does not exist.</param>
        /// <returns>return column value if exists otherwise a default value</returns>
        public static dynamic GetValueIfExistsInDt(DataTable dt, int rowNum, System.Type dataType, string columnName, object defaultValue)
        {
            //Test @ ~/api/services/onlineCancellation/testConvert/{transnum}
            //TODO: add other datatypes like DateTime
            if (!IsOfNullableType(dataType))
            {
                if (defaultValue == null)
                {
                    if (dataType == typeof(string))
                    {
                        defaultValue = string.Empty;
                    }
                    else if (dataType == typeof(int))
                    {
                        defaultValue = 0;
                    }
                    else if (dataType == typeof(DateTime) && defaultValue != null)
                    {
                        defaultValue = Convert.ToDateTime("01/01/1900");
                    }
                    else if (dataType == typeof(DateTime) && defaultValue == null)
                    {
                        defaultValue = null;
                    }
                }
            }
            if (dt != null && dt.Rows.Count > rowNum && dt.Columns.Contains(columnName))
            {
                if (!DBNull.Value.Equals(dt.Rows[rowNum][columnName]))
                {
                    if (dataType == typeof(string))
                    {
                        defaultValue = Convert.ToString(dt.Rows[rowNum][columnName]);
                    }
                    else if (dataType == typeof(int) || dataType == typeof(int?))
                    {
                        defaultValue = int.TryParse(Convert.ToString(dt.Rows[rowNum][columnName]), out var intValue) ? intValue : defaultValue;
                    }
                    else if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
                    {
                        if (DateTime.TryParseExact(Convert.ToString(dt.Rows[rowNum][columnName]), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var outDate))
                        {
                            defaultValue = outDate;
                        }
                        if (defaultValue == null)
                        {
                            defaultValue = DateTime.TryParse(Convert.ToString(dt.Rows[rowNum][columnName]), out var outDate1) ? outDate1 : defaultValue;
                        }
                    }
                }
            }
            //if ((dt.Rows[rowNum][columnName]).GetType() == typeof(string))datateime
            //{
            //}
            return defaultValue;
        }
        private static bool IsOfNullableType<T>(T o)
        {
            var type = typeof(T);
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static string GetBarCodeImage(string value, int width, int height, int fontSize)
        {
            var img = new System.Drawing.Bitmap(width, height);
            var g = Graphics.FromImage(img);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.FillRectangle(Brushes.White, 0, 0, width, height);
            g.DrawString("*" + value + "*", new Font("Free 3 of 9", fontSize, FontStyle.Regular), new SolidBrush(Color.Black), 2, 10);
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                var base64BarCode = Convert.ToBase64String(ms.ToArray());
                return base64BarCode;
            }
        }

        public static string GetPlaceHolderFromContent(string sectioncontent)
        {
            if (sectioncontent != null)
            {
                if (sectioncontent.Contains("{") && sectioncontent.Contains("}"))
                {
                    var placeHolder = sectioncontent.Substring(sectioncontent.IndexOf('{')).Substring(0, sectioncontent.Substring(sectioncontent.IndexOf('{')).IndexOf('}') + 1);

                    var placeHolderName = placeHolder.Replace("{", "").Replace("}", "");
                    return placeHolderName.ToUpper();
                }
            }
            return "";
        }

     
      

        public static string GetOnlyNumbersFromTheString(string str)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                foreach (var c in str)
                {
                    if ((c >= '0' && c <= '9'))
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
            return str;
        }
    }
}