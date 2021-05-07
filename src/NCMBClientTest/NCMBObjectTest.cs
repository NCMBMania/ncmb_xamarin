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
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
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
        public void TestSaveAndDeleteWithAclSync()
        {
            var message = "Hello, world";
            var item = _ncmb.Object("DataStoreTest");
            item.Set("message", message);
            var acl = _ncmb.Acl();
            acl.SetPublicReadAccess(true);
            acl.SetPublicWriteAccess(false);
            acl.SetRoleReadAccess("admin", true);
            acl.SetRoleWriteAccess("admin", true);
            item.SetAcl(acl);
            item.Save();
            Assert.NotNull(item.Get("objectId"));
            Assert.AreEqual(item.Get("message").ToString(), message);
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
                newItem.Set("objectId", objectId);
                await newItem.FetchAsync();
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
            newItem.Set("objectId", objectId);
            newItem.Fetch();
            Assert.AreEqual(item.ObjectId(), newItem.ObjectId());
            Assert.AreEqual(newItem.Get("message").ToString(), message);
            item.Delete();
        }

    }
}
