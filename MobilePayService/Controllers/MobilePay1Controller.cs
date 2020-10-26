using MobilePayService.Methods;
using MobilePayService.Models;
using MobilePayService.RestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;

namespace MobilePayService.Controllers
{
    public class MobilePay1Controller : Controller
    {
        Thread t;
        HttpListener listener;

        // Non-static method 
        private void mythread1()
        {
            BCClientModel bcClient = new BCClientModel();

            Proxy proxy = new Proxy();
            proxy.SimpleListenerExample(bcClient.redirect_uri);
            //listener = new HttpListener();
            //listener.Prefixes.Add("https://dev.mdcnordic.com:8443/mobilepay/redirect/");
            //listener.Start();
            // t.Start();
        }
        // GET: api/MobilePay
        [System.Web.Http.Route("MobilePay")]
        [System.Web.Http.HttpPost]
        public string Post([FromBody]BCClientModel content)
        {
            MobilePay1Controller obj = new MobilePay1Controller();
            // Creating thread 
            // Using thread class 
            Thread thr = new Thread(new ThreadStart(obj.mythread1));
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
        [System.Web.Http.HttpGet]
        // GET: api/MobilePay/5
        public string Get()
        {
            return "value";
        }

        public ActionResult Index()
        {
            return View();
        }


        [System.Web.Http.Route("RefreshToken")]
        [System.Web.Http.HttpPost]
        // POST: api/MobilePay
        public void Post([FromBody]string value)
        {
            AccessTokenModel accessToken = new AccessTokenModel();
            //bcClient.userName = "meganb@m365x009498.onmicrosoft.com";
            //bcClient.password = "D7tSLRBqzPe0UAmqQe3vyJbw0WGC4/RCjnpxrrPTdr0=";
            //bcClient.BCTenantId = "https://api.businesscentral.dynamics.com/v2.0/a6aec78e-8b25-4bc0-8e2f-2ab576f0fa66/batchflow4-sandbox/WS/CRONUS%20Danmark%20A%2FS/Codeunit/AgreementCallBack";
            Proxy proxy = new Proxy();
            // string p = proxy.getRefreshedToken();
            //AuthCodeMethod.GetAccessTokenAsync();
            accessToken.code = "341b9e646059fb42e21d72d17dff5c05306cff94f2ea6048e4bf96b07bef2896";
            accessToken.code_verifier = "gr9VwnmCJ-a-qWB2bd01dpnPGZHNzsOGC2qnNlxcm1g";
            try
            {
                proxy.getRefereshToken(accessToken, model =>
                {
                    DBManager.AddTokens(model);
                });
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

        }

        // PUT: api/MobilePay/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MobilePay/5
        public void Delete(int id)
        {
        }
    }
}
