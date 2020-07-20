using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ncmb_xamarin
{
    public class NCMBSignature
    {
        private string _fqdn = "mbaas.api.nifcloud.com";
        private string _signatureMethod = "HmacSHA256";
        private string _signatureVersion = "2";
        private string _version = "2013-09-01";
        private string _application_key;
        private string _client_key;

        private Hashtable _base_info = new Hashtable()
        {
            {"SignatureVersion", ""},
            {"SignatureMethod", ""},
            {"X-NCMB-Application-Key", ""},
            {"X-NCMB-Timestamp", ""}
        };

        public NCMBSignature(string app_key, string cli_key)
        {
            _application_key = app_key;
            _client_key = cli_key;
            _base_info["SignatureVersion"] = _signatureVersion;
            _base_info["SignatureMethod"] = _signatureMethod;
            _base_info["X-NCMB-Application-Key"] = app_key;
        }

        private string path(string class_name, string objectId = null, string definePath = null) {
            string path = $"/{_version}";
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

        public string url(string class_name, string objectId = null, JObject queries = null, string definePath = null)
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
            return $"https://{_fqdn}{path(class_name, objectId, definePath)}{queryString}";
        }

        public string generate(string method, string class_name, DateTime time, string objectId = null, JObject queries = null, string definePath = null)
        {
            
            _base_info["X-NCMB-Timestamp"] = time.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            
            var sigList = new List<string>();
            if (queries != null)
            {
                foreach (KeyValuePair <string, JToken> key in queries)
                {
                    var obj = key.Value.ToString(Newtonsoft.Json.Formatting.None);
                    if (obj == "{}") continue;
                    _base_info.Add(key.Key, Uri.EscapeDataString(obj));
                }
                
            }
            var keys = new ArrayList(_base_info.Keys);
            keys.Sort(StringComparer.Ordinal);
            foreach (string key in keys)
            {
                sigList.Add($"{key}={_base_info[key]}");
            }
            var queryString = String.Join("&", sigList);
            var str = String.Join("\n", new[]{
                method,
                _fqdn,
                path(class_name, objectId, definePath),
                queryString
            });
            var hmacSha256 = new HMACSHA256(Encoding.Default.GetBytes(_client_key));
            var hash = hmacSha256.ComputeHash(Encoding.Default.GetBytes(str));
            return Convert.ToBase64String(hash);
        }
    }
}
