using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBObjectTest
    {
        public NCMBObjectTest()
        {
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
            new NCMB(ApplicationKey, ClientKey);
        }

        [Test()]
        public void TestSaveAndDeleteSync()
        {
            Task.Run(async () =>
            {
                var message = "Hello, world";
                var item = new NCMBObject("DataStoreTest");
                item.Set("message", message);
                await item.Save();
                Assert.NotNull(item.Get("objectId"));
                Assert.AreEqual(item.Get("message").ToString(), message);
                await item.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSaveAndDeleteASync()
        {
            Task.Run(async () =>
            {
                var message = "Hello, world";
                var item = new NCMBObject("DataStoreTest");
                item.Set("message", message);
                await item.Save();
                Assert.NotNull(item.Get("objectId"));
                Assert.AreEqual(item.Get("message").ToString(), message);
                await item.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSaveAndDeleteWithAclSync()
        {
            Task.Run(async () =>
            {
                var message = "Hello, world";
                var item = new NCMBObject("DataStoreTest");
                item.Set("message", message);
                var acl = new NCMBAcl();
                acl.SetPublicReadAccess(true);
                acl.SetPublicWriteAccess(false);
                acl.SetRoleReadAccess("admin", true);
                acl.SetRoleWriteAccess("admin", true);
                item.SetAcl(acl);
                await item.Save();
                Assert.NotNull(item.Get("objectId"));
                Assert.AreEqual(item.Get("message").ToString(), message);
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSaveAndFetchASync()
        {
            Task.Run(async () =>
            {
                var message = "Hello, world";
                var item = new NCMBObject("DataStoreTest");
                item.Set("message", message);
                await item.Save();
                Assert.NotNull(item.ObjectId());
                var objectId = item.ObjectId();
                var newItem = new NCMBObject("DataStoreTest");
                newItem.Set("objectId", objectId);
                await newItem.Fetch();
                Assert.AreEqual(item.ObjectId(), newItem.ObjectId());
                Assert.AreEqual(newItem.Get("message").ToString(), message);
                await item.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSaveAndFetchSync()
        {
            Task.Run(async () =>
            {
                var message = "Hello, world";
                var item = new NCMBObject("DataStoreTest");
                item.Set("message", message);
                await item.Save();
                Assert.NotNull(item.ObjectId());
                var objectId = item.ObjectId();
                var newItem = new NCMBObject("DataStoreTest");
                newItem.Set("objectId", objectId);
                await newItem.Fetch();
                Assert.AreEqual(item.ObjectId(), newItem.ObjectId());
                Assert.AreEqual(newItem.Get("message").ToString(), message);
                await item.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSaveGetBoolean()
        {
            Task.Run(async () =>
            {
                var message = "Hello, world";
                var item = new NCMBObject("DataStoreTest");
                item.Set("message", message);
                item.Set("bol1", true);
                item.Set("bol2", false);
                await item.Save();
                Assert.NotNull(item.Get("objectId"));
                await item.Fetch();
                Assert.IsTrue(item.GetBoolean("bol1"));
                Assert.IsTrue(!item.GetBoolean("bol2"));
                await item.Delete();
            }).GetAwaiter().GetResult();
        }
    }
}
