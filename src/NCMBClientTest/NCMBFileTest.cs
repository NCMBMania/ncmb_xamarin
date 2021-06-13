using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;
using System.IO;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBFileTest
    {
        public NCMBFileTest()
        {
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
            new NCMB(ApplicationKey, ClientKey);
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
                var data = File.ReadAllBytes("../../ncmb.png");
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

