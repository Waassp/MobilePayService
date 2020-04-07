using IdentityModel;
using MobilePayService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MobilePayService.Methods
{
    public static class AuthCodeMethod
    {
        public static void GetAccessTokenAsync(BCClientModel model,Action<BCClientModel> callback)
        {
           
                var codeVerifier = CryptoRandom.CreateUniqueId(32);
                string codeChallenge;
                using (var sha256 = SHA256.Create())
                {
                    var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                    codeChallenge = Base64Url.Encode(challengeBytes);
                }

                //IdentityModel.Client.DiscoveryPolicy discoveryPolicy = new IdentityModel.Client.DiscoveryPolicy();
                //discoveryPolicy.ValidateEndpoints = false;
                //discoveryPolicy.ValidateIssuerName = false;
                //IdentityModel.OidcClient.Policy policy = new IdentityModel.OidcClient.Policy();
                //policy.Discovery = discoveryPolicy;
                //IdentityModel.OidcClient.OidcClientOptions oidcClientOptions = new IdentityModel.OidcClient.OidcClientOptions();
                //oidcClientOptions.Authority = "https://sandprod-admin.mobilepay.dk/account/.well-known/openid-configuration";
                //oidcClientOptions.ClientId = "mdcnordic";
                //oidcClientOptions.ClientSecret = "Cvz4+Eguct3mkcAxlpRLCjOYiqtX0YM2Rlkd+XfGvbk=";
                //oidcClientOptions.Scope = "openid invoice subscriptions offline_access";
                //oidcClientOptions.RedirectUri = "http://dev.mdcnordic.com:8443/mobilepay/redirect/";
                //oidcClientOptions.Policy = policy;
                //oidcClientOptions.LoadProfile = false;

                //IdentityModel.OidcClient.OidcClient oidcClient = new IdentityModel.OidcClient.OidcClient(oidcClientOptions);
                //var state = await oidcClient.PrepareLoginAsync().ConfigureAwait(false);                
                model.nonce = CryptoRandom.CreateUniqueId(32);
                model.state = CryptoRandom.CreateUniqueId(32);
                model.code_challenge = codeChallenge;
                model.code_verifier = codeVerifier;
                callback(model);

            
            

        }
    }
}