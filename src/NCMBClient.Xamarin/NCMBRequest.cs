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

        public JObject Get(string name, JObject queries)
        {
            return Exec("GET", name, null, null, queries);
        }

        public JObject Post(string name, JObject fields)
        {
            return Exec("POST", name, fields);
        }

        public JObject Put(string name, string objectId, JObject fields)
        {
            return Exec("PUT", name, fields, objectId);
        }

        public bool Delete(string name, string objectId)
        {
            var response = Exec("DELETE", name, null, objectId);
            return response.Count == 0;
        }

        public JObject Exec(string method, string name, JObject fields = null, string objectId = null, JObject queries = null, string path = null)
        {
            var s = new NCMBSignature(_ncmb.ApplicationKey, _ncmb.ClientKey);
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

            var signature = s.Generate(method, name, time, objectId, queries, path);
            var url = s.Url(name, objectId, queries, path);
            var headers = new Hashtable()
            {
                { "X-NCMB-Application-Key", _ncmb.ApplicationKey },
                { "X-NCMB-Timestamp", time.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                { "X-NCMB-Signature", signature },
                { "Content-Type", "application/json" }
            };
            if (_ncmb.SessionToken != null)
            {
                headers.Add("X-NCMB-Apps-Session-Token", _ncmb.SessionToken);
            }

            var client = new WebClient();
            foreach(string key in headers.Keys)
            {
                client.Headers[key] = headers[key].ToString();
            }
            client.Encoding = Encoding.UTF8;

            Console.WriteLine(queries);
            var response = method == "GET" ? System.Text.Encoding.UTF8.GetString(client.DownloadData(url)) : client.UploadString(url, method, fields != null ? fields.ToString(): "");
            if (method == "DELETE" && response == "") return new JObject();
            return JObject.Parse(response);
        }
    }
}
