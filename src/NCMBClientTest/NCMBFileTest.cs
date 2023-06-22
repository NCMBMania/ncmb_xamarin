using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;
using System.IO;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBFileTest : Test
    {
        public NCMBFileTest(): base()
        {
        }

        [Test()]
        public void TestUpload()
        {
            Task.Run(async () =>
            {
                var str = "1,2,3";
                var data = System.Text.Encoding.ASCII.GetBytes(str);
                var file = new NCMBFile("test2.csv", data, "text/csv");
                await file.Save();
                Assert.NotNull(file.Get("fileName"));
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestUploadBinary()
        {
            Task.Run(async () =>
            {
                var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));
                var data = File.ReadAllBytes(dir + "/ncmb.png");
                var file = new NCMBFile("ncmb.png", data, "image/png");
                await file.Save();
                Assert.NotNull(file.Get("fileName"));
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestUploadWithAcl()
        {
            Task.Run(async () =>
            {
                var str = "1,2,3";
                var data = System.Text.Encoding.ASCII.GetBytes(str);
                var file = new NCMBFile("test2.csv", data, "text/csv");
                var acl = new NCMBAcl();
                acl.SetPublicReadAccess(true);
                acl.SetPublicWriteAccess(true);
                file.SetAcl(acl);
                await file.Save();
                Assert.NotNull(file.Get("fileName"));
            }).GetAwaiter().GetResult();
        }
        /*
        [Test()]
        public void TestSaveAndDeleteSync()
        {
            Task.Run(async () =>
            {

            }).GetAwaiter().GetResult();
        }
        */
    }
}

