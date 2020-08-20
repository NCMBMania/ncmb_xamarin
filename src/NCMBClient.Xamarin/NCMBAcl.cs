using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace NCMBClient
{
    public class NCMBAcl
    {
        private JObject _fields;

        public NCMBAcl()
        {
            _fields = new JObject();
            var p = new JObject();
            p.Add("read", true);
            p.Add("write", true);
            _fields.Add("*", p);
        }

        public void SetPublicReadAccess(Boolean bol)
        {
            SetPermission("*", "read", bol);
        }
        public void SetPublicWriteAccess(Boolean bol)
        {
            SetPermission("*", "write", bol);
        }
        public void SetUserReadAccess(NCMBUser user, Boolean bol)
        {
            SetPermission(user.ObjectId(), "read", bol);
        }
        public void SetUserWriteAccess(NCMBUser user, Boolean bol)
        {
            SetPermission(user.ObjectId(), "write", bol);
        }
        public void SetRoleReadAccess(String name, Boolean bol)
        {
            SetPermission($"role:{name}", "read", bol);
        }
        public void SetRoleWriteAccess(String name, Boolean bol)
        {
            SetPermission($"role:{name}", "write", bol);
        }

        private void SetPermission(String key, String type, Boolean bol)
        {
            var p = new JObject();
            if (_fields.ContainsKey(key))
            {
                p = (JObject)_fields["*"];
            }
            p[type] = bol;
            _fields[key] = p;
        }

        public JObject JObject()
        {
            var obj = new JObject();
            foreach (KeyValuePair<string, JToken> key in _fields)
            {
                var p = (JObject)key.Value;
                var v = new JObject();
                String[] types = { "read", "write" };
                foreach (String k in types)
                {
                    if (p.ContainsKey(k) && (Boolean)p[k] == true)
                    {
                        v.Add(k, true);
                    }
                }
                obj.Add(key.Key, v);
            }
            return obj;
        }
    }
}
