using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Net;

namespace NCMBClient
{
    public class NCMBRequest
    {
        private NCMB _ncmb;
        public NCMBRequest(NCMB ncmb)
        {
            _ncmb = ncmb;
        }

        public JObject post(string name, JObject fields)
        {
            return exec("POST", name, fields);
        }

        public JObject put(string name, string objectId, JObject fields)
        {
            return exec("PUT", name, fields, objectId);
        }

        public Boolean delete(string name, string objectId)
        {
            var response = exec("DELETE", name, null, objectId);
            return response.Count == 0;
        }

        public JObject exec(string method, string name, JObject fields = null, string objectId = null, JObject queries = null, string path = null)
        {
            var s = new NCMBSignature(_ncmb.application_key, _ncmb.client_key);
            var time = DateTime.Now;
            if (fields != null)
            {
                foreach (var key in new string[]{ "objectId", "createDate", "updateDate"}) {
                    if (fields.ContainsKey(key))
                        fields.Remove(key);
                }
            }
            if (fields != null)
            {
                foreach (KeyValuePair<string, JToken> key in fields)
                {
                    if (key.Value.Type == JTokenType.Date)
                    {
                        var date = new JObject();
                        date["__type"] = "Date";
                        date["iso"] = ((DateTime)key.Value).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        fields[key.Key] = date;
                    }
                }
            }

            var signature = s.generate(method, name, time, objectId, queries, path);
            var url = s.url(name, objectId, queries, path);
            var headers = new Hashtable()
            {
                { "X-NCMB-Application-Key", _ncmb.application_key },
                { "X-NCMB-Timestamp", time.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                { "X-NCMB-Signature", signature },
                { "Content-Type", "application/json" }
            };
            if (_ncmb.sessionToken != null)
            {
                headers.Add("X-NCMB-Apps-Session-Token", _ncmb.sessionToken);
            }

            var client = new WebClient();
            foreach(string key in headers.Keys)
            {
                client.Headers[key] = headers[key].ToString();
            }
            client.Encoding = Encoding.UTF8;
            var response = client.UploadString(url, method, fields != null ? fields.ToString(): "");
            if (method == "DELETE" && response == "") return new JObject();
            return JObject.Parse(response);
        }
    }
}
