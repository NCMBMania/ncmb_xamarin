using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;

namespace NCMBClientTest
{
    public class NCMBQueryTest
    {
        public NCMBQueryTest()
        {
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
            new NCMB(ApplicationKey, ClientKey);
        }

        [Test()]
        public void TestFetchSync()
        {
            for (var i = 0; i < 5; i++)
            {
                var item = new NCMBObject("QueryTest");
                item.Set("message", "Test message");
                item.Set("number", 500 + i);
                item.Save();
            }
            var query = new NCMBQuery("QueryTest");
            query.EqualTo("message", "Test message").GreaterThanOrEqualTo("number", 502);
            var results = query.FetchAll();
            Assert.AreEqual(results.Length, 3);
            query = new NCMBQuery("QueryTest");
            results = query.FetchAll();
            foreach (var obj in results)
            {
                obj.Delete();
            }

        }

        [Test()]
        public void TestFetchASync()
        {
            Task.Run(async () =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var item = new NCMBObject("QueryTest");
                    item.Set("message", "Test message");
                    item.Set("number", 500 + i);
                    await item.SaveAsync();
                }
                var query = new NCMBQuery("QueryTest");
                query.EqualTo("message", "Test message").GreaterThanOrEqualTo("number", 502);
                var results = await query.FetchAllAsync();
                Assert.AreEqual(results.Length, 3);
                query = new NCMBQuery("QueryTest");
                results = await query.FetchAllAsync();
                foreach (var obj in results)
                {
                    await obj.DeleteAsync();
                }
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestFetchLimitASync()
        {
            Task.Run(async () =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var item = new NCMBObject("QueryTest");
                    item.Set("message", "Test message");
                    item.Set("number", 500 + i);
                    await item.SaveAsync();
                }
                var query = new NCMBQuery("QueryTest");
                query.EqualTo("message", "Test message");
                var results = await query.FetchAllAsync();
                Assert.AreEqual(results.Length, 5);
                query = new NCMBQuery("QueryTest");
                query.EqualTo("message", "Test message").Limit(2);
                results = await query.FetchAllAsync();
                Assert.AreEqual(results.Length, 2);
                query = new NCMBQuery("QueryTest");
                results = await query.FetchAllAsync();
                foreach (var obj in results)
                {
                    await obj.DeleteAsync();
                }
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestFetchPointerASync()
        {
            Task.Run(async () =>
            {
                var item1 = new NCMBObject("QueryTest");
                item1.Set("message", "Test message");
                item1.Set("number", 500);
                await item1.SaveAsync();

                var item2 = new NCMBObject("QueryTest");
                item2.Set("message", "Test message");
                item2.Set("number", 500);
                item2.Set("obj", item1);
                await item2.SaveAsync();

                var query = new NCMBQuery("QueryTest");
                query
                    .EqualTo("objectId", item2.Get("objectId"))
                    .Include("obj");
                var obj = await query.FetchAsync();
                Assert.AreEqual(obj.Get("objectId"), item2.Get("objectId"));
                Assert.AreEqual(((NCMBObject) obj.Get("obj")).Get("objectId"), item1.Get("objectId"));
                
                await obj.DeleteAsync();
                await item1.DeleteAsync();
                
            }).GetAwaiter().GetResult();
        }


    }
}
