using IdentityModel;
using MobilePayService.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MobilePayService.Methods
{
    public static class AuthCodeMethod
    {
        public static void GetAccessTokenAsync(BCClientModel model, Action<BCClientModel> callback)
        {

            var codeVerifier = CryptoRandom.CreateUniqueId(32);
            string codeChallenge;
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                codeChallenge = Base64Url.Encode(challengeBytes);
            }
            model.nonce = CryptoRandom.CreateUniqueId(32);
            model.state = CryptoRandom.CreateUniqueId(32);
            model.code_challenge = codeChallenge;
            model.code_verifier = codeVerifier;
            callback(model);




        }
    }
}