using System.Web.Http;
using System.Web.Routing;

namespace MobilePayService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Proxy.SimpleListenerExample("https://dev.mdcnordic.com:8443/mobilepay/redirect/");
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
