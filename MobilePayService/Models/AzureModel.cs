using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MobilePayService.Models
{
    public class AzureModel
    {
        public string scope { get; set; }
        public string AzClientSecret { get; set; }
        public string AzTenantId { get; set; }
        public string AzSoapUrl { get; set; }
        public string AzureLoginUrl
        {
            get
            {
                return "https://login.microsoftonline.com";
            }
        }
        public string prompt {
            get {
                return "consent";
            }
        }
        public string response_mode
        {
            get
            {
                return "code";
            }
        }
        public string AzClientId { get; set; }
        public string AzAccessToken { get; set; }
        public string AzRefreshToken { get; set; }
        public string AzTokenUpdateTime { get; set; }
        public string redirect_uri
        {
            get
            {
                return ConfigurationManager.AppSettings["RedirectUrl"].ToString();
            }
        }
    }
}