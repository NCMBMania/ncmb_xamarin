using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;

namespace NCMBClientTest
{
    
    [TestFixture()]
    public class NCMBUserTest : Test
    {
        public NCMBUserTest()
        {
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
            new NCMB(ApplicationKey, ClientKey);
        }

        [Test()]
        public void TestSignUpSync()
        {
            Task.Run(async () =>
            {
                var user = new NCMBUser();
                user.Set("userName", "TestUser");
                user.Set("password", "TestPass");
                await user.SignUp();
                Console.WriteLine(user.ObjectId());
                Assert.AreNotEqual(user.ObjectId(), "");
                await user.Delete();
                NCMBUser.Logout();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSignUpASync()
        {
            Task.Run(async () =>
            {
                var user = new NCMBUser();
                user.Set("userName", "TestUser");
                user.Set("password", "TestPass");
                await user.SignUp();
                Console.WriteLine(user.ObjectId());
                Assert.AreNotEqual(user.ObjectId(), "");
                await user.Delete();
                NCMBUser.Logout();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestLoginASync()
        {
            Task.Run(async () =>
            {
                var user = new NCMBUser();
                user.Set("userName", "TestLogin");
                user.Set("password", "TestPass");
                await user.SignUp();

                user = new NCMBUser();
                user.Set("userName", "TestLogin");
                user.Set("password", "TestPass");
                Assert.AreEqual(await user.Login(), true);
                Console.WriteLine(user.ObjectId());
                Assert.AreNotEqual(user.ObjectId(), "");
                Assert.AreNotEqual(_ncmb.SessionToken, "");
                Assert.NotNull(_ncmb.SessionToken);
                await user.Delete();
                NCMBUser.Logout();
            }).GetAwaiter().GetResult();
        }

        public void TestLoginByEmail()
        {
            var emailAddress = "test@moongift.jp";
            var password = "cA3hWAZLA2zTDnuh4";


        }
    }
}
