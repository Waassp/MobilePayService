using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MobilePayService.Models
{
    public class BCClientModel
    {
        public string state { get; set; }
        public string code_challenge { get; set; }
        public string nonce { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string BCTenantId { get; set; }
        public string enableCallback { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public string extractedTenantId
        {
            get
            {
                return BCTenantId.Split('/')[4];
            }
        }
        public string code_verifier { get; set; }
        public string url
        {
            get
            {
                return "https://sandprod-admin.mobilepay.dk/account/connect/authorize";
            }
        }
        public string response_type
        {
            get
            {
                return "code%20id_token";
            }
        }
        public string client_id
        {
            get
            {
                return "mdcnordic";
            }
        }
        public string redirect_uri
        {
            get
            {
                return ConfigurationManager.AppSettings["RedirectUrl"].ToString();
            }
        }
        public string scope { get; set; }
        //{
        //    get
        //    {
        //        return "openid%20subscriptions%20offline_access";
        //    }
        //}
        public string code_challenge_method
        {
            get
            {
                return "S256";
            }
        }


    }
}