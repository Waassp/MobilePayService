using MobilePayService.Methods;
using MobilePayService.Models;
using MobilePayService.RestAPI;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HttpServer = MobilePayService.RestAPI.HttpServer;

namespace MobilePayService.Controllers
{
    public class MobilePayIndexController : Controller
    {
        HttpProxyServer proxy = null;
        private new readonly HttpServer Server = null;
        readonly string REDIRECT_URL = ConfigurationManager.AppSettings["RedirectUrl"].ToString();

        public MobilePayIndexController()
        {
            Server = new HttpProxyServer();
        }
        public string GetBaseUrl()
        {
            var request = HttpContext.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
            {
                appUrl = "/" + appUrl;
            }

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        public ActionResult Index()
        {
            int key = Convert.ToInt32(HttpContext.Request.QueryString["sessionId"]);
            ViewBag.Url = DBManager.GetUrlByKey(key);

            return View("Welcome");
        }

        public ActionResult RedirectTest()
        {
            return View("redirect");
        }

        [RequireHttps]
        public ActionResult Redirect()
        {
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
        public ActionResult MobilePay([FromBody] BCClientModel content)
        {
            int insertedId = 0;
            string baseURL = GetBaseUrl();
            string returnedURL = GenerateAuthURL(content);            
            
            Server.Start(REDIRECT_URL);
            Thread.Sleep(2);
            try
            {
                SessionUrl su = new SessionUrl();
                su.url = returnedURL;
                su.bcTenantId = content.password;
                insertedId = DBManager.InsertRecordSession(su);
            }
            catch (Exception eexx)
            {
                throw eexx;
            }
            string dataToPassWithUrl = baseURL + "/MobilePayIndex" + "/?sessionId=" + insertedId;

            return Content(dataToPassWithUrl);
        }


        [System.Web.Mvc.HttpPost]
        public string GenerateInvoice([FromBody] InvoiceModel invoice)
        {

            int id = 0; string str = "Thanks!";
            string query = "select ID from MobilePayOnBoarding where BCTenantId = '" + invoice.BCTenantURL + "'";


            try
            {
                DBManager.VerifyTenantID(query, status =>
                {

                    if (status.Equals("S"))
                    {
                        id = DBManager.GenerateInvoice(invoice);
                    }

                    if (id == 0)
                    {
                        str = "ERROR on generating invoice!";
                    }

                    if (id == -1)
                    {
                        str = "Invoice URL Already Exist !";
                    }
                });

            }
            catch (Exception eexx)
            {
                str = "Exception on generate Invoice " + eexx;
            }

            return str;
        }

        public string PostInvoice(InvoiceModel[] invoice, string responsebody)
        {
            Thread.Sleep(2000);
            string str = "";
            try
            {
                foreach (var item in invoice)
                {
                    if (item.Status == "Invalid")
                    {
                        continue;
                    }
                    DBManager.VerifyInvoice(item, callback =>
                    {
                        if (callback != null)
                        {
                            proxy = new HttpProxyServer();
                            if (callback.enableCallback.Equals("true"))
                            {
                                proxy.PostInvoice(callback, item, responsebody);
                            }
                        }
                        else
                        {
                            str = "Invalid invoice id !" + item.InvoiceId;
                        }
                    });
                }
                str = "Thanks!";
                return str;
            }
            catch (Exception ex)
            {
                str = "Error occured, return this error to integrator MDCNordic!";
                return str;
            }

        }


        [System.Web.Mvc.HttpPost]
        public string CreateAgreement([FromBody] AgreementModel agreement)
        {
            int id = 0; string str = "Thanks!";
            string query = "select ID from MobilePayOnBoarding where BCTenantId='" + agreement.BcTenantId + "'";
            try
            {
                DBManager.VerifyTenantID(query, status =>
                {

                    if (status.Equals("S"))
                    {
                        id = DBManager.InsertAgreement(agreement);
                    }

                    if (id == 0)
                    {
                        str = "ERROR on creating agreement!";
                    }

                    if (id == -1)
                    {
                        str = "Agreement Id already Exist !";
                    }
                });

            }
            catch (Exception eexx)
            {
                str = "Exception in Create Agreement " + eexx;
            }

            return str;
        }

        [System.Web.Mvc.HttpPost]
        public string UpdateAgreementStatus([FromBody] AgreementModel agreement)
        {
            string str = "Thanks!";
            try
            {
                DBManager.VerifyAgreement(agreement.Agreement_Id, callback =>
                {

                    if (callback != null)
                    {
                        DBManager.UpdateAgreement(agreement);
                        proxy = new HttpProxyServer();
                        if (callback.enableCallback.Equals("true"))
                        {
                            proxy.PostAgreement(callback, agreement);
                        }
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
        public string GetTokens([FromBody] BCClientModel content)
        {
            BCClientModel bCClient = new BCClientModel();
            string json = "";
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
            string url = "";
            BCClientModel bcClient = new BCClientModel();
            bcClient.userName = content.userName;
            bcClient.password = content.password;
            bcClient.BCTenantId = content.BCTenantId;
            bcClient.enableCallback = string.IsNullOrEmpty(content.enableCallback) ? "false" : content.enableCallback;
            bcClient.scope = content.scope;
            proxy = new HttpProxyServer();
            try
            {
                url = proxy.SendLogingRequest(bcClient, model =>
                {
                    DBManager.InsertRecord(model);
                });

            }
            catch (Exception eexx)
            {
                throw eexx;
            }
            return url
        }
    }
}