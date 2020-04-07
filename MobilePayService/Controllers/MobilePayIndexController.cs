using MobilePayService.Methods;
using MobilePayService.Models;
using MobilePayService.RestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
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
       // [System.Web.Http.Route("MobilePay")]
        public ActionResult MobilePay([FromBody]BCClientModel content)
        {
            int insertedId = 0;
            string baseURL = GetBaseUrl();
            string returnedURL = GenerateAuthURL(content);
            //Set("url", returnedURL);


            try
            {
                SessionUrl su = new SessionUrl();
                su.url = returnedURL;
                su.bcTenantId = content.password;
                insertedId =     DBManager.InsertRecordSession(su);
            }
            catch (Exception eexx)
            {
                throw eexx;
            }
            string dataToPassWithUrl = baseURL + "/MobilePayIndex" + "/?sessionId=" + insertedId;


            return Content(dataToPassWithUrl);
        }

        [System.Web.Mvc.HttpPost]
        public string CreateAgreement([FromBody]AgreementModel agreement)
        {
            int id=0; string str="Thanks!";
            try
            {
                DBManager.VerifyTenantID(agreement.BcTenantId, status =>
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

        private string GenerateAuthURL(BCClientModel content)
        {
            Thread thr = new Thread(new ThreadStart(mythread1));
            thr.Start();

            string url = "";
            BCClientModel bcClient = new BCClientModel();
            bcClient.userName = content.userName;// "SY";
            bcClient.password = content.password;// "D7tSLRBqzPe0UAmqQe3vyJbw0WGC4/RCjnpxrrPTdr0=";
            bcClient.BCTenantId = content.BCTenantId;// "https://api.businesscentral.dynamics.com/v2.0/a6aec78e-8b25-4bc0-8e2f-2ab576f0fa66/batchflow4-sandbox/WS/CRONUS%20Danmark%20A%2FS/Codeunit/AgreementCallBack";
            Proxy proxy = new Proxy();
            // string p = proxy.getRefreshedToken();
            //AuthCodeMethod.GetAccessTokenAsync();
            try
            {
                url = proxy.SendLogingRequest(bcClient, model =>
                {
                    DBManager.InsertRecord(model);
                });
                //proxy.SimpleListenerExample(bcClient.redirect_uri);

            }
            catch (Exception eexx)
            {
                throw eexx;
            }
            finally
            {
                // Stop HttpListener
                // httpListener.Stop();
            }


            return url;
        }
    }
}