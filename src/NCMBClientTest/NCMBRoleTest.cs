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
            Task.Run(async () =>
            {
                var role = new NCMBRole();
                role.Set("roleName", "admin2");
                await role.Save();
                Assert.NotNull(role.Get("objectId"));
                await role.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestAddRole()
        {
            Task.Run(async () =>
            {
                var role1 = new NCMBRole();
                role1.Set("roleName", "role1");
                await role1.Save();
                Assert.NotNull(role1.Get("objectId"));

                var role2 = new NCMBRole();
                role2.Set("roleName", "role2");
                await role2.Save();
                Assert.NotNull(role1.Get("objectId"));

                await role1.AddRole(role2).Save();

                await role1.Fetch();

                var roles = await role1.FetchRole();

                Assert.AreEqual(role2.Get("roleName").ToString(), roles[0].Get("roleName").ToString());

                await role1.Delete();
                await role2.Delete();
            }).GetAwaiter().GetResult();
        }


        [Test()]
        public void TestAddAndRemoveRole()
        {
            Task.Run(async () =>
            {
                var role1 = new NCMBRole();
                role1.Set("roleName", "role3");
                await role1.Save();
                Assert.NotNull(role1.Get("objectId"));

                var role2 = new NCMBRole();
                role2.Set("roleName", "role4");
                await role2.Save();
                Assert.NotNull(role2.Get("objectId"));

                var role3 = new NCMBRole();
                role3.Set("roleName", "role6");
                await role3.Save();
                Assert.NotNull(role3.Get("objectId"));

                await role1.AddRole(role2).AddRole(role3).Save();

                await role1.Fetch();

                var roles = await role1.FetchRole();
            
                Assert.AreEqual(roles.Length, 2);

                // Assert.AreEqual(role3.Get("roleName").ToString(), roles[0].Get("roleName").ToString());

                role1.ClearOperation();
                await role1.RemoveRole(role2).Save();
                await role1.Fetch();

                var roles2 = await role1.FetchRole();
                Assert.AreEqual(roles2.Length, 1);

                await role1.Delete();
                await role2.Delete();
                await role3.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestAddUser()
        {
            Task.Run(async () =>
            {
                var acl = new NCMBAcl();
                acl.SetPublicWriteAccess(true);

                var user1 = new NCMBUser();
                var userName = "TestLogin1";
                var password = "TestPass";
                user1.Set("userName", userName);
                user1.Set("password", password);
                await user1.SignUp();
                var user = await NCMBUser.Login(userName, password);
                user.SetAcl(acl);
                await user.Save();

                var user2 = new NCMBUser();
                userName = "TestLogin2";
                password = "TestPass";
                user2.Set("userName", userName);
                user2.Set("password", password);
                await user2.SignUp();

                user = await NCMBUser.Login(userName, password);
                user.SetAcl(acl);
                await user.Save();
            
                var role1 = new NCMBRole();
                role1.Set("roleName", "role5");
                await role1.Save();
                Assert.NotNull(role1.Get("objectId"));


                await role1.AddUser(user1).AddUser(user2).Save();

                await role1.Fetch();

                var users = await role1.FetchUser();
                // Console.WriteLine(users.Length);
                Assert.AreEqual(2, users.Length);
                role1.ClearOperation();
                await role1.RemoveUser(user1).Save();
                users = await role1.FetchUser();
                Assert.AreEqual(1, users.Length);
                // Assert.AreEqual(user1.Get("userName").ToString(), users[0].Get("userName").ToString());
                await role1.Delete();
                await user1.Delete();
                await user2.Delete();
            }).GetAwaiter().GetResult();
        }


    }
}
