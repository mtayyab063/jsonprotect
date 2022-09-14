using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication16.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string companyCode = "73")
        {
            var data = new { date = Helper.GetCurrentUnixTimestampSeconds(), companyCode = companyCode };
            var json = JsonConvert.SerializeObject(data);
            var hash = DataProtector.Protect(json);
            return RedirectToAction("About", new { h = hash });
        }

        public ActionResult About(string h = "")
        {
            //var d = JsonConvert.DeserializeObject(hash);
            ValidateHash(h);
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private bool ValidateHash(string hash)
        {
            var decryptedData = DataProtector.Unprotect(hash); //DecryptHash(hash);
            var json = JsonConvert.DeserializeObject<dynamic>(decryptedData);
            var date = Helper.ConvertToUnixTimestampSecondsToDateTime(Convert.ToDouble(json.date));
            if ((DateTime.Now - date).TotalHours > 1)
            {
                return false;
            }
            return true;
        }
        private static DateTime ConvertFromUnixTimestampSeconds(long unixTimeStamp)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            var actualTime = dt.AddSeconds(unixTimeStamp).ToLocalTime();
            return actualTime;
        }
    }
}