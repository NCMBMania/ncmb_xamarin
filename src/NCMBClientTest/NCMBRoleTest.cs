using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBRoleTest
    {
        public NCMBRoleTest()
        {
            var ApplicationKey = "9170ffcb91da1bbe0eff808a967e12ce081ae9e3262ad3e5c3cac0d9e54ad941";
            var ClientKey = "9e5014cd2d76a73b4596deffdc6ec4028cfc1373529325f8e71b7a6ed553157d";
            new NCMB(ApplicationKey, ClientKey);
        }

        [Test()]
        public void TestCreateRole()
        {
            var role = new NCMBRole();
            role.Set("roleName", "admin");
            role.Save();
            Assert.NotNull(role.Get("objectId"));
            role.Delete();
        }

        [Test()]
        public void TestAddRole()
        {

            var role1 = new NCMBRole();
            role1.Set("roleName", "role1");
            role1.Save();
            Assert.NotNull(role1.Get("objectId"));

            var role2 = new NCMBRole();
            role2.Set("roleName", "role2");
            role2.Save();
            Assert.NotNull(role1.Get("objectId"));

            role1.AddRole(role2).Save();

            role1.Fetch();

            var roles = role1.FetchRole();

            Assert.AreEqual(role2.Get("roleName").ToString(), roles[0].Get("roleName").ToString());

            role1.Delete();
            role2.Delete();
        }

    }
}
