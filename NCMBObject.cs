using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ncmb_xamarin
{
    public class NCMBObject
    {
        private string _name;
        private JObject _fields;
        private NCMB _ncmb;
        
        public NCMBObject(NCMB ncmb, string name)
        {
            _ncmb = ncmb;
            _name = name;
            _fields = new JObject();
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

        public JToken get(string key)
        {
            return _fields.GetValue(key);
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
                    default:
                        var obj = (JObject)key.Value;
                        Console.WriteLine(obj);
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
                r.put(_name, (string) _fields.GetValue("objectId"), (JObject) _fields.DeepClone()) :
                r.post(_name, (JObject)_fields.DeepClone());
            sets(response);
            return this;
        }
    }
}
