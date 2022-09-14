using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;

namespace WebApplication16
{
    public static class DataProtector
    {
        public static readonly string CertificateFilePath = HostingEnvironment.MapPath("~/App_Data/Muhammad Tayyab.pfx");

        public static string Protect(string data)
        {
            dynamic bytes = System.Text.Encoding.UTF8.GetBytes(data);
            dynamic b = MachineKey.Protect(bytes, "Hash");
            return Convert.ToBase64String(b);
        }
        public static string Unprotect(string value)
        {
            dynamic bytes = Convert.FromBase64String(value);
            dynamic b = MachineKey.Unprotect(bytes, "Hash");
            return System.Text.Encoding.UTF8.GetString(b);
        }

        /// <summary>
        /// It protects and return url encoded for base 64
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ProtectBase64Url(string data)
        {
            dynamic bytes = System.Text.Encoding.UTF8.GetBytes(data);
            dynamic b = MachineKey.Protect(bytes, "Hash");
            return Base64UrlEncoder.Encode(b);

        }

        /// <summary>
        /// it decode  base 64 for url and unprotect 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UnprotectBase64Url(string value)
        {
            dynamic bytes = Base64UrlEncoder.DecodeBytes(value);
            dynamic b = MachineKey.Unprotect(bytes, "Hash");
            return System.Text.Encoding.UTF8.GetString(b);
        }


        public static string ProtectBase64Url(string data, string purpose)
        {
            dynamic bytes = System.Text.Encoding.UTF8.GetBytes(data);
            dynamic b = MachineKey.Protect(bytes, purpose);
            return Base64UrlEncoder.Encode(b);

        }

        /// <summary>
        /// it decode  base 64 for url and unprotect 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UnprotectBase64Url(string value, string purpose = "")
        {
            dynamic bytes = Base64UrlEncoder.DecodeBytes(value);
            if (purpose == "")
            {
                dynamic b = MachineKey.Unprotect(bytes);
                return System.Text.Encoding.UTF8.GetString(b);
            }
            else
            {
                dynamic b = MachineKey.Unprotect(bytes, purpose);
                return System.Text.Encoding.UTF8.GetString(b);
            }
        }

        public static string ProtectUsingCertificate(string plainText)
        {
            var x509Certificate = new X509Certificate2(CertificateFilePath, "P@ssw0rd.1", X509KeyStorageFlags.Exportable);
            if (x509Certificate != null)
            {
                var publicKey = (RSACryptoServiceProvider)x509Certificate.PublicKey.Key;
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedBytes = publicKey.Encrypt(plainBytes, false);
                var encryptedText = Convert.ToBase64String(encryptedBytes);
                return encryptedText;
            }
            return null;
        }
        public static string UnprotectUsingCertificate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var x509Certificate = new X509Certificate2(CertificateFilePath, "P@ssw0rd.1");
            if (x509Certificate != null)
            {
                var encryptedBytes = ConvertToBase64(value);
                var privateKey = (RSACryptoServiceProvider)x509Certificate.PrivateKey;
                var decryptedBytes = privateKey.Decrypt(encryptedBytes, false);
                var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                return decryptedText;
            }
            return string.Empty;
        }

        public static bool ValidateHash(string hash)
        {
            var decryptedData = UnprotectUsingCertificate(hash); //DecryptHash(hash);
            if (string.IsNullOrEmpty(decryptedData))
            {
                return false;
            }
            var hashData = decryptedData.Split('|');
            if (hashData.Length == 0)
            {
                return false;
            }
            return true;
        }
        private static byte[] ConvertToBase64(string value)
        {
            value = value.Replace('-', '+');
            value = value.Replace('_', '/');
            value = value.Replace(' ', '+');
            var encryptedBytes = Convert.FromBase64String(value);
            return encryptedBytes;
        }

        private static DateTime ConvertFromUnixTimestampSeconds(long unixTimeStamp)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            var actualTime = dt.AddSeconds(unixTimeStamp).ToLocalTime();
            return actualTime;
        }



    }
}