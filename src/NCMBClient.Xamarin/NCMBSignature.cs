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
        }

        private string Path(string class_name, string objectId = null, string definePath = null) {
            string path = $"/{Version}";
            if (definePath != null) {
                return $"{path}/{definePath}";
            }
            var defined = new List<string> { "users", "push", "role", "files", "installations" };

            if (defined.IndexOf(class_name) > -1) {
                path = $"{path}/{class_name}";
            } else {
                path = $"{path}/classes/{class_name}";
            }
            if (objectId != null)
                path = $"{path}/{objectId}";
            return path;
        }

        public string Url(string class_name, string objectId = null, JObject queries = null, string definePath = null)
        {
            var queryList = new List<string>();
            if (queries != null && queries.Count >= 0)
            {
                foreach (KeyValuePair<string, JToken> key in queries)
                {
                    var str = key.Value.ToString(Newtonsoft.Json.Formatting.None);
                    if (str == "{}") continue;
                    queryList.Add($"{key.Key}={Uri.EscapeDataString(str)}");
                }

            }
            var queryString = queryList.Count == 0 ? "" : $"?{String.Join("&", queryList)}";
            return $"https://{Fqdn}{Path(class_name, objectId, definePath)}{queryString}";
        }

        public string Generate(string method, string class_name, DateTime time, string objectId = null, JObject queries = null, string definePath = null)
        {
            
            _baseInfo["X-NCMB-Timestamp"] = time.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            
            var sigList = new List<string>();
            if (queries != null)
            {
                foreach (KeyValuePair <string, JToken> key in queries)
                {
                    var obj = key.Value.ToString(Newtonsoft.Json.Formatting.None);
                    if (obj == "{}") continue;
                    _baseInfo.Add(key.Key, Uri.EscapeDataString(obj));
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
                method,
                Fqdn,
                Path(class_name, objectId, definePath),
                queryString
            });
            var hmacSha256 = new HMACSHA256(Encoding.Default.GetBytes(_clientKey));
            var hash = hmacSha256.ComputeHash(Encoding.Default.GetBytes(str));
            return Convert.ToBase64String(hash);
        }
    }
}
