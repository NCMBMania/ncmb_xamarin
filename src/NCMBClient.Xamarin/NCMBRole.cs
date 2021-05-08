using System;
using System.Threading.Tasks;

namespace NCMBClient
{
    public class NCMBRole : NCMBObject
    {
        public NCMBRole() : base("roles")
        {
        }

        public NCMBRole AddUser(NCMBUser user)
        {
            var key = "belongUser";
            return this.Add(key, user);
        }

        public NCMBRole RemoveUser(NCMBUser user)
        {
            var key = "belongUser";
            return this.Remove(key, user);
        }

        public NCMBRole AddRole(NCMBRole role)
        {
            var key = "belongRole";
            return this.Add(key, role);
        }

        public NCMBRole RemoveRole(NCMBRole role)
        {
            var key = "belongRole";
            return this.Remove(key, role);
        }

        public NCMBQuery Query()
        {
            return new NCMBQuery("roles");
        }

        public NCMBRole[] FetchUser()
        {
            var query = Query();
            return (NCMBRole[])query.RelatedTo(this, "belongUser").FetchAll();
        }

        public async Task<NCMBRole[]> FetchUserAsync()
        {
            var query = Query();
            return (NCMBRole[])await query.RelatedTo(this, "belongUser").FetchAllAsync();
        }

        public NCMBRole[] FetchRole()
        {
            var query = Query();
            var roles = query.RelatedTo(this, "belongRole").FetchAll();
            Console.WriteLine(roles[0].Get("objectId"));
            return (NCMBRole[]) roles;
        }

        public async Task<NCMBRole[]> FetchRoleAsync()
        {
            var query = Query();
            return (NCMBRole[]) await query.RelatedTo(this, "belongRole").FetchAllAsync();
        }

        private NCMBRole Add(string key, NCMBObject obj)
        {
            var relation = (NCMBRelation)this.Get(key);
            if (relation is null)
            {
                relation = new NCMBRelation();
            }
            relation.Add(obj);
            this.Set(key, relation);
            return this;
        }

        private NCMBRole Remove(string key, NCMBObject obj)
        {
            var relation = (NCMBRelation)this.Get(key);
            if (relation is null)
            {
                relation = new NCMBRelation();
            }
            relation.Remove(obj);
            this.Set(key, relation);
            return this;
        }

    }
}
