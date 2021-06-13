using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBInstallationTest
    {
        public NCMBInstallationTest()
        {
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
            new NCMB(ApplicationKey, ClientKey);
        }

        [Test()]
        public void TestCreateInstallation()
        {
            Task.Run(async () =>
            {
                var installation = new NCMBInstallation();
                await installation
                    .Set("deviceToken", "aaa")
                    .Set("deviceType", "ios")
                    .Save();
                Assert.NotNull(installation.Get("objectId"));
                await installation.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestCreateInstallationFail()
        {
            Task.Run(async () =>
            {
                var installation = new NCMBInstallation();
                try
                {
                    installation
                        .Set("deviceType", "ios");
                    await installation.Save();
                    Assert.AreEqual(true, false);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(e.Message, "deviceToken is required.");
                }
            }).GetAwaiter().GetResult();
        }


        [Test()]
        public void TestFetchInstallations()
        {
            Task.Run(async () =>
            {
                var ary = new string[3] {"aaa", "bbb", "ccc"};
                foreach (var deviceToken in ary)
                {
                    var installation = new NCMBInstallation();
                    await installation
                        .Set("deviceToken", deviceToken)
                        .Set("deviceType", "ios")
                        .Save();
                }
                var query = NCMBInstallation.Query();
                var installations = await query.FetchAll();
                Assert.AreEqual(3, installations.Length);
                foreach (var installation in installations)
                {
                    await installation.Delete();
                }
            }).GetAwaiter().GetResult();
        }
    }
}
