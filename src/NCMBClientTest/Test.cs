using NUnit.Framework;
using System;
using NCMBClient;
using System.IO;

namespace NCMBClientTest
{
    [TestFixture()]
    public class Test
    {
        public NCMB _ncmb;

        public Test()
        {
            var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));
            DotNetEnv.Env.Load(dir + "/.env");
            _ncmb = new NCMB(DotNetEnv.Env.GetString("APPLICATION_KEY"), DotNetEnv.Env.GetString("CLIENT_KEY"));
        }

        [Test()]
        public void TestCase()
        {
            Assert.AreNotEqual(_ncmb.ApplicationKey, "");
        }
    }
}
