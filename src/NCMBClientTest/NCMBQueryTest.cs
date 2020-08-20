using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;

namespace NCMBClientTest
{
    public class NCMBQueryTest
    {
        private NCMB _ncmb;
        public NCMBQueryTest()
        {
            var ApplicationKey = "08068a4622540e7586869a9bc4de3655967d8282a0bb6463787c849dc8daee87";
            var ClientKey = "ab48936a4f392b7517c2cf241cb0049753409a1845f7a429c812aca602844395";
            _ncmb = new NCMB(ApplicationKey, ClientKey);
        }

        [Test()]
        public void TestFindSync()
        {
            for (var i = 0; i < 5; i++)
            {
                var item = _ncmb.Object("QueryTest");
                item.Set("message", "Test message");
                item.Set("number", 500 + i);
                item.Save();
            }
            var query = _ncmb.Query("QueryTest");
            query.EqualTo("message", "Test message").GreaterThanOrEqualTo("number", 502);
            var results = query.Find();
            Assert.AreEqual(results.Length, 3);
            foreach (var obj in results)
            {
                obj.Delete();
            }
        }

        [Test()]
        public void TestFindASync()
        {
            Task.Run(async () =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var item = _ncmb.Object("QueryTest");
                    item.Set("message", "Test message");
                    item.Set("number", 500 + i);
                    await item.SaveAsync();
                }
                var query = _ncmb.Query("QueryTest");
                query.EqualTo("message", "Test message").GreaterThanOrEqualTo("number", 502);
                var results = await query.FindAsync();
                Assert.AreEqual(results.Length, 3);
                foreach (var obj in results)
                {
                    await obj.DeleteAsync();
                }
            }).GetAwaiter().GetResult();
        }

    }
}
