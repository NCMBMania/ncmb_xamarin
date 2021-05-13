using NUnit.Framework;
using System;
using NCMBClient;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBPushTest
    {
        public NCMBPushTest()
        {
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
            new NCMB(ApplicationKey, ClientKey);
        }

        [Test()]
        public void TestCreateInstallation()
        {
            var installation = new NCMBInstallation();
            installation
                .Set("deviceToken", "aaa")
                .Set("deviceType", "ios")
                .Save();
            Assert.NotNull(installation.Get("objectId"));
            installation.Delete();
        }

        [Test()]
        public void TestCreateInstallationFail()
        {
            var installation = new NCMBInstallation();
            try
            {
                installation
                    .Set("deviceType", "ios");
                installation.Save();
                Assert.AreEqual(true, false);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "deviceToken is required.");
            }
        }


        [Test()]
        public void TestFetchInstallations()
        {

            var ary = new string[3] { "aaa", "bbb", "ccc" };
            foreach (var deviceToken in ary)
            {
                var installation = new NCMBInstallation();
                installation
                .Set("deviceToken", deviceToken)
                .Set("deviceType", "ios")
                .Save();
            }
            var query = NCMBInstallation.Query();
            var installations = query.FetchAll();
            Assert.AreEqual(3, installations.Length);
            foreach (var installation in installations)
            {
                installation.Delete();
            }
        }

    }


}
