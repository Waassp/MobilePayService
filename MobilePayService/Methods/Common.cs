using System;
using System.Net;
using System.Text;

namespace MobilePayService.Methods
{
    public class Common
    {
        public static HttpWebRequest CreateWebRequest(String SoapUrl, string Cred, string soapAction,bool auth2=false)
        {
            string base64Cred = Base64Encode(Cred);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(SoapUrl);
            webRequest.Headers.Add(@"SOAPAction: " + soapAction);
            if(auth2)
                webRequest.Headers.Add(HttpRequestHeader.Authorization,"Basic " + base64Cred);
            else
                webRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer  " + base64Cred);

            webRequest.ContentType = "text/xml;charset=\"UTF-8\"";

            webRequest.Method = "POST";
            return webRequest;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}