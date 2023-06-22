using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;
using System.IO;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBInstallationTest: Test
    {
        public NCMBInstallationTest(): base()
        {
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
