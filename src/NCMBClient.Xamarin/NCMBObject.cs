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
        public JObject _fields;
        private Dictionary<string, object> _objects;
        private NCMBAcl _acl;
        public static NCMB _ncmb;
        
        public NCMBObject(string name)
        {
            this.Name = name;
            _fields = new JObject();
            _objects = new Dictionary<string, object>();
        }

        public NCMBObject Set(string key, string value)
        {
            _fields[key] = value;
            var type = this.GetType();
            return this;
        }

        public NCMBObject Set(string key, int value)
        {
            _fields[key] = value;
            return this;
        }

        public NCMBObject Set(string key, DateTime value)
        {
            _fields[key] = value;
            return this;
        }


        public NCMBObject Set(string key, bool value)
        {
            _fields[key] = value;
            return this;
        }

        public NCMBObject Set(string key, JObject value)
        {
            _fields[key] = value;
            return this;
        }
        public NCMBObject Set(string key, JArray value)
        {
            _fields[key] = value;
            return this;
        }
        public NCMBObject Set(string key, NCMBObject value)
        {
            _objects.Add(key, value);
            return this;
        }
        public NCMBObject SetAcl(NCMBAcl acl)
        {
            _acl = acl;
            return this;
        }
        public NCMBObject Set(string key, NCMBRelation value)
        {
            if (_objects.ContainsKey(key))
            {
                _objects.Remove(key);
            }
            _objects.Add(key, value);
            return this;
        }
        public NCMBObject Set(string key, NCMBGeoPoint value)
        {
            if (_objects.ContainsKey(key))
            {
                _objects.Remove(key);
            }
            _objects.Add(key, value);
            return this;
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

        public string GetString(string key)
        {
            return (string) _fields.GetValue(key);
        }

        public int GetInt(string key)
        {
            var val = _fields.GetValue(key);
            var i = Convert.ToInt32(val);
            return i;
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
                if (key.Key == "__type") continue;
                if (key.Key == "className") continue;
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
                        if (obj.ContainsKey("__type") && ((string)obj["__type"]) == "Date")
                        {
                            this.Set(key.Key, DateTime.Parse((string)obj["iso"]));
                        }
                        else if (obj.ContainsKey("__type") && ((string)obj["__type"]) == "Object")
                        {
                            var ChildObject = new NCMBObject((string)obj["className"]);
                            ChildObject.Sets((JObject)obj);
                            this.Set(key.Key, ChildObject);
                        }
                        else if (obj.ContainsKey("__type") && ((string)obj["__type"]) == "Relation")
                        {
                            // Do nothing
                        }
                        else if (obj.ContainsKey("__type") && ((string)obj["__type"]) == "GeoPoint")
                        {
                            var data = (JObject)key.Value;
                            var geo = new NCMBGeoPoint((double) data["latitude"], (double) data["longitude"]);
                            this.Set(key.Key, geo);
                        }
                        else if (key.Key == "acl")
                        {
                            var acl = new NCMBAcl();
                            acl.Sets((JObject)key.Value);
                            this.SetAcl(acl);
                        }
                        else
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
            NCMBRequest r = new NCMBRequest();
            r.Name = Name;
            r.Fields = GetData();
            // Console.WriteLine(_fields);
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
            NCMBRequest r = new NCMBRequest();
            r.Method = "DELETE";
            r.Name = Name;
            r.ObjectId = ObjectId();
            var response = r.Exec();
            return response.Count == 0;
        }

        public async Task<bool> DeleteAsync()
        {
            NCMBRequest r = new NCMBRequest();
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
            NCMBRequest r = new NCMBRequest();
            r.Method = "GET";
            r.Name = Name;
            r.ObjectId = ObjectId();
            var response = await r.ExecAsync();
            Sets(response);
            return true;
        }

        public bool Fetch()
        {
            NCMBRequest r = new NCMBRequest();
            r.Method = "GET";
            r.Name = Name;
            r.ObjectId = ObjectId();
            var response = r.Exec();
            Sets(response);
            return true;
        }

        public JObject ToPointer()
        {
            var data = new JObject();
            data["__type"] = "Pointer";
            switch (this.Name)
            {
                case "users":
                    data["className"] = "user";
                    break;
                case "roles":
                    data["className"] = "role";
                    break;
                default:
                    data["className"] = this.Name;
                    break;
            }
            data["objectId"] = this.Get("objectId").ToString();
            return data;
        }

        public JObject GetData()
        {
            var results = new JObject();
            foreach (var key in _objects)
            {
                var data = new JObject();
                var type = key.Value.GetType();
                if (type.Equals(typeof(NCMBObject)))
                {
                    results[key.Key] = ((NCMBObject) key.Value).ToPointer();
                } else if (type.Equals(typeof(NCMBRelation)))
                {
                    results[key.Key] = ((NCMBRelation)key.Value).ToJson();
                }
                else if (type.Equals(typeof(NCMBGeoPoint)))
                {
                    results[key.Key] = ((NCMBGeoPoint)key.Value).ToJson();
                }
                
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
