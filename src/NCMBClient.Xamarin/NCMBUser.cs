using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;

namespace NCMBClient
{
    public class NCMBUser : NCMBObject
    {
        public NCMBUser() : base("users")
        {
        }

        static public void Logout()
        {
            _ncmb.SessionToken = null;
        }

        public async Task<NCMBUser> SignUp()
        {
            await this.Save();
            ToSession();
            return this;
        }

        public static async Task<NCMBUser> SignUp(string userName, string password)
        {
            var user = new NCMBUser();
            user.Set("userName", userName).Set("password", password);
            await user.SignUp();
            return user;
        }

        public static async Task<Boolean> RequestAuthenticationMail(string mailAddress)
        {
            var r = new NCMBRequest();
            r.Name = "users";
            r.Method = "POST";
            r.Path = "requestMailAddressUserEntry";
            var data = new JObject
            {
                ["mailAddress"] = mailAddress
            };
            r.Fields = data;
            await r.Exec();
            return true;
        }

        public static async Task<NCMBUser> Login(string userName, string password)
        {
            var user = new NCMBUser();
            user.Set("userName", userName).Set("password", password);
            await user.Login();
            return user;
        }

        public static async Task<NCMBUser> LoginWithMailAddress(string mailAddress, string password)
        {
            var user = new NCMBUser();
            user.Set("mailAddress", mailAddress).Set("password", password);
            await user.Login();
            return user;
        }

        public static async Task<NCMBUser> SignUpWithTwitter(string id, string screenName, string oauthConsumerKey, string consumerSecret, string oauthToken, string oauthTokenSecret)
        {
            var data = new JObject {
                ["id"] = id,
                ["screen_name"] = screenName,
                ["oauth_consumer_key"] = oauthConsumerKey,
                ["consumer_secret"] = consumerSecret,
                ["oauth_token"] = oauthToken,
                ["oauth_token_secret"] = oauthTokenSecret
            };
            return await SignUpWith("twitter", data);
        }

        public static async Task<NCMBUser> SignUpWithFacebook(string id, string accessToken, DateTime expirationDate)
        {
            var data = new JObject {
                ["id"] = id,
                ["access_token"] = accessToken
            };
            data["expiration_date"] = new JObject
            {
                ["__type"] = "Date",
                ["iso"] = expirationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
            return await SignUpWith("facebook", data);
        }

        public static async Task<NCMBUser> SignUpWithGoogle(string id, string accessToken)
        {
            var data = new JObject
            {
                ["id"] = id,
                ["access_token"] = accessToken
            };
            return await SignUpWith("google", data);
        }

        public static async Task<NCMBUser> SignUpWithApple(string id, string accessToken, string clientId)
        {
            var data = new JObject
            {
                ["id"] = id,
                ["access_token"] = accessToken,
                ["client_id"] = clientId
            };
            return await SignUpWith("apple", data);
        }

        public static async Task<NCMBUser> SignUpWithAnonymous(string id = null)
        {
            var data = new JObject
            {
                ["id"] = id ?? Guid.NewGuid().ToString()
            };
            return await SignUpWith("anonymous", data);
        }

        public static async Task<NCMBUser> SignUpWith(string provider, JObject authData)
        {
            var r = new NCMBRequest();
            r.Name = "users";
            r.Method = "POST";
            r.Path = "users";
            var data = new JObject
            {
                ["authData"] = new JObject
                {
                    [provider] = authData
                }
            };
            r.Fields = data;
            var response = await r.Exec();
            var user = new NCMBUser();
            user.Sets(response);
            if (response.ContainsKey("sessionToken"))
            {
                user.ToSession();
            }
            return user;
        }

        public async Task<Boolean> Login()
        {
            var r = new NCMBRequest();
            r.Name = Name;
            r.Fields = GetData();
            r.Method = "POST";
            r.Path = "login";
            var response = await r.Exec();
            if (response.ContainsKey("sessionToken"))
            {
                Sets(response);
                ToSession();
                return true;
            }
            return false;
        }

        private void ToSession()
        {
            NCMBUser._ncmb.SessionToken = this.Get("sessionToken").ToString();
            this.Remove("sessionToken");
        }

        public static NCMBQuery Query()
        {
            return new NCMBQuery("users");
        }

        
        public new Task<NCMBObject> Save()
        {
            if (base._fields.ContainsKey("objectId")) {
                var ary = new string[3] { "mailAddress", "password", "mailAddressConfirm" };
                foreach (var key in ary)
                {
                    if (base._fields.ContainsKey(key))
                    {
                        base._fields.Remove(key);
                    }
                }
            }
            return base.Save();
        }
    }
}
