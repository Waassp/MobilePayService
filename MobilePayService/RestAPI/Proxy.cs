using MobilePayService.Methods;
using MobilePayService.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
    public class Proxy
    {
        public HttpServer Server = null;
       // private Uri Url = null;
       // private string parameters = "";
        public Proxy()
        {
            Server = new HttpProxyServer();
        }
        //public string SendLogingRequest(BCClientModel clientModel, Action<BCClientModel> callback)
        //{
        //    string HtmlResult = "";
        //    AuthCodeMethod.GetAccessTokenAsync(clientModel, model =>
        //    {
        //        Url = new Uri(clientModel.url);
        //        parameters = model.url + "?response_type=" + model.response_type + "&client_id=" + model.client_id + "&redirect_uri=" + model.redirect_uri + "&scope=openid" + model.scope + "offline_access&state=" + clientModel.state +
        //            "&code_challenge=" + model.code_challenge + "&code_challenge_method=" + model.code_challenge_method + "&nonce=" + model.nonce + "&response_mode=form_post";

        //        using (WebClient wc = new WebClient())
        //        {
        //            HtmlResult = wc.DownloadString(parameters);
        //        }
        //        callback(model);
        //    });
        //    return parameters;

        //}

        //public void getRefereshToken(AccessTokenModel model, Action<AccessTokenModel> callback)
        //{
        //    var data = new List<KeyValuePair<string, string>>();
        //    data.Add(new KeyValuePair<string, string>("grant_type", model.grant_type));
        //    data.Add(new KeyValuePair<string, string>("code", model.code));
        //    data.Add(new KeyValuePair<string, string>("redirect_uri", "https://dev.mdcnordic.com/MobilPayService/MobilePayIndex/redirect/"));
        //    data.Add(new KeyValuePair<string, string>("code_verifier", model.code_verifier));
        //    data.Add(new KeyValuePair<string, string>("client_id", "mdcnordic"));
        //    data.Add(new KeyValuePair<string, string>("client_secret", model.client_secret));

        //    HttpContent content = new FormUrlEncodedContent(data);

        //    string jsonContent = content.ReadAsStringAsync().Result;
        //    var responseString = "";
        //    using (var httpClient = new HttpClient(new Http2CustomHandler()))
        //    {
        //        // Send the request to the server
        //        HttpResponseMessage response = httpClient.PostAsync("https://api.sandbox.mobilepay.dk/merchant-authentication-openidconnect/connect/token", content).Result;

        //        // Get the response
        //        responseString = response.Content.ReadAsStringAsync().Result;
        //    }
        //    JObject json = JObject.Parse(responseString);
        //    model.access_token = json.GetValue("access_token").ToString();
        //    model.refresh_token = json.GetValue("refresh_token").ToString();

        //    callback(model);
        //}

        //public static string GetRequestPostData(HttpListenerRequest request)
        //{
        //    if (!request.HasEntityBody)
        //    {
        //        return null;
        //    }

        //    using (var body = request.InputStream)
        //    {
        //        using (var reader = new StreamReader(body, request.ContentEncoding))
        //        {
        //            return reader.ReadToEnd();
        //        }
        //    }
        //}

        //public static HttpWebRequest CreateWebRequest(String SoapUrl, string Cred)
        //{
        //    string base64Cred = Base64Encode(Cred);
        //    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(SoapUrl);
        //    webRequest.Headers.Add(@"SOAPAction: 'urn:microsoft-dynamics-schemas/codeunit/MerchantTokens:PutMerchantTokens'");
        //    webRequest.Headers.Add(HttpRequestHeader.Authorization,
        //       "Basic " + base64Cred);
        //    webRequest.ContentType = "text/xml;charset=\"UTF-8\"";

        //    webRequest.Method = "POST";
        //    return webRequest;
        //}

        //public static string GetMerchantId(string accessToken)
        //{
        //    string merchantId = "";
        //    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://api.sandbox.mobilepay.dk/invoice-restapi/api/v1/merchants/me");
        //    webRequest.Headers.Add("Authorization:" + accessToken);
        //    webRequest.Headers.Add("x-ibm-client-id:5a1b01c5-f7f6-44cb-b712-6c027b831e86");
        //    webRequest.Headers.Add("x-ibm-client-secret:P4cO4aR0fI5dE5aL7pR2rJ8qT8lD5vK7yL3qW7qD3jP3dB5bJ2");
        //    webRequest.ContentType = "text/xml;charset=\"UTF-8\"";
        //    webRequest.Method = "GET";
        //    try
        //    {
        //        using (WebResponse response = webRequest.GetResponse())
        //        {
        //            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
        //            {
        //                merchantId = rd.ReadToEnd();
        //                if (!string.IsNullOrEmpty(merchantId))
        //                    merchantId = JsonConvert.DeserializeObject<dynamic>(merchantId)["MerchantId"].Value;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        merchantId = null;
        //    }
        //    return merchantId;
        //}

        //public static string Base64Encode(string plainText)
        //{
        //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        //    return Convert.ToBase64String(plainTextBytes);
        //}

        //public void PostInvoice(BCClientModel clientModel, InvoiceModel invoice, string responsebody)
        //{
        //    string Cred = clientModel.userName + ":" + clientModel.password;
        //    HttpWebRequest request = Common.CreateWebRequest(invoice.InvoiceCallBackSoapURL, Cred);
        //    XmlDocument soapEnvelopeXml = new XmlDocument();
        //    soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:mer='urn:microsoft-dynamics-schemas/codeunit/Invoice_CallBack'> <soapenv:Body> <mer:InvoiceCallBack> <mer:invoiceId>" + invoice.InvoiceId + "</mer:invoiceId> <mer:status>" + invoice.Status + "</mer:status> <mer:errorCode>" + invoice.ErrorCode + "</mer:errorCode> <mer:errorMessage>" + invoice.ErrorMessage + "</mer:errorMessage> <mer:date>" + invoice.Date + "</mer:date> <mer:response_Body>" + responsebody + "</mer:response_Body> </mer:InvoiceCallBack> </soapenv:Body> </soapenv:Envelope>");
        //    using (Stream stream = request.GetRequestStream())
        //    {
        //        soapEnvelopeXml.Save(stream);
        //    }
        //    try
        //    {
        //        using (WebResponse response = request.GetResponse())
        //        {
        //            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
        //            {
        //                string soapResult = rd.ReadToEnd();
        //                Console.WriteLine(soapResult);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw;

        //    }
        //}

        //public void PostAgreement(BCClientModel clientModel, AgreementModel agreement)
        //{

        //    string Cred = clientModel.userName + ":" + clientModel.password;
        //    HttpWebRequest request = Common.CreateWebRequest(clientModel.BCTenantId, Cred);
        //    XmlDocument soapEnvelopeXml = new XmlDocument();
        //    soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:mer='urn:microsoft-dynamics-schemas/codeunit/MerchantTokens'> <soapenv:Body> <mer:AgreementStatus> <mer:agreementId>" + agreement.Agreement_Id + "</mer:agreementId> <mer:status>" + agreement.Status + "</mer:status> <mer:statusText>" + agreement.Status_Text + "</mer:statusText> <mer:statusCode>" + agreement.Status_Code + "</mer:statusCode> <mer:callBackTime>" + agreement.Timestamp + "</mer:callBackTime> </mer:AgreementStatus> </soapenv:Body> </soapenv:Envelope>");

        //    using (Stream stream = request.GetRequestStream())
        //    {
        //        soapEnvelopeXml.Save(stream);
        //    }
        //    try
        //    {
        //        using (WebResponse response = request.GetResponse())
        //        {
        //            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
        //            {
        //                string soapResult = rd.ReadToEnd();
        //                Console.WriteLine(soapResult);

        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        throw;
        //    }
        //}

        //internal void PostToClient(string userName, string Password, string url, string AccessToken, string RefreshToken)
        //{

        //    string Cred = userName + ":" + Password;
        //    HttpWebRequest request = CreateWebRequest(url, Cred);
        //    XmlDocument soapEnvelopeXml = new XmlDocument();
        //    soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:mer='urn:microsoft-dynamics-schemas/codeunit/MerchantTokens'> <soapenv:Body> <mer:PutMerchantTokens> <mer:accessToken>" + AccessToken + "</mer:accessToken> <mer:refreshToken>" + RefreshToken + "</mer:refreshToken> </mer:PutMerchantTokens> </soapenv:Body> </soapenv:Envelope>");

        //    using (Stream stream = request.GetRequestStream())
        //    {
        //        soapEnvelopeXml.Save(stream);
        //    }
        //    try
        //    {
        //        using (WebResponse response = request.GetResponse())
        //        {
        //            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
        //            {
        //                string soapResult = rd.ReadToEnd();
        //                Console.WriteLine(soapResult);

        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        throw;
        //    }
        //}
        public void SimpleListenerExample(string redirectUrl)
        {
           // Server.Start(redirectUrl);
            //  BCClientModel clientModel = new BCClientModel();
            //AccessTokenModel accessToken = new AccessTokenModel();
            //if (!HttpListener.IsSupported)
            //{
            //    Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            //    return;
            //}
            // string formData = "";//"code=04b7783f570a97b241aafee88b25be24abb03b36a5478075fe38081de52fbce7&id_token=eyJhbGciOiJSUzI1NiIsImtpZCI6IkFGQTNBRjQxRDVCMjk4QkI5ODVGOTFDNEQxNzlGNTQwRjVBREE2MzAiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJyNk92UWRXeW1MdVlYNUhFMFhuMVFQV3RwakEifQ.eyJuYmYiOjE1ODIxNzg4MDIsImV4cCI6MTU4MjE3OTEwMiwiaXNzIjoiaHR0cHM6Ly9hcGkubW9iaWxlcGF5LmRrL21lcmNoYW50IiwiYXVkIjoibWRjbm9yZGljIiwibm9uY2UiOiIwQVY2cDRQanV1RDZtcmR5eV9DVEZrakh4c0NpVHFfUGtoQWtiQUh1TUhuZTA2UFEiLCJpYXQiOjE1ODIxNzg4MDIsImNfaGFzaCI6InBsOU42aGd3WXVDMlQ4MXhHWUZKUlEiLCJzdWIiOiIyMzUzZTI5Mi1hMmM4LTRjODEtODA3ZC0wYzEwODUxMTA2NTYiLCJhdXRoX3RpbWUiOjE1ODIxNzUxMzQsImlkcCI6ImxvY2FsIiwiYW1yIjpbIlVzZXJFbnRlcmVkQ29kZSIsIlNlcnZlclByb3ZpZGVkS2V5Il19.VJDD_xuShCpWvdVd-Pyw_2mDUlIP1WQ5UsyUdBVXNq9w_Eh15vAB7UWxhdEaZ6JzYi35n1jEBp1KCedxy2jEVJ4YFgNx81HzetRaEpgHH7-ylrsb6XWiGObeRzLaLeybUT93wfhiXsisG5ce80T4MKZusd1VduZwP2SoGcqbyBl5I4pNVgScSK0JVFNfmlM3JKwjdDeoLzma7SN7qMR3ZAhPr99nu3L_Hde-SRxv_o190sb4j5TjNHNS6ZO-8A1LSeX2KGop4IrdYpWTvucX0wkDGifSwX-yC6-HdUc6D3zY9zs3X3_6q7Dr5KNtLx1q8uU9b1ErMuHMuNaZ3IOtMg&scope=openid%20subscriptions%20offline_access&state=oVHk-e_GRpmfSeg6HsV35V8RFvyGMBmZDux2Tu8FLtA";
            // Regex rx = new Regex(@"=(.*?)&|=.*");



            //IAsyncResult result = listener.BeginGetContext(new AsyncCallback(WebRequestCallback), listener);
            // Create a listener.

            //HttpListener listener = new HttpListener();
            //listener.Prefixes.Add(redirectUrl);
            //listener.Start();

            //Console.WriteLine("Listening...");
            // Note: The GetContext method blocks while waiting for a request. 

            // HttpListenerContext context = listener.GetContext();
            //  HttpListenerRequest request = context.Request;
            //  HttpListenerResponse response = context.Response;

            // formData = GetRequestPostData(request);
            //int count = 0;
            //foreach (Match mech in rx.Matches(formData))
            //{
            //    if (count == 0)
            //        accessToken.code = mech.Groups[1].Value;
            //    if (count == 1)
            //        accessToken.id_token = mech.Groups[1].Value;
            //    if (count == 3)
            //        accessToken.state = mech.Groups[0].Value.ToString().Replace('=', ' ').Trim();
            //    count++;

            //}

            //try
            //{
            //    accessToken.code_verifier = DBManager.GetCodeVerifier(accessToken.state, callback =>
            //    {
            //        clientModel = callback;
            //    });

            //    //string returns = 
            //    getRefereshToken(accessToken, model =>
            //    {
            //        DBManager.AddTokens(model);
            //        if (clientModel.enableCallback.Equals("true"))
            //        {
            //            string merchantId = "";
            //            PostToClient(clientModel.userName, clientModel.password, clientModel.BCTenantId, model.access_token, model.refresh_token);

            //            if (!string.IsNullOrEmpty(model.access_token))
            //                merchantId = GetMerchantId(model.access_token);
            //            if (!string.IsNullOrEmpty(merchantId) && !string.IsNullOrEmpty(model.access_token))
            //                DBManager.AddMerchantID(merchantId, model);
            //        }

            //    });



            //}
            //catch (Exception eexx)
            //{
            //    throw eexx;
            //}
            //finally
            //{

            //    string responseString = "<!DOCTYPE html> <html> <head> <title>Page Title</title> </head> <body> <div style=\"position:fixed;top:0;left:0;min-width:100%; min-height:50px;background-color:#222;border-color:#080808;\"> <p>&nbsp;</p> </div>  <div style=\"padding-left: 25%; margin-right: auto; margin-left: auto; margin-top: 4%;\">   <h2 style=\"font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-weight: 500; line-height: 1.1;font-size: 30px;\">Congratulations !</h2> <h3 style=\"font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-weight: 500; line-height: 1.1;font-size: 24px;\">To continue Go back to your application</h3>    <hr style=\"margin-top: 20px; margin-bottom: 20px; border: 0; border-top: 1px solid #eee;\"/> <footer> <p style=\"font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 14px; line-height: 1.428571429; color: #333;\"> &copy; <script>document.write(new Date().getFullYear())</script> - Merchant On-Boarding</p> </footer> </div>  </body> </html>";
            //    byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            //    response.ContentLength64 = buffer.Length;
            //    Stream output = response.OutputStream;
            //    output.Write(buffer, 0, buffer.Length);

            //    output.Close();
            //    // Stop HttpListener
            //    listener.Stop();
            //}

        }
    }
}