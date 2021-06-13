using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBRelationTest
    {
        public NCMBRelationTest()
        {
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
            new NCMB(ApplicationKey, ClientKey);
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
