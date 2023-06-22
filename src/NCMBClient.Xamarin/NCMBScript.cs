using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NCMBClient
{
    public class NCMBScript
    {
        public Dictionary<string, string> _Headers = new Dictionary<string, string>();
        public JObject _Queries = new JObject();
        public JObject _Bodies = new JObject();
        public string _scriptName;

        public NCMBScript(string scriptName)
        {
            _scriptName = scriptName;
        }

        public NCMBScript Header(string name, string value)
        {
            _Headers.Add(name, value);
            return this;
        }
        public NCMBScript Body(string name, JObject value)
        {
            _Bodies[name] = value;
            return this;
        }
        public NCMBScript Body(string name, JArray value)
        {
            _Bodies[name] = value;
            return this;
        }
        public NCMBScript Body(string name, int value)
        {
            _Bodies[name] = value;
            return this;
        }
        public NCMBScript Body(string name, string value)
        {
            _Bodies[name] = value;
            return this;
        }
        public NCMBScript Query(string name, string value)
        {
            _Queries[name] = value;
            return this;
        }

        public NCMBRequest GetRequest(string method)
        {
            var r = new NCMBRequest();
            r.Name = _scriptName;
            r.Method = method;
            r.Headers = _Headers;
            r.isScript = true;
            r.Queries = _Queries;
            if (method == "POST" || method == "PUT") {
                r.Fields = (JObject) _Bodies.DeepClone();
            }
            return r;
        }

        public async Task<JObject> Get()
        {
            var r = GetRequest("GET");
            return await r.Exec();
        }

        public async Task<Byte[]> GetByte()
        {
            var r = GetRequest("GET");
            return await r.ExecByte();
        }

        public async Task<JObject> Post()
        {
            var r = GetRequest("POST");
            return await r.Exec();
        }

        public async Task<Byte[]> PostByte()
        {
            var r = GetRequest("POST");
            return await r.ExecByte();
        }

        public async Task<JObject> Put()
        {
            var r = GetRequest("PUT");
            return await r.Exec();
        }

        public async Task<Byte[]> PutByte()
        {
            var r = GetRequest("PUT");
            return await r.ExecByte();
        }

        public async Task<JObject> Delete()
        {
            var r = GetRequest("DELETE");
            return await r.Exec();
        }

        public async Task<Byte[]> DeleteByte()
        {
            var r = GetRequest("DELETE");
            return await r.ExecByte();
        }
    }
}
