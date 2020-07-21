using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;

namespace NCMBClient
{
    public class NCMBObject
    {
        public string Name { get; }
        private JObject _fields;
        private Dictionary<string, object> _objects;
        private NCMB _ncmb;
        
        public NCMBObject(NCMB ncmb, string name)
        {
            _ncmb = ncmb;
            this.Name = name;
            _fields = new JObject();
            _objects = new Dictionary<string, object>();
        }

        public NCMBObject Set(string key, string value)
        {
            _fields[key] = value;
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

        public object Get(string key)
        {
            return _objects.ContainsKey(key) ? _objects[key] : _fields.GetValue(key);
        }

        public T Get<T>(string key) => (T)Get(key);

        public NCMBObject Sets(JObject query)
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
                        this.set(key.Key, (JArray) key.Value);
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
            return this;
        }

        public NCMBObject Save()
        {
            NCMBRequest r = new NCMBRequest(_ncmb);
            var response = _fields.ContainsKey("objectId") ?
                r.Put(Name, (string) _fields.GetValue("objectId"), GetData()) :
                r.Post(Name, GetData());
            Sets(response);
            return this;
        }

        public bool Delete()
        {
            NCMBRequest r = new NCMBRequest(_ncmb);
            return r.Delete(Name, (string)_fields.GetValue("objectId"));
        }

        private JObject GetData()
        {
            var results = new JObject();
            Console.WriteLine(_objects);
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
            return (JObject) results.DeepClone();
        }
    }
}
