using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NCMBClient
{
    public class NCMBSignature
    {
        private const string Fqdn = "mbaas.api.nifcloud.com";
        private const string SignatureMethod = "HmacSHA256";
        private const string SignatureVersion = "2";
        private const string Version = "2013-09-01";
        private readonly string _applicationKey;
        private readonly string _clientKey;
        public String Method;
        public String Name;
        public DateTime Time;
        public String ObjectId;
        public JObject Queries;
        public String Path;

        private readonly Dictionary<string, string> _baseInfo = new Dictionary<string, string>()
        {
            {"SignatureVersion", ""},
            {"SignatureMethod", ""},
            {"X-NCMB-Application-Key", ""},
            {"X-NCMB-Timestamp", ""}
        };

        public NCMBSignature(string applicationKey, string clientKey)
        {
            _applicationKey = applicationKey;
            _clientKey = clientKey;
            _baseInfo["SignatureVersion"] = SignatureVersion;
            _baseInfo["SignatureMethod"] = SignatureMethod;
            _baseInfo["X-NCMB-Application-Key"] = applicationKey;
            Time = DateTime.Now;
        }

        // private string GetPath(string class_name, string objectId = null, string definePath = null) {
        private string GetPath() {
            string path = $"/{Version}";
            if (Path != null) {
                return $"{path}/{Path}";
            }
            var defined = new List<string> { "users", "push", "roles", "files", "installations" };

            if (defined.IndexOf(Name) > -1) {
                path = $"{path}/{Name}";
            } else {
                path = $"{path}/classes/{Name}";
            }
            if (ObjectId != null)
                path = $"{path}/{ObjectId}";
            return path;
        }

        private string GetEscapeValue(JToken value)
        {
            switch (value.Type)
            {
                case JTokenType.String:
                case JTokenType.Integer:
                case JTokenType.Boolean:
                    return value.ToString();
                default:
                    var str = value.ToString(Newtonsoft.Json.Formatting.None);
                    if (str == "{}") return null;
                    return str;
            }

        }
        public string Url()
        {
            var queryList = new List<string>();
            if (Queries != null && Queries.Count >= 0)
            {
                foreach (KeyValuePair<string, JToken> key in Queries)
                {
                    var value = GetEscapeValue(key.Value);
                    if (value != null)
                    {
                        queryList.Add($"{key.Key}={Uri.EscapeDataString(value)}");
                    } 
                }

            }
            var queryString = queryList.Count == 0 ? "" : $"?{String.Join("&", queryList)}";
            return $"https://{Fqdn}{GetPath()}{queryString}";
        }

        public string Generate()
        {
            
            _baseInfo["X-NCMB-Timestamp"] = Time.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            
            var sigList = new List<string>();
            if (Queries != null)
            {
                foreach (KeyValuePair <string, JToken> key in Queries)
                {
                    var value = GetEscapeValue(key.Value);
                    if (value != null)
                    {
                        _baseInfo.Add(key.Key, Uri.EscapeDataString(value));
                    }
                }
                
            }
            var keys = new ArrayList(_baseInfo.Keys);
            keys.Sort(StringComparer.Ordinal);
            foreach (string key in keys)
            {
                sigList.Add($"{key}={_baseInfo[key]}");
            }
            var queryString = String.Join("&", sigList);
            
            var str = String.Join("\n", new[]{
                Method,
                Fqdn,
                GetPath(),
                queryString
            });
            var hmacSha256 = new HMACSHA256(Encoding.Default.GetBytes(_clientKey));
            var hash = hmacSha256.ComputeHash(Encoding.Default.GetBytes(str));
            return Convert.ToBase64String(hash);
        }
    }
}
