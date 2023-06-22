using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBScriptTest: Test
    {
        public NCMBScriptTest(): base()
        {
        }

        [Test()]
        public void TestScriptGet()
        {
            Task.Run(async () =>
            {
                var script = new NCMBScript("script_test_get.js");
                script.Header("hoge1", "fuga1");
                script.Header("hoge2", "fuga2");
                var strings = new JArray("val1", "val2", "val3");
                script.Body("hoge1", strings);
                script.Query("hoge3", "fuga3");
                var response = await script.Get();
                // TestContext.WriteLine(response.ToString());
                var query = (JObject)response["query"];
                Assert.AreEqual(query["hoge3"].ToString(), "fuga3");
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestScriptPost()
        {
            Task.Run(async () =>
            {
                var script = new NCMBScript("script_test_post.js");
                script.Header("hoge1", "fuga1");
                script.Header("hoge2", "fuga2");
                var strings = new JArray("val1", "val2", "val3");
                script.Body("hoge1", strings);
                script.Query("hoge3", "fuga3");
                var response = await script.Post();
                var query = (JObject)response["query"];
                Assert.AreEqual(query["hoge3"].ToString(), "fuga3");
                var body = (JObject)response["body"];
                Assert.AreEqual(body["hoge1"], strings);
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestScriptPut()
        {
            Task.Run(async () =>
            {
                var script = new NCMBScript("script_test_put.js");
                script.Header("hoge1", "fuga1");
                script.Header("hoge2", "fuga2");
                var strings = new JArray("val1", "val2", "val3");
                script.Body("hoge1", strings);
                script.Query("hoge3", "fuga3");
                var response = await script.Put();
                
                var query = (JObject)response["query"];
                Assert.AreEqual(query["hoge3"].ToString(), "fuga3");
                var body = (JObject)response["body"];
                Assert.AreEqual(body["hoge1"], strings);
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestScriptDelete()
        {
            Task.Run(async () =>
            {
                var script = new NCMBScript("script_test_delete.js");
                script.Header("hoge1", "fuga1");
                script.Header("hoge2", "fuga2");
                var strings = new JArray("val1", "val2", "val3");
                script.Body("hoge1", strings);
                script.Query("hoge3", "fuga3");
                var response = await script.Delete();
                var query = (JObject)response["query"];
                Assert.AreEqual(query["hoge3"].ToString(), "fuga3");
            }).GetAwaiter().GetResult();
        }
    }
}
