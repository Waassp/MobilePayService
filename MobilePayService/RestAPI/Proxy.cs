﻿using MobilePayService.Methods;
using MobilePayService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace MobilePayService.RestAPI
{
    public class Proxy
    {
        //private WebClient client = null;
        private Uri Url = null;
        private string parameters = "";
        public Proxy()
        {
            //client = new System.Net.WebClient();
            //client.Headers.Add("X-AppSecretToken", appId);
            //client.Headers.Add("X-AgreementGrantToken", granttoken);
            //client.Headers.Add("Content-Type", "application/json");
        }
        public string SendLogingRequest(BCClientModel clientModel, Action<BCClientModel> callback)
        {
            string HtmlResult = "";
            AuthCodeMethod.GetAccessTokenAsync(clientModel, model => {
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

        public string getRefereshToken(AccessTokenModel model, Action<AccessTokenModel> callback)
        {
            var pocoObject = new
            {
                Name = "Waqar",
                Occupation = "developer"
            };
            var json = JsonConvert.SerializeObject(pocoObject);
            var data = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
            var parameters = new Dictionary<string, string>();
            parameters["grant_type"] = model.grant_type;
            parameters["code"]       = model.code;
            parameters["redirect_uri"] = model.redirect_uri;
            parameters["code_verifier"] = model.code_verifier;
            parameters["client_id"] = model.client_id;
            parameters["client_secret"] = model.client_secret;
            Url = new Uri("https://api.sandbox.mobilepay.dk/merchant-authentication-openidconnect/connect/token");
           

            //Create a query
            HttpClient client = new HttpClient();
            var response = client.PostAsync(Url, new FormUrlEncodedContent(parameters));
            //string result = response.Content.ReadAsStringAsync();

            //close out the client
            client.Dispose();

            //string HtmlResult = "";
            //parameters = null;
            ////AuthCodeMethod.GetAccessTokenAsync(clientModel, model => {


            //Query = "grant_type =" + model.grant_type + "&code=" + model.code + "&redirect_uri=" + model.redirect_uri + "&code_verifier=" + model.code_verifier + "&client_id=" + model.client_id +
            //   "&client_secret=" + model.client_secret + "";
            //using (WebClient wc = new WebClient())
            //{
            //    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //    wc.QueryString.Add("grant_type", model.grant_type);
            //    wc.QueryString.Add("code", model.code);
            //    wc.QueryString.Add("redirect_uri", model.redirect_uri);
            //    wc.QueryString.Add("code_verifier", model.code_verifier);
            //    wc.QueryString.Add("client_id", model.client_id);
            //    wc.QueryString.Add("client_secret", model.client_secret);
            //    var data = wc.UploadValues(Url, "POST", wc.QueryString);
            //    // data here is optional, in case we recieve any string data back from the POST request.
            //    HtmlResult = UnicodeEncoding.UTF8.GetString(data);
            //    JObject json = JObject.Parse(HtmlResult);
            //    model.access_token = json.GetValue("access_token").ToString();
            //    model.refresh_token = json.GetValue("refresh_token").ToString();

            //}
            callback(model);

            return "";//HtmlResult;
        }

        public static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }

            using (var body = request.InputStream)
            {
                using (var reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static HttpWebRequest CreateWebRequest(String SoapUrl, string Cred)
        {
            string base64Cred = Base64Encode(Cred);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(SoapUrl);
            webRequest.Headers.Add(@"SOAPAction: 'urn:microsoft-dynamics-schemas/codeunit/MerchantTokens:PutMerchantTokens'");
            webRequest.Headers.Add(HttpRequestHeader.Authorization,
               "Basic " + base64Cred);
            webRequest.ContentType = "text/xml;charset=\"UTF-8\"";

            webRequest.Method = "POST";
            return webRequest;
        }

        public static string GetMerchantId(string accessToken)
        {
            string merchantId="";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://api.sandbox.mobilepay.dk/invoice-restapi/api/v1/merchants/me");
            webRequest.Headers.Add("Authorization:"+ accessToken);
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
                        if(!string.IsNullOrEmpty(merchantId))
                            merchantId = JsonConvert.DeserializeObject<dynamic>(merchantId)["MerchantId"].Value; 
                    }
                }
            }
            catch (Exception e)
            {
                merchantId = null;                
            }
            return merchantId;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public void PostInvoice(BCClientModel clientModel, InvoiceModel invoice,string responsebody)
        {
            string Cred = clientModel.userName + ":" + clientModel.password;
            HttpWebRequest request = CreateWebRequest(invoice.InvoiceCallBackSoapURL, Cred);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:mer='urn:microsoft-dynamics-schemas/codeunit/Invoice_CallBack'> <soapenv:Body> <mer:InvoiceCallBack> <mer:invoiceId>" + invoice.InvoiceId + "</mer:invoiceId> <mer:status>" + invoice.Status + "</mer:status> <mer:errorCode>" + invoice.ErrorCode + "</mer:errorCode> <mer:errorMessage>" + invoice.ErrorMessage + "</mer:errorMessage> <mer:date>" + invoice.Date + "</mer:date> <mer:response_Body>" + responsebody + "</mer:response_Body> </mer:InvoiceCallBack> </soapenv:Body> </soapenv:Envelope>");
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
            catch (Exception e)
            {
                throw;
                
            }
        }

        public void PostAgreement(BCClientModel clientModel, AgreementModel agreement)
        {

            string Cred = clientModel.userName + ":" + clientModel.password;
            HttpWebRequest request = CreateWebRequest(clientModel.BCTenantId, Cred);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:mer='urn:microsoft-dynamics-schemas/codeunit/MerchantTokens'> <soapenv:Body> <mer:AgreementStatus> <mer:agreementId>" +agreement.Agreement_Id + "</mer:agreementId> <mer:status>" + agreement.Status + "</mer:status> <mer:statusText>" + agreement.Status_Text + "</mer:statusText> <mer:statusCode>" + agreement.Status_Code + "</mer:statusCode> <mer:callBackTime>" + agreement.Timestamp + "</mer:callBackTime> </mer:AgreementStatus> </soapenv:Body> </soapenv:Envelope>");

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
            catch (Exception e)
            {

                throw;
            }
        }

        internal void PostToClient(string userName,string Password,string url,string AccessToken,string RefreshToken)
         {

            string Cred = userName + ":" + Password;
            HttpWebRequest request = CreateWebRequest(url, Cred);
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
            catch (Exception e)
            {

                throw;
            }
        }
        public void SimpleListenerExample(string redirectUrl)
        {
            
            BCClientModel clientModel = new BCClientModel();
            AccessTokenModel accessToken = new AccessTokenModel();
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            string formData = "";//"code=04b7783f570a97b241aafee88b25be24abb03b36a5478075fe38081de52fbce7&id_token=eyJhbGciOiJSUzI1NiIsImtpZCI6IkFGQTNBRjQxRDVCMjk4QkI5ODVGOTFDNEQxNzlGNTQwRjVBREE2MzAiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJyNk92UWRXeW1MdVlYNUhFMFhuMVFQV3RwakEifQ.eyJuYmYiOjE1ODIxNzg4MDIsImV4cCI6MTU4MjE3OTEwMiwiaXNzIjoiaHR0cHM6Ly9hcGkubW9iaWxlcGF5LmRrL21lcmNoYW50IiwiYXVkIjoibWRjbm9yZGljIiwibm9uY2UiOiIwQVY2cDRQanV1RDZtcmR5eV9DVEZrakh4c0NpVHFfUGtoQWtiQUh1TUhuZTA2UFEiLCJpYXQiOjE1ODIxNzg4MDIsImNfaGFzaCI6InBsOU42aGd3WXVDMlQ4MXhHWUZKUlEiLCJzdWIiOiIyMzUzZTI5Mi1hMmM4LTRjODEtODA3ZC0wYzEwODUxMTA2NTYiLCJhdXRoX3RpbWUiOjE1ODIxNzUxMzQsImlkcCI6ImxvY2FsIiwiYW1yIjpbIlVzZXJFbnRlcmVkQ29kZSIsIlNlcnZlclByb3ZpZGVkS2V5Il19.VJDD_xuShCpWvdVd-Pyw_2mDUlIP1WQ5UsyUdBVXNq9w_Eh15vAB7UWxhdEaZ6JzYi35n1jEBp1KCedxy2jEVJ4YFgNx81HzetRaEpgHH7-ylrsb6XWiGObeRzLaLeybUT93wfhiXsisG5ce80T4MKZusd1VduZwP2SoGcqbyBl5I4pNVgScSK0JVFNfmlM3JKwjdDeoLzma7SN7qMR3ZAhPr99nu3L_Hde-SRxv_o190sb4j5TjNHNS6ZO-8A1LSeX2KGop4IrdYpWTvucX0wkDGifSwX-yC6-HdUc6D3zY9zs3X3_6q7Dr5KNtLx1q8uU9b1ErMuHMuNaZ3IOtMg&scope=openid%20subscriptions%20offline_access&state=oVHk-e_GRpmfSeg6HsV35V8RFvyGMBmZDux2Tu8FLtA";
            Regex rx = new Regex(@"=(.*?)&|=.*");


            // Create a listener.
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(redirectUrl);
            listener.Start();

            Console.WriteLine("Listening...");
            // Note: The GetContext method blocks while waiting for a request. 
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            
            formData = GetRequestPostData(request);
            int count = 0;
            foreach (Match mech in rx.Matches(formData))
            {
                if (count == 0)
                    accessToken.code = mech.Groups[1].Value;
                if (count == 1)
                    accessToken.id_token = mech.Groups[1].Value;
                if (count == 3)
                    accessToken.state = mech.Groups[0].Value.ToString().Replace('=', ' ').Trim();
                count++;

            }

            try
            {
                accessToken.code_verifier = DBManager.GetCodeVerifier(accessToken.state, callback =>
                {
                    clientModel = callback;
                });
                
                string returns = getRefereshToken(accessToken, model =>
                {
                    DBManager.AddTokens(model);
                    if (clientModel.enableCallback.Equals("true"))
                    {
                        string merchantId = "";
                        PostToClient(clientModel.userName, clientModel.password, clientModel.BCTenantId, model.access_token, model.refresh_token);

                        if (!string.IsNullOrEmpty(model.access_token))
                            merchantId = GetMerchantId(model.access_token);
                        if (!string.IsNullOrEmpty(merchantId) && !string.IsNullOrEmpty(model.access_token))
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
                // Stop HttpListener
                listener.Stop();
            }
            
        }
    }
}