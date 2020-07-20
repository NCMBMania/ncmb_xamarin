using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;

namespace ncmb_xamarin
{
    public class NCMBObject
    {
        public string name;
        private JObject _fields;
        private Hashtable _objects;
        private NCMB _ncmb;
        
        public NCMBObject(NCMB ncmb, string name)
        {
            _ncmb = ncmb;
            this.name = name;
            _fields = new JObject();
            _objects = new Hashtable();
        }

        public NCMBObject set(string key, string value)
        {
            _fields[key] = value;
            return this;
        }

        public NCMBObject set(string key, int value)
        {
            _fields[key] = value;
            return this;
        }

        public NCMBObject set(string key, DateTime value)
        {
            _fields[key] = value;
            return this;
        }


        public NCMBObject set(string key, Boolean value)
        {
            _fields[key] = value;
            return this;
        }

        public NCMBObject set(string key, JObject value)
        {
            _fields[key] = value;
            return this;
        }
        public NCMBObject set(string key, JArray value)
        {
            _fields[key] = value;
            return this;
        }
        public NCMBObject set(string key, NCMBObject value)
        {
            _objects.Add(key, value);
            return this;
        }
        public Object get(string key)
        {
            return _objects.ContainsKey(key) ? _objects[key] : _fields.GetValue(key);
        }

        public NCMBObject sets(JObject query)
        {
            foreach (KeyValuePair<string, JToken> key in query)
            {
                switch (key.Value.Type)
                {
                    case JTokenType.String:
                        this.set(key.Key, (string)key.Value);
                        break;
                    case JTokenType.Integer:
                        this.set(key.Key, (int)key.Value);
                        break;
                    case JTokenType.Boolean:
                        this.set(key.Key, (Boolean)key.Value);
                        break;
                    case JTokenType.Date:
                        this.set(key.Key, (DateTime)key.Value);
                        break;
                    case JTokenType.Array:
                        this.set(key.Key, (JArray) key.Value);
                        break;
                    default:
                        var obj = (JObject)key.Value;
                        if (obj.ContainsKey("__type") && ((string) obj["__type"]) == "Date")
                        {
                            this.set(key.Key, DateTime.Parse((string) obj["iso"]));
                        } else
                        {
                            this.set(key.Key, (JObject)key.Value);
                        }
                        break;
                }
            }
            return this;
        }

        public NCMBObject save()
        {
            NCMBRequest r = new NCMBRequest(_ncmb);
            var response = _fields.ContainsKey("objectId") ?
                r.put(name, (string) _fields.GetValue("objectId"), getData()) :
                r.post(name, getData());
            sets(response);
            return this;
        }

        public Boolean delete()
        {
            NCMBRequest r = new NCMBRequest(_ncmb);
            return r.delete(name, (string)_fields.GetValue("objectId"));
        }

        private JObject getData()
        {
            var results = new JObject();
            Console.WriteLine(_objects);
            foreach (DictionaryEntry key in _objects)
            {
                var data = new JObject();
                var obj = (NCMBObject) key.Value;
                data["__type"] = "Pointer";
                data.Add("className", obj.name);
                var objectId = obj.get("objectId");
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
