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
            var item1 = new NCMBObject("RelationTest");
            item1.Set("name", "item1").Save();
            var item2 = new NCMBObject("RelationTest");
            item2.Set("name", "item2").Save();

            var relation = new NCMBRelation();
            relation.Add(item1).Add(item2);

            var item3 = new NCMBObject("RelationMaster");
            item3.Set("relation", relation).Save();

            Assert.NotNull(item3.Get("objectId"));

        }

        [Test()]
        public void TestRelationRemove()
        {
            var item1 = new NCMBObject("RelationTest");
            item1.Set("name", "item1").Save();
            var item2 = new NCMBObject("RelationTest");
            item2.Set("name", "item2").Save();

            var relation = new NCMBRelation();
            relation.Add(item1).Add(item2);

            var item3 = new NCMBObject("RelationMaster");
            item3.Set("relation", relation).Save();

            Assert.NotNull(item3.Get("objectId"));

            relation = new NCMBRelation();
            relation.Remove(item1);
            item3.Set("relation", relation).Save();
            item3.Delete();
            item1.Delete();
            item2.Delete();
        }

        [Test()]
        public void TestRelationFetch()
        {
            var item1 = new NCMBObject("RelationTest");
            item1.Set("name", "item1").Save();
            var item2 = new NCMBObject("RelationTest");
            item2.Set("name", "item2").Save();

            var relation = new NCMBRelation();
            relation.Add(item1).Add(item2);

            var item3 = new NCMBObject("RelationMaster");
            item3.Set("relation", relation).Save();
            Assert.NotNull(item3.Get("objectId"));

            var query = new NCMBQuery("RelationTest");
            var items = query.RelatedTo(item3, "relation").FetchAll();

            Assert.AreEqual(items.Length, 2);
            Assert.AreEqual(items[0].Get("objectId").ToString(), item1.Get("objectId").ToString());
        }
    }
}
