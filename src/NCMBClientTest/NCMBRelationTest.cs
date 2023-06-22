using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;
using System.IO;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBRelationTest: Test
    {
        public NCMBRelationTest(): base()
        {
        }

        [Test()]
        public void TestRelationSave()
        {
            Task.Run(async () =>
            {
                var item1 = new NCMBObject("RelationTest");
                await item1.Set("name", "item1").Save();
                var item2 = new NCMBObject("RelationTest");
                await item2.Set("name", "item2").Save();

                var relation = new NCMBRelation();
                relation.Add(item1).Add(item2);

                var item3 = new NCMBObject("RelationMaster");
                await item3.Set("relation", relation).Save();

                Assert.NotNull(item3.Get("objectId"));
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestRelationRemove()
        {
            Task.Run(async () =>
            {
                var item1 = new NCMBObject("RelationTest");
                await item1.Set("name", "item1").Save();
                var item2 = new NCMBObject("RelationTest");
                await item2.Set("name", "item2").Save();

                var relation = new NCMBRelation();
                relation.Add(item1).Add(item2);

                var item3 = new NCMBObject("RelationMaster");
                await item3.Set("relation", relation).Save();

                Assert.NotNull(item3.Get("objectId"));

                relation = new NCMBRelation();
                relation.Remove(item1);
                await item3.Set("relation", relation).Save();
                await item3.Delete();
                await item1.Delete();
                await item2.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestRelationFetch()
        {
            Task.Run(async () =>
            {
                var item1 = new NCMBObject("RelationTest");
                await item1.Set("name", "item1").Save();
                var item2 = new NCMBObject("RelationTest");
                await item2.Set("name", "item2").Save();

                var relation = new NCMBRelation();
                relation.Add(item1).Add(item2);

                var item3 = new NCMBObject("RelationMaster");
                await item3.Set("relation", relation).Save();
                Assert.NotNull(item3.Get("objectId"));

                var query = new NCMBQuery("RelationTest");
                var items = await query.RelatedTo(item3, "relation").FetchAll();

                Assert.AreEqual(items.Length, 2);
                Assert.AreEqual(items[0].Get("objectId").ToString(), item1.Get("objectId").ToString());
            }).GetAwaiter().GetResult();
        }
    }
}
