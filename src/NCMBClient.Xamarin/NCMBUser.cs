﻿using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

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
        
        public static async Task<NCMBUser> Login(string userName, string password)
        {
            var user = new NCMBUser();
            user.Set("userName", userName).Set("password", password);
            await user.Login();
            return user;
        }

        public async Task<Boolean> Login()
        {
            var r = GetLoginRequest();
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

        private NCMBRequest GetLoginRequest()
        {
            NCMBRequest r = new NCMBRequest();
            r.Name = Name;
            r.Queries = GetData();
            r.Method = "GET";
            r.Path = "login";
            return r;
        }
    }
}
