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

        public NCMBAcl SetPublicReadAccess(Boolean bol)
        {
            SetPermission("*", "read", bol);
            return this;
        }
        public NCMBAcl SetPublicWriteAccess(Boolean bol)
        {
            SetPermission("*", "write", bol);
            return this;
        }
        public NCMBAcl SetUserReadAccess(NCMBUser user, Boolean bol)
        {
            SetPermission(user.ObjectId(), "read", bol);
            return this;
        }
        public NCMBAcl SetUserWriteAccess(NCMBUser user, Boolean bol)
        {
            SetPermission(user.ObjectId(), "write", bol);
            return this;
        }
        public NCMBAcl SetRoleReadAccess(String name, Boolean bol)
        {
            SetPermission($"role:{name}", "read", bol);
            return this;
        }
        public NCMBAcl SetRoleWriteAccess(String name, Boolean bol)
        {
            SetPermission($"role:{name}", "write", bol);
            return this;
        }

        public NCMBAcl SetPublicAccess(JObject obj)
        {
            if ((bool) obj["read"] == true)
            {
                this.SetPublicReadAccess(true);
            }
            if ((bool)obj["write"] == true)
            {
                this.SetPublicWriteAccess(true);
            }
            return this;
        }

        public NCMBAcl SetRoleAccess(string name, JObject obj)
        {
            name = name.Replace("role:", "");
            if ((bool)obj["read"] == true)
            {
                this.SetRoleReadAccess(name, true);
            }
            if ((bool)obj["write"] == true)
            {
                this.SetRoleWriteAccess(name, true);
            }
            return this;
        }

        public NCMBAcl SetUserAccess(string objectId, JObject obj)
        {
            var user = new NCMBUser();
            user.Set("objectId", objectId);
            if ((bool)obj["read"] == true)
            {
                this.SetUserReadAccess(user, true);
            }
            if ((bool)obj["write"] == true)
            {
                this.SetUserWriteAccess(user, true);
            }
            return this;
        }

        public NCMBAcl Sets(JObject query)
        {
            foreach (KeyValuePair<string, JToken> key in query)
            {
                if (key.Key == "*")
                {
                    this.SetPublicAccess((JObject) key.Value);
                }
                else if (key.Key.StartsWith("role:"))
                {
                    this.SetRoleAccess(key.Key, (JObject)key.Value);
                }
                else
                {
                    this.SetUserAccess(key.Key, (JObject)key.Value);
                }
            }
            return this;
        }

        private NCMBAcl SetPermission(String key, String type, Boolean bol)
        {
            var p = new JObject();
            if (_fields.ContainsKey(key))
            {
                p = (JObject)_fields["*"];
            }
            p[type] = bol;
            _fields[key] = p;
            return this;
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
