using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;
using System.IO;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBObjectTest: Test
    {
        public NCMBObjectTest(): base()
        {
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
