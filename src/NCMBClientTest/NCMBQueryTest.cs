using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace NCMBClientTest
{
    public class NCMBQueryTest: Test
    {
        public NCMBQueryTest(): base()
        {
        }

        [Test()]
        public void TestFetchSync()
        {
            Task.Run(async () =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var item = new NCMBObject("QueryTest");
                    item.Set("message", "Test message");
                    item.Set("number", 500 + i);
                    await item.Save();
                }
                var query = new NCMBQuery("QueryTest");
                query.EqualTo("message", "Test message").GreaterThanOrEqualTo("number", 502);
                var results = await query.FetchAll();
                Assert.AreEqual(results.Length, 3);
                query = new NCMBQuery("QueryTest");
                results = await query.FetchAll();
                foreach (var obj in results)
                {
                    await obj.Delete();
                }
            }).GetAwaiter().GetResult();
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
                    await item.Save();
                }
                var query = new NCMBQuery("QueryTest");
                query.EqualTo("message", "Test message").GreaterThanOrEqualTo("number", 502);
                var results = await query.FetchAll();
                Assert.AreEqual(results.Length, 3);
                query = new NCMBQuery("QueryTest");
                results = await query.FetchAll();
                foreach (var obj in results)
                {
                    await obj.Delete();
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
                    await item.Save();
                }
                var query = new NCMBQuery("QueryTest");
                query.EqualTo("message", "Test message");
                var results = await query.FetchAll();
                Assert.AreEqual(results.Length, 5);
                query = new NCMBQuery("QueryTest");
                query.EqualTo("message", "Test message").Limit(2);
                results = await query.FetchAll();
                Assert.AreEqual(results.Length, 2);
                query = new NCMBQuery("QueryTest");
                results = await query.FetchAll();
                foreach (var obj in results)
                {
                    await obj.Delete();
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
                await item1.Save();

                var item2 = new NCMBObject("QueryTest");
                item2.Set("message", "Test message");
                item2.Set("number", 500);
                item2.Set("obj", item1);
                await item2.Save();

                var query = new NCMBQuery("QueryTest");
                query
                    .EqualTo("objectId", item2.Get("objectId"))
                    .Include("obj");
                var obj = await query.Fetch();
                Assert.AreEqual(obj.Get("objectId"), item2.Get("objectId"));
                Assert.AreEqual(((NCMBObject)obj.Get("obj")).Get("objectId"), item1.Get("objectId"));

                await obj.Delete();
                await item1.Delete();

            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestFetchWithCount()
        {
            Task.Run(async () =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var item = new NCMBObject("QueryTest");
                    item.Set("message", "Test message");
                    item.Set("number", i);
                    await item.Save();
                }

                var query = new NCMBQuery("QueryTest");
                query.GreaterThan("number", 3);
                var items = await query.Count().FetchAll();
                Assert.AreEqual(1, query.GetCount());
                query = new NCMBQuery("QueryTest");
                query.GreaterThanOrEqualTo("number", 3);
                items = await query.Count().FetchAll();
                Assert.AreEqual(2, query.GetCount());

                query = new NCMBQuery("QueryTest");
                items = await query.FetchAll();
                foreach (var item in items)
                {
                    await item.Delete();
                }
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestFetchOr()
        {
            Task.Run(async () =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var item = new NCMBObject("QueryTest");
                    item.Set("message", "Test message");
                    item.Set("number", i);
                    await item.Save();
                }

                var q1 = new NCMBQuery("QueryTest");
                var q2 = new NCMBQuery("QueryTest");

                q1.EqualTo("number", 0);
                q2.EqualTo("number", 1);
                var query = new NCMBQuery("QueryTest");
                query.Or(new NCMBQuery[2]{ q1, q2 });
                var items = await query.FetchAll();
            
                Assert.AreEqual(2, items.Length);
                var num0 = items[0].Get("number") as JValue;
                var num1 = items[1].Get("number") as JValue;

                Assert.AreEqual(0, num0.ToObject<int>());
                Assert.AreEqual(1, num1.ToObject<int>());
                query = new NCMBQuery("QueryTest");
                items = await query.FetchAll();
                foreach (var item in items)
                {
                    await item.Delete();
                }
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestFetchSelect()
        {
            Task.Run(async () =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var item = new NCMBObject("Test");
                    item.Set("message", "Test message");
                    item.Set("number", i);
                    await item.Save();

                    var item2 = new NCMBObject("Test2");
                    item2.Set("message", "Test message");
                    item2.Set("num", i);
                    await item2.Save();

                }

                var q1 = new NCMBQuery("Test");
                var q2 = new NCMBQuery("Test2");

                q2.InString("num", new int[] {1, 4 });

                var t = q2.FetchAll();
                var items = await q1.Select("number", "num", q2).FetchAll();
                foreach(var item in items)
                {
                    var num = item.GetInt("number");
                    Assert.IsTrue(num == 4 || num == 1);
                }

                var query = new NCMBQuery("Test");
                items = await query.FetchAll();
                foreach (var item in items)
                {
                    await item.Delete();
                }
                query = new NCMBQuery("Test2");
                items = await query.FetchAll();
                foreach (var item in items)
                {
                    await item.Delete();
                }
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestFetchInQuery()
        {
            Task.Run(async () =>
            {
                for (var i = 0; i < 5; i++)
                {
                    var item = new NCMBObject("Test");
                    item.Set("message", "Test message");
                    item.Set("number", i);
                    await item.Save();

                    var item2 = new NCMBObject("Test2");
                    item2.Set("message", "Test message");
                    item2.Set("num", item);
                    await item2.Save();

                }

                var q1 = new NCMBQuery("Test");
                var q2 = new NCMBQuery("Test2");

                q1.InString("number", new int[] { 1, 4 });
                var items = await q2.InQuery("num", q1).Include("num").FetchAll();
                Assert.IsTrue(items.Length > 0);
                foreach (var item in items)
                {
                    var num = ((NCMBObject) item.Get("num")).GetInt("number");
                    Assert.IsTrue(num == 4 || num == 1);
                }

                var query = new NCMBQuery("Test");
                items = await query.FetchAll();
                foreach (var item in items)
                {
                    await item.Delete();
                }
                query = new NCMBQuery("Test2");
                items = await query.FetchAll();
                foreach (var item in items)
                {
                    await item.Delete();
                }
            }).GetAwaiter().GetResult();
        }
    }
}
