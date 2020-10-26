using MobilePayService.Methods;
using MobilePayService.Models;
using MobilePayService.RestAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MobilePayService.Controllers
{
    public class MobilePayIndexController : Controller
    {

        private void mythread1()
        {
            BCClientModel bcClient = new BCClientModel();
            Proxy proxy = new Proxy();
            proxy.SimpleListenerExample(bcClient.redirect_uri);
        }

        public string GetBaseUrl()
        {
            var request = HttpContext.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        public ActionResult Index()
        {
            int key = Convert.ToInt32( HttpContext.Request.QueryString["sessionId"]);
            ViewBag.Url = DBManager.GetUrlByKey(key);

            return View("Welcome");
        }

        public ActionResult Redirect()
        {
           // int key = Convert.ToInt32(HttpContext.Request.QueryString["sessionId"]);
           // ViewBag.Url = DBManager.GetUrlByKey(key);

            return View("redirect");
        }

        #region Session-specific utility methods
        private static object Get(string str)
        {
            return System.Web.HttpContext.Current.Session[str];
        }

        private static void Set(string str, object value)
        {
            System.Web.HttpContext.Current.Session[str] = value;
        }
        #endregion



        [System.Web.Mvc.HttpPost]
        public ActionResult MobilePay([FromBody]BCClientModel content)
        {
            int insertedId = 0;
            string baseURL = GetBaseUrl();            

            try
            {
                string returnedURL = GenerateAuthURL(content);
                SessionUrl su = new SessionUrl();
                su.url = returnedURL;
                su.bcTenantId = content.password;
                insertedId =     DBManager.InsertRecordSession(su);
            }
            catch (Exception eexx)
            {
                var message = "Exception in mobilepay method Exception is " + eexx.Message;
                System.Diagnostics.EventLog.WriteEntry(message, eexx.StackTrace,System.Diagnostics.EventLogEntryType.Error);
            }
            string dataToPassWithUrl = baseURL + "/MobilePayIndex" + "/?sessionId=" + insertedId;

            return Content(dataToPassWithUrl);
        }


        [System.Web.Mvc.HttpPost]
        public string GenerateInvoice([FromBody]InvoiceModel invoice)
        {
           
            int id = 0; string str = "Thanks!"; 
            string query = "select ID from MobilePayOnBoarding where BCTenantId = '" + invoice.BCTenantURL + "'";


            try
            {
                DBManager.VerifyTenantID(query, status =>
                {

                    if (status.Equals("S"))
                        id = DBManager.GenerateInvoice(invoice);

                    if (id == 0)
                        str = "ERROR on generating invoice!";
                    if (id == -1)
                        str = "Invoice URL Already Exist !";

                });

            }
            catch (Exception eexx)
            {
                str = "Exception on generate Invoice " + eexx;
            }

            return str;
        }

        public  string PostInvoice(InvoiceModel invoice,string responsebody)
        {
            //InvoiceModel invoice = lstInvoice[0];            
            string str = "Thanks!";
            //try
            //{

            DBManager.VerifyInvoice(invoice, callback =>
            {
                if (callback != null)
                {
                    Proxy proxy = new Proxy();
                    if (callback.enableCallback.Equals("true"))
                        proxy.PostInvoice(callback, invoice, responsebody);
                }
                else
                {
                    str = "Invalid invoice id !" + invoice.InvoiceId;
                }
            });
            return str;
        }


        [System.Web.Mvc.HttpPost]
        public string CreateAgreement([FromBody]AgreementModel agreement)
        {
            int id=0; string str="Thanks!";
            string query = "select ID from MobilePayOnBoarding where BCTenantId='" +agreement.BcTenantId + "'";
            try
            {
                DBManager.VerifyTenantID(query, status =>
                {

                    if (status.Equals("S"))
                        id = DBManager.InsertAgreement(agreement);

                    if (id == 0)
                        str = "ERROR on creating agreement!";
                    if (id == -1)
                        str = "Agreement Id already Exist !";

                });
                
            }
            catch (Exception eexx)
            {
                str = "Exception in Create Agreement " + eexx;
            }

            return str;
        }

        [System.Web.Mvc.HttpPost]
        public string UpdateAgreementStatus([FromBody]AgreementModel agreement)
        {
            int id = 0; string str = "Thanks!";
            try
            {
            DBManager.VerifyAgreement(agreement.Agreement_Id, callback => {

                if (callback!=null)
                {
                    DBManager.UpdateAgreement(agreement);
                    Proxy proxy = new Proxy();
                    if (callback.enableCallback.Equals("true"))
                        proxy.PostAgreement(callback, agreement);
                }
                else
                {
                    str = "Invalid agreement !";
                }
                

            });            
            }
            catch (Exception eexx)
            {
                str = "Exception in UpdateAgreement  " + eexx;
            }

            return str;
            }

        [System.Web.Mvc.HttpPost]
        public string GetTokens([FromBody]BCClientModel content)
        {
            BCClientModel bCClient = new BCClientModel();
            string json="";
            string query = "select ID from MobilePayOnBoarding where BCTenantId='" + content.BCTenantId + "'";
            DBManager.VerifyTenantID(query, status =>
            {
                
                if (status.Equals("S"))
                {
                    DBManager.GetTokens(content.BCTenantId, callback =>
                    {
                        var results = new
                        {
                            AccessToken = callback.accessToken,
                            RefreshToken = callback.refreshToken                            
                        };

                        json = JsonConvert.SerializeObject(results);

                    });
                }
                else
                {
                    json = "Invalide tenantId !";
                }

                     
             });
            return json;
        }

        private string GenerateAuthURL(BCClientModel content)
        {
            Thread thr = new Thread(new ThreadStart(mythread1));
            thr.Start();

            string url = "";
            BCClientModel bcClient = new BCClientModel();
            bcClient.userName = content.userName;
            bcClient.password = content.password;
            bcClient.BCTenantId = content.BCTenantId;
            bcClient.enableCallback = string.IsNullOrEmpty(content.enableCallback) ? "false" : content.enableCallback;
            bcClient.scope = content.scope;
            Proxy proxy = new Proxy();
            try
            {
                url = proxy.SendLogingRequest(bcClient, model =>
                {
                    DBManager.InsertRecord(model);
                });

            }
            catch (Exception eexx)
            {
                throw;
            }
            finally
            {

            }
            return url;
        }
    }
}