using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YogaStudioHelper.Models
{
    public class PaypalConfiguration
    {

        public readonly static string ClientId;
        public readonly static string ClientSecret;
        
        static PaypalConfiguration()
        {
            // TODO
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }

        public static Dictionary<string,string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties(); 

        }

        // Create access token 
        private static string GetAccessToken()
        {
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }


        //This will return api Context Object 
        public static APIContext GetAPIContext()
        {
            var apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }



    }
}