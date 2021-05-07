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
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
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
            var results = query.FindAll();
            Assert.AreEqual(results.Length, 3);
            query = _ncmb.Query("QueryTest");
            results = query.FindAll();
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
                var results = await query.FindAllAsync();
                Assert.AreEqual(results.Length, 3);
                query = _ncmb.Query("QueryTest");
                results = await query.FindAllAsync();
                foreach (var obj in results)
                {
                    await obj.DeleteAsync();
                }
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestFindLimitASync()
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
                query.EqualTo("message", "Test message");
                var results = await query.FindAllAsync();
                Assert.AreEqual(results.Length, 5);
                query = _ncmb.Query("QueryTest");
                query.EqualTo("message", "Test message").Limit(2);
                results = await query.FindAllAsync();
                Assert.AreEqual(results.Length, 2);
                query = _ncmb.Query("QueryTest");
                results = await query.FindAllAsync();
                foreach (var obj in results)
                {
                    await obj.DeleteAsync();
                }
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestFindPointerASync()
        {
            Task.Run(async () =>
            {
                var item1 = _ncmb.Object("QueryTest");
                item1.Set("message", "Test message");
                item1.Set("number", 500);
                await item1.SaveAsync();

                var item2 = _ncmb.Object("QueryTest");
                item2.Set("message", "Test message");
                item2.Set("number", 500);
                item2.Set("obj", item1);
                await item2.SaveAsync();

                var query = _ncmb.Query("QueryTest");
                query.EqualTo("objectId", item2.Get("objectId")).Include("obj");
                var obj = await query.FindAsync();
                Assert.AreEqual(obj.Get("objectId"), item2.Get("objectId"));
                Assert.AreEqual(((NCMBObject) obj.Get("obj")).Get("objectId"), item1.Get("objectId"));
                
                await obj.DeleteAsync();
                await item1.DeleteAsync();
                
            }).GetAwaiter().GetResult();
        }


    }
}
