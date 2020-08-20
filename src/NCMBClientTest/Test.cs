using NUnit.Framework;
using System;
using NCMBClient;

namespace NCMBClientTest
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void TestCase()
        {
            var ApplicationKey = "08068a4622540e7586869a9bc4de3655967d8282a0bb6463787c849dc8daee87";
            var ClientKey = "ab48936a4f392b7517c2cf241cb0049753409a1845f7a429c812aca602844395";
            var ncmb = new NCMB(ApplicationKey, ClientKey);
            Assert.AreNotEqual(ncmb.ApplicationKey, "");
        }
    }
}
