using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBObjectTest
    {
        private NCMB _ncmb;

        public NCMBObjectTest()
        {
            var ApplicationKey = "08068a4622540e7586869a9bc4de3655967d8282a0bb6463787c849dc8daee87";
            var ClientKey = "ab48936a4f392b7517c2cf241cb0049753409a1845f7a429c812aca602844395";
            _ncmb = new NCMB(ApplicationKey, ClientKey);
        }

        [Test()]
        public void TestSaveAndDeleteSync()
        {
            var message = "Hello, world";
            var item = _ncmb.Object("DataStoreTest");
            item.Set("message", message);
            item.Save();
            Assert.NotNull(item.Get("objectId"));
            Assert.AreEqual(item.Get("message").ToString(), message);
            item.Delete();
        }

        [Test()]
        public void TestSaveAndDeleteASync()
        {
            Task.Run(async () =>
            {
                var message = "Hello, world";
                var item = _ncmb.Object("DataStoreTest");
                item.Set("message", message);
                await item.SaveAsync();
                Assert.NotNull(item.Get("objectId"));
                Assert.AreEqual(item.Get("message").ToString(), message);
                await item.DeleteAsync();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSaveAndFetchASync()
        {
            Task.Run(async () =>
            {
                var message = "Hello, world";
                var item = _ncmb.Object("DataStoreTest");
                item.Set("message", message);
                await item.SaveAsync();
                Assert.NotNull(item.ObjectId());
                var objectId = item.ObjectId();
                var newItem = _ncmb.Object("DataStoreTest");
                await newItem.Set("objectId", objectId).FetchAsync();
                Assert.AreEqual(item.ObjectId(), newItem.ObjectId());
                Assert.AreEqual(newItem.Get("message").ToString(), message);
                await item.DeleteAsync();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSaveAndFetchSync()
        {
            var message = "Hello, world";
            var item = _ncmb.Object("DataStoreTest");
            item.Set("message", message);
            item.Save();
            Assert.NotNull(item.ObjectId());
            var objectId = item.ObjectId();
            var newItem = _ncmb.Object("DataStoreTest");
            newItem.Set("objectId", objectId).Fetch();
            Assert.AreEqual(item.ObjectId(), newItem.ObjectId());
            Assert.AreEqual(newItem.Get("message").ToString(), message);
            item.Delete();
        }

    }
}
