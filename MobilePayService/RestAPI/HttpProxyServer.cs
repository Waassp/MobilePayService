using MobilePayService.Methods;
using MobilePayService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;

namespace MobilePayService.RestAPI
{
    public class Http2CustomHandler : WinHttpHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Version = new Version("2.0");
            return base.SendAsync(request, cancellationToken);
        }
    }
    public class HttpProxyServer : HttpServer
    {
        private Uri Url = null;
        private string parameters = "";
        protected override void ProcessRequest(HttpListenerContext Context)
        {
            HttpListenerRequest request = Context.Request;
            HttpListenerResponse response = Context.Response;
            string formData = GetRequestPostData(request);

            AccessTokenModel accessToken = new AccessTokenModel();
            BCClientModel clientModel = new BCClientModel();
            Regex rx = new Regex(@"=(.*?)&|=.*");
            int count = 0;
            foreach (Match mech in rx.Matches(formData))
            {
                if (count == 0)
                {
                    accessToken.code = mech.Groups[1].Value;
                }

                if (count == 1)
                {
                    accessToken.id_token = mech.Groups[1].Value;
                }

                if (count == 3)
                {
                    accessToken.state = mech.Groups[0].Value.ToString().Replace('=', ' ').Trim();
                }

                count++;
            }

            try
            {
                accessToken.code_verifier = DBManager.GetCodeVerifier(accessToken.state, callback =>
                {
                    clientModel = callback;
                });

                getRefereshToken(accessToken, model =>
                {
                    DBManager.AddTokens(model);
                   
                        string merchantId = "";
                        //PostToClient(clientModel.userName, clientModel.password, clientModel.BCTenantId, model.access_token, model.refresh_token);

                        if (!string.IsNullOrEmpty(model.access_token))
                        {
                            merchantId = GetMerchantId(model.access_token);
                        }

                        if (!string.IsNullOrEmpty(merchantId) && !string.IsNullOrEmpty(model.access_token))
                        {
                            DBManager.AddMerchantID(merchantId, model);
                        }
               

                });
            }
            catch (Exception eexx)
            {
                throw eexx;
            }
            finally
            {
                string responseString = "<!DOCTYPE html> <html> <head> <title>Page Title</title> </head> <body> <div style=\"position:fixed;top:0;left:0;min-width:100%; min-height:50px;background-color:#222;border-color:#080808;\"> <p>&nbsp;</p> </div>  <div style=\"padding-left: 25%; margin-right: auto; margin-left: auto; margin-top: 4%;\">   <h2 style=\"font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-weight: 500; line-height: 1.1;font-size: 30px;\">Congratulations !</h2> <h3 style=\"font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-weight: 500; line-height: 1.1;font-size: 24px;\">To continue Go back to your application</h3>    <hr style=\"margin-top: 20px; margin-bottom: 20px; border: 0; border-top: 1px solid #eee;\"/> <footer> <p style=\"font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 14px; line-height: 1.428571429; color: #333;\"> &copy; <script>document.write(new Date().getFullYear())</script> - Merchant On-Boarding</p> </footer> </div>  </body> </html>";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                output.Close();
                // HttpServer.Stop();
                //Listener.Stop();
            }
        }
        private static string GetMerchantId(string accessToken)
        {
            string merchantId = "";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://api.sandbox.mobilepay.dk/invoice-restapi/api/v1/merchants/me");
            webRequest.Headers.Add("Authorization:" + accessToken);
            webRequest.Headers.Add("x-ibm-client-id:5a1b01c5-f7f6-44cb-b712-6c027b831e86");
            webRequest.Headers.Add("x-ibm-client-secret:P4cO4aR0fI5dE5aL7pR2rJ8qT8lD5vK7yL3qW7qD3jP3dB5bJ2");
            webRequest.ContentType = "text/xml;charset=\"UTF-8\"";
            webRequest.Method = "GET";
            try
            {
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        merchantId = rd.ReadToEnd();
                        if (!string.IsNullOrEmpty(merchantId))
                        {
                            merchantId = JsonConvert.DeserializeObject<dynamic>(merchantId)["MerchantId"].Value;
                        }
                    }
                }
            }
            catch (Exception)
            {
                merchantId = null;
            }
            return merchantId;
        }
        private void getRefereshToken(AccessTokenModel model, Action<AccessTokenModel> callback)
        {
            var data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("grant_type", model.grant_type));
            data.Add(new KeyValuePair<string, string>("code", model.code));
            data.Add(new KeyValuePair<string, string>("redirect_uri", "https://dev.mdcnordic.com/MobilPayService/MobilePayIndex/redirect/"));
            data.Add(new KeyValuePair<string, string>("code_verifier", model.code_verifier));
            data.Add(new KeyValuePair<string, string>("client_id", "mdcnordic"));
            data.Add(new KeyValuePair<string, string>("client_secret", model.client_secret));

            HttpContent content = new FormUrlEncodedContent(data);

            string jsonContent = content.ReadAsStringAsync().Result;
            var responseString = "";
            using (var httpClient = new HttpClient(new Http2CustomHandler()))
            {
                // Send the request to the server
                HttpResponseMessage response = httpClient.PostAsync("https://api.sandbox.mobilepay.dk/merchant-authentication-openidconnect/connect/token", content).Result;

                // Get the response
                responseString = response.Content.ReadAsStringAsync().Result;
            }
            JObject json = JObject.Parse(responseString);
            model.access_token = json.GetValue("access_token").ToString();
            model.refresh_token = json.GetValue("refresh_token").ToString();

            callback(model);
        }
        internal void PostToClient(string userName, string Password, string url, string AccessToken, string RefreshToken)
        {
            string Cred = userName + ":" + Password;
            string soapAction = "urn:microsoft-dynamics-schemas/codeunit/MerchantTokens:PutMerchantTokens";
            HttpWebRequest request = Common.CreateWebRequest(url, Cred, soapAction);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:mer='urn:microsoft-dynamics-schemas/codeunit/MerchantTokens'> <soapenv:Body> <mer:PutMerchantTokens> <mer:accessToken>" + AccessToken + "</mer:accessToken> <mer:refreshToken>" + RefreshToken + "</mer:refreshToken> </mer:PutMerchantTokens> </soapenv:Body> </soapenv:Envelope>");

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string soapResult = rd.ReadToEnd();
                        Console.WriteLine(soapResult);

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }

            using (var body = request.InputStream)
            {
                using (var reader = new StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public string SendLogingRequest(BCClientModel clientModel, Action<BCClientModel> callback)
        {
            string HtmlResult = "";
            AuthCodeMethod.GetAccessTokenAsync(clientModel, model =>
            {
                Url = new Uri(clientModel.url);
                parameters = model.url + "?response_type=" + model.response_type + "&client_id=" + model.client_id + "&redirect_uri=" + model.redirect_uri + "&scope=openid" + model.scope + "offline_access&state=" + clientModel.state +
                    "&code_challenge=" + model.code_challenge + "&code_challenge_method=" + model.code_challenge_method + "&nonce=" + model.nonce + "&response_mode=form_post";

                using (WebClient wc = new WebClient())
                {
                    HtmlResult = wc.DownloadString(parameters);
                }
                callback(model);
            });
            return parameters;

        }
    }
}