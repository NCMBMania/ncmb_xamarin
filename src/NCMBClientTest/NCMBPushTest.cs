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

    }


}
