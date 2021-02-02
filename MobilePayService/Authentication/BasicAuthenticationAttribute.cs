using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MobilePayService.Authentication
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {

        private const string Realm = "mdcnordic";
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //If the Authorization header is empty or null
            //then return Unauthorized
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
                // If the request was unauthorized, add the WWW-Authenticate header 
                // to the response which indicates that it require basic authentication
                if (actionContext.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    actionContext.Response.Headers.Add("WWW-Authenticate",
                        string.Format("Basic realm=\"{0}\"", Realm));
                }
            }
            else
            {
                //Get the authentication token from the request header
                string authenticationToken = actionContext.Request.Headers
                    .Authorization.Parameter;
                //Decode the string
                string decodedAuthenticationToken = Encoding.UTF8.GetString(
                    Convert.FromBase64String(authenticationToken));
                //Convert the string into an string array
                string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                //First element of the array is the username
                string username = usernamePasswordArray[0];
                //Second element of the array is the password
                string password = usernamePasswordArray[1];
                //call the login method to check the username and password
                var isValid = username == ConfigurationManager.AppSettings["UserName"].ToString() && password == ConfigurationManager.AppSettings["Password"].ToString();

                if (isValid)
                {
                    var identity = new GenericIdentity(username);
                    IPrincipal principal = new GenericPrincipal(identity, null);
                    Thread.CurrentPrincipal = principal;
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = principal;
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }
        //var authHeader = actionContext.Request.Headers.Authorization;

        //if (authHeader != null)
        //{
        //    var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
        //    var decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
        //    var usernamePasswordArray = decodedAuthenticationToken.Split(':');
        //    var userName = usernamePasswordArray[0];
        //    var password = usernamePasswordArray[1];

        //    // Replace this with your own system of security / means of validating credentials
        //    var isValid = userName == ConfigurationManager.AppSettings["UserName"].ToString() && password == ConfigurationManager.AppSettings["Password"].ToString();

        //    if (isValid)
        //    {
        //        var principal = new GenericPrincipal(new GenericIdentity(userName), null);
        //        Thread.CurrentPrincipal = principal;

        //        actionContext.Response =
        //           actionContext.Request.CreateResponse(HttpStatusCode.OK,
        //              "User " + userName + " successfully authenticated");                    
        //    }
        //}
        //else
        //{
        //    HandleUnathorized(actionContext);
        //}


    }

}