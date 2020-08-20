using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

namespace NCMBClient
{
    public class NCMBObject
    {
        public string Name { get; }
        private JObject _fields;
        private Dictionary<string, object> _objects;
        private NCMBAcl _acl;
        public NCMB _ncmb;
        
        public NCMBObject(NCMB ncmb, string name)
        {
            _ncmb = ncmb;
            this.Name = name;
            _fields = new JObject();
            _objects = new Dictionary<string, object>();
        }

        public void Set(string key, string value)
        {
            _fields[key] = value;
        }

        public void Set(string key, int value)
        {
            _fields[key] = value;
        }

        public void Set(string key, DateTime value)
        {
            _fields[key] = value;
        }


        public void Set(string key, bool value)
        {
            _fields[key] = value;
        }

        public void Set(string key, JObject value)
        {
            _fields[key] = value;
        }
        public void Set(string key, JArray value)
        {
            _fields[key] = value;
        }
        public void Set(string key, NCMBObject value)
        {
            _objects.Add(key, value);
        }
        public void SetAcl(NCMBAcl acl)
        {
            _acl = acl;
        }

        public void Remove(string key)
        {
            if (_fields.ContainsKey(key))
            {
                _fields.Remove(key);
            }
            
        }

        public object Get(string key)
        {
            return _objects.ContainsKey(key) ? _objects[key] : _fields.GetValue(key);
        }

        public JObject GetAllFields()
        {
            return _fields;
        }

        public T Get<T>(string key) => (T)Get(key);

        public void Sets(JObject query)
        {
            foreach (KeyValuePair<string, JToken> key in query)
            {
                switch (key.Value.Type)
                {
                    case JTokenType.String:
                        this.Set(key.Key, (string)key.Value);
                        break;
                    case JTokenType.Integer:
                        this.Set(key.Key, (int)key.Value);
                        break;
                    case JTokenType.Boolean:
                        this.Set(key.Key, (Boolean)key.Value);
                        break;
                    case JTokenType.Date:
                        this.Set(key.Key, (DateTime)key.Value);
                        break;
                    case JTokenType.Array:
                        this.Set(key.Key, (JArray) key.Value);
                        break;
                    case JTokenType.Null:
                        this._fields[key.Key] = null;
                        break;
                    default:
                        var obj = (JObject)key.Value;
                        if (obj.ContainsKey("__type") && ((string) obj["__type"]) == "Date")
                        {
                            this.Set(key.Key, DateTime.Parse((string) obj["iso"]));
                        } else
                        {
                            this.Set(key.Key, (JObject)key.Value);
                        }
                        break;
                }
            }
        }

        public Boolean Save()
        {
            var r = GetRequest();
            var response = r.Exec();
            if (response.ContainsKey("error")) {
                return false;
            }
            Sets(response);
            return true;
        }

        public NCMBRequest GetRequest()
        {
            NCMBRequest r = new NCMBRequest(_ncmb);
            r.Name = Name;
            r.Fields = GetData();
            if (_fields.ContainsKey("objectId"))
            {
                r.Method = "PUT";
                r.ObjectId = ObjectId();
            }
            else
            {
                r.Method = "POST";
            }
            return r;
        }

        public async Task<NCMBObject> SaveAsync()
        {
            var r = GetRequest();
            var response = await r.ExecAsync();
            Sets(response);
            return this;
        }

        public bool Delete()
        {
            NCMBRequest r = new NCMBRequest(_ncmb);
            r.Method = "DELETE";
            r.Name = Name;
            r.ObjectId = ObjectId();
            var response = r.Exec();
            return response.Count == 0;
        }

        public async Task<bool> DeleteAsync()
        {
            NCMBRequest r = new NCMBRequest(_ncmb);
            r.Method = "DELETE";
            r.Name = Name;
            r.ObjectId = ObjectId();
            var response = await r.ExecAsync();
            return response.Count == 0;
        }

        public String ObjectId()
        {
            return (string)_fields.GetValue("objectId");
        }

        public async Task<bool> FetchAsync()
        {
            NCMBRequest r = new NCMBRequest(_ncmb);
            r.Method = "GET";
            r.Name = Name;
            r.ObjectId = ObjectId();
            var response = await r.ExecAsync();
            Sets(response);
            return true;
        }

        public bool Fetch()
        {
            NCMBRequest r = new NCMBRequest(_ncmb);
            r.Method = "GET";
            r.Name = Name;
            r.ObjectId = ObjectId();
            var response = r.Exec();
            Sets(response);
            return true;
        }

        public JObject GetData()
        {
            var results = new JObject();
            foreach (var key in _objects)
            {
                var data = new JObject();
                var obj = (NCMBObject) key.Value;
                data["__type"] = "Pointer";
                data.Add("className", obj.Name);
                var objectId = obj.Get("objectId");
                data.Add("objectId", objectId.ToString());
                results[key.Key] = data;
            }
            foreach (KeyValuePair<string, JToken> key in _fields)
            {
                results[key.Key] = key.Value;
            }
            if (_acl != null)
            {
                results["acl"] = _acl.JObject();
            }
            return (JObject) results.DeepClone();
        }
    }
}
