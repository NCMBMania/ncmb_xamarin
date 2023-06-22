using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace NCMBClientTest
{
    
    [TestFixture()]
    public class NCMBUserTest : Test
    {
        public NCMBUserTest(): base()
        {
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
                try {
                    await user.SignUp();
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                

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

        [Test()]
        public void TestLoginByFacebook()
        {
            Task.Run(async () =>
            {
                var facebookId = DotNetEnv.Env.GetString("FACEBOOK_ID");
                var facebookAccessToken = DotNetEnv.Env.GetString("FACEBOOK_ACCESS_TOKEN");
                var dt = DateTime.Now;
                dt.AddHours(1);
                var user = await NCMBUser.SignUpWithFacebook(facebookId, facebookAccessToken, dt);
                Assert.AreNotEqual(user.ObjectId(), "");
                Assert.AreNotEqual(_ncmb.SessionToken, "");
                Assert.NotNull(_ncmb.SessionToken);
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestLoginByAnonymous()
        {
            Task.Run(async () =>
            {
                var user = await NCMBUser.SignUpWithAnonymous();
                Assert.AreNotEqual(user.ObjectId(), "");
                Assert.AreNotEqual(_ncmb.SessionToken, "");
                Assert.NotNull(_ncmb.SessionToken);
                var authData = user.Get("authData") as JObject;
                var data = authData["anonymous"] as JObject;
                var id = data["id"].ToString();
                var user2 = await NCMBUser.SignUpWithAnonymous(id);
                Assert.AreEqual(user.ObjectId(), user2.ObjectId());
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestRequestMailAddressUserEntry()
        {
            Task.Run(async () =>
            {
                var bol = await NCMBUser.RequestAuthenticationMail(DotNetEnv.Env.GetString("EMAIL_ADDRESS"));
                Assert.AreEqual(bol, true);
            }).GetAwaiter().GetResult();
        }

    }
}
