using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace NCMBClient
{
    public class NCMBRequest
    {
        public string Method;
        public string Name;
        public JObject Fields;
        public string ObjectId;
        public JObject Queries;
        public string Path;
        public static NCMB _ncmb;
        public byte[] Data;
        public string MimeType;
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public bool isScript = false;

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
                        var date = new JObject
                        {
                            ["__type"] = "Date",
                            ["iso"] = ((DateTime)key.Value).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                        };
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
                /* { "Content-Type", "application/json" } */
            };
            if (_ncmb.SessionToken != null)
            {
                headers.Add("X-NCMB-Apps-Session-Token", _ncmb.SessionToken);
            }
            if (isScript)
            {
                foreach (KeyValuePair<string, string> item in Headers)
                {
                    headers.Add(item.Key, item.Value);
                }
            }
            return headers;
        }

        private HttpMethod GetMethod(string method)
        {
            if (method == "POST") return HttpMethod.Post;
            if (method == "PUT") return HttpMethod.Put;
            if (method == "DELETE") return HttpMethod.Delete;
            return HttpMethod.Get;
        }

        public async Task<JObject> Exec()
        {
            var response = await GetResponse();
            var body = await response.Content.ReadAsStringAsync();
            if (Method == "DELETE" && body == "") return new JObject();
            var result = JObject.Parse(body);
            if (result.ContainsKey("error") && result.ContainsKey("code"))
            {
                throw new Exception($"NCMB Error {result.GetValue("code")} {result.GetValue("error")}");
            }
            return result;
        }

        public async Task<byte[]> ExecByte()
        {
            var response = await GetResponse();
            return await response.Content.ReadAsByteArrayAsync();
        }

        private Task<HttpResponseMessage> GetResponse()
        {
            var s = new NCMBSignature(NCMBRequest._ncmb.ApplicationKey, NCMBRequest._ncmb.ClientKey, isScript);
            s.Method = Method;
            s.Name = Name;
            s.ObjectId = ObjectId;
            s.Queries = Queries;
            s.Path = Path;

            var signature = s.Generate();

            var headers = GetHeader(s.Time, signature);
            var client = new HttpClient();
            var request = new HttpRequestMessage(GetMethod(Method), s.Url());
            foreach (string key in headers.Keys)
            {
                // Console.WriteLine($"{key} {headers[key].ToString()}");
                request.Headers.Add(key, headers[key].ToString());
            }
            if (!(Data is null))
            {
                // File upload
                var multipart = new MultipartFormDataContent();
                var file = new ByteArrayContent(Data);
                file.Headers.ContentType = new MediaTypeHeaderValue(MimeType);
                multipart.Add(file, "file", ObjectId);
                if (Fields != null && Fields.ContainsKey("acl") && Fields["acl"] != null)
                {
                    multipart.Add(new StringContent(Fields["acl"].ToString()), "acl");
                }
                request.Content = multipart;
            }
            else
            {
                if (Method == "POST" || Method == "PUT")
                {
                    request.Content = new StringContent(FieldToString(), Encoding.UTF8, "application/json");
                }
            }
            return client.SendAsync(request);
        }
    }
}
