using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePayService.Models
{
    public class AccessTokenModel
    {
        public string grant_type
        {
            get
            {
                return "authorization_code";
            }
        }
        public string code { get; set; }
        public string redirect_uri
        {
            get
            {
                return "https://dev.mdcnordic.com:8443/mobilepay/redirect/";
            }
        }
        public string code_verifier { get; set; }
        public string client_id
        {
            get
            {
                return "mdcnordic";
            }
        }
        public string client_secret
        {
            get
            {
                return "Cvz4+Eguct3mkcAxlpRLCjOYiqtX0YM2Rlkd+XfGvbk=";
            }
        }
        public string id_token { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }

        public string state { get; set; }


    }
}