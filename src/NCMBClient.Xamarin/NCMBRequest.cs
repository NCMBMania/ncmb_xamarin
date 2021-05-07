using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Net;

namespace NCMBClient
{
    public class NCMBRequest
    {
        public String Method;
        public String Name;
        public JObject Fields;
        public String ObjectId;
        public JObject Queries;
        public String Path;
        public static NCMB _ncmb;

        public NCMBRequest()
        {
        }

        private String FieldToString()
        {
            if (Fields != null)
            {
                foreach (var key in new string[] { "objectId", "createDate", "updateDate" })
                {
                    if (Fields.ContainsKey(key))
                        Fields.Remove(key);
                }
            }
            if (Fields != null)
            {
                foreach (KeyValuePair<string, JToken> key in Fields)
                {
                    if (key.Value.Type == JTokenType.Date)
                    {
                        var date = new JObject();
                        date["__type"] = "Date";
                        date["iso"] = ((DateTime)key.Value).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        Fields[key.Key] = date;
                    }
                }
            }
            return Fields != null ? Fields.ToString() : "";
        }

        private Hashtable GetHeader(DateTime time, String signature)
        {
            var headers = new Hashtable()
            {
                { "X-NCMB-Application-Key", NCMBRequest._ncmb.ApplicationKey },
                { "X-NCMB-Timestamp", time.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                { "X-NCMB-Signature", signature },
                { "Content-Type", "application/json" }
            };
            if (_ncmb.SessionToken != null)
            {
                headers.Add("X-NCMB-Apps-Session-Token", NCMBRequest._ncmb.SessionToken);
            }
            return headers;
        }

        private WebClient GetClient(Hashtable headers)
        {
            var client = new WebClient();
            foreach (string key in headers.Keys)
            {
                client.Headers[key] = headers[key].ToString();
            }
            client.Encoding = Encoding.UTF8;
            return client;
        }

        public JObject Exec()
        {
            var s = new NCMBSignature(NCMBRequest._ncmb.ApplicationKey, NCMBRequest._ncmb.ClientKey);
            s.Method = Method;
            s.Name = Name;
            s.ObjectId = ObjectId;
            s.Queries = Queries;
            s.Path = Path;
            var signature = s.Generate();

            var headers = GetHeader(s.Time, signature);
            var client = GetClient(headers);
            var response = "";
            if (Method == "GET")
            {
                // Console.WriteLine(s.Url());
                response = System.Text.Encoding.UTF8.GetString(client.DownloadData(new Uri(s.Url())));
            }
            else
            {
                response = client.UploadString(s.Url(), Method, FieldToString());
            }
            if (Method == "DELETE" && response == "") return new JObject();
            return JObject.Parse(response);
        }

        public async Task<JObject> ExecAsync()
        {
            var s = new NCMBSignature(NCMBRequest._ncmb.ApplicationKey, NCMBRequest._ncmb.ClientKey);
            s.Method = Method;
            s.Name = Name;
            s.ObjectId = ObjectId;
            s.Queries = Queries;
            s.Path = Path;
            var signature = s.Generate();
            var headers = GetHeader(s.Time, signature);
            var client = GetClient(headers);
            var response = "";
            if (Method == "GET")
            {
                response = System.Text.Encoding.UTF8.GetString(await client.DownloadDataTaskAsync(new Uri(s.Url())));
            } else
            {
                response = await client.UploadStringTaskAsync(s.Url(), Method, FieldToString());
            }
            if (Method == "DELETE" && response == "") return new JObject();
            return JObject.Parse(response);
        }
    }
}
