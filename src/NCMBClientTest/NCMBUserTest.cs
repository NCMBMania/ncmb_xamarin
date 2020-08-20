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
        }

        [Test()]
        public void TestSignUpSync()
        {
            var user = this._ncmb.User();
            user.Set("userName", "TestUser");
            user.Set("password", "TestPass");
            user.SignUp();
            Console.WriteLine(user.ObjectId());
            Assert.AreNotEqual(user.ObjectId(), "");
            user.Delete();
            _ncmb.Logout();
        }

        [Test()]
        public void TestSignUpASync()
        {
            Task.Run(async () =>
            {
                var user = this._ncmb.User();
                user.Set("userName", "TestUser");
                user.Set("password", "TestPass");
                await user.SignUpAsync();
                Console.WriteLine(user.ObjectId());
                Assert.AreNotEqual(user.ObjectId(), "");
                await user.DeleteAsync();
                _ncmb.Logout();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestLoginSync()
        {
            var user = this._ncmb.User();
            user.Set("userName", "TestLogin");
            user.Set("password", "TestLogin");
            Assert.AreEqual(user.Login(), true);
            Console.WriteLine(user.ObjectId());
            Assert.AreNotEqual(user.ObjectId(), "");
            Assert.AreNotEqual(_ncmb.SessionToken, "");
            Assert.NotNull(_ncmb.SessionToken);
        }

        [Test()]
        public void TestLoginASync()
        {
            Task.Run(async () =>
            {
                var user = this._ncmb.User();
                user.Set("userName", "TestLogin");
                user.Set("password", "TestLogin");
                Assert.AreEqual(await user.LoginAsync(), true);
                Console.WriteLine(user.ObjectId());
                Assert.AreNotEqual(user.ObjectId(), "");
                Assert.AreNotEqual(_ncmb.SessionToken, "");
                Assert.NotNull(_ncmb.SessionToken);
            }).GetAwaiter().GetResult();
        }

    }
}
