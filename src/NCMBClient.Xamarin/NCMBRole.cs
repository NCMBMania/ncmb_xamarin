using System;
using System.Threading.Tasks;
using System.Collections;

namespace NCMBClient
{
    public class NCMBRole : NCMBObject
    {
        private string __op;
        private ArrayList users;
        private ArrayList roles;

        public NCMBRole() : base("roles")
        {
            ClearOperation();
        }

        public void ClearOperation()
        {
            __op = "";
            users = new ArrayList();
            roles = new ArrayList();
        }

        private void SetOpe(string ope)
        {
            if (__op != "" && __op != ope)
            {
                throw new Exception($"Already set other operation {__op}");
            }
            __op = ope;
        }
        public NCMBRole AddUser(NCMBUser user)
        {
            SetOpe("AddUser");
            users.Add(user);
            return this;
            // return this.Add(key, user);
        }

        public NCMBRole RemoveUser(NCMBUser user)
        {
            SetOpe("RemoveUser");
            users.Add(user);
            return this;
        }

        public NCMBRole AddRole(NCMBRole role)
        {
            SetOpe("AddRole");
            roles.Add(role);
            return this;
        }

        public NCMBRole RemoveRole(NCMBRole role)
        {
            SetOpe("RemoveRole");
            roles.Add(role);
            return this;
        }

        public static NCMBQuery Query()
        {
            return new NCMBQuery("roles");
        }

        public async Task<NCMBUser[]> FetchUser()
        {
            var query = NCMBUser.Query();
            var ary = await query.RelatedTo(this, "belongUser").FetchAll();
            return ReturnUser(ary);
        }

        public NCMBUser[] ReturnUser(NCMBObject[] ary)
        {
            if (ary.Length == 0)
            {
                return new NCMBUser[0];
            }
            var results = new NCMBUser[ary.Length];
            var i = 0;
            foreach (var obj in ary)
            {
                results[i] = (NCMBUser)obj;
                i++;
            }
            return results;
        }

        public async Task<NCMBRole[]> FetchRole()
        {
            var query = Query();
            var ary = await query.RelatedTo(this, "belongRole").FetchAll();
            return ReturnRole(ary);
        }

        public NCMBRole[] ReturnRole(NCMBObject[] ary)
        {
            if (ary.Length == 0)
            {
                return new NCMBRole[0];
            }
            var results = new NCMBRole[ary.Length];
            var i = 0;
            foreach (var obj in ary)
            {
                results[i] = (NCMBRole)obj;
                i++;
            }
            return results;
        }

        public async new Task<bool> Save()
        {
            var relation = new NCMBRelation();
            if (__op == "AddUser" || __op == "AddRole")
            {
                foreach (var user in users)
                {
                    relation.Add((NCMBUser)user);
                }
                foreach (var role in roles)
                {
                    relation.Add((NCMBRole)role);
                }
            }
            if (__op == "RemoveUser" || __op == "RemoveRole")
            {
                foreach (var user in users)
                {
                    relation.Remove((NCMBUser)user);
                }
                foreach (var role in roles)
                {
                    relation.Remove((NCMBRole)role);
                }
            }
            if (__op == "AddUser" || __op == "RemoveUser")
            {
                this.Set("belongUser", relation);
            }
            if (__op == "AddRole" || __op == "RemoveRole")
            {
                this.Set("belongRole", relation);
            }
            if (users.Count == 0)
            {
                this._fields.Remove("belongUser");
            }
            if (roles.Count == 0)
            {
                this._fields.Remove("belongRole");
            }
            await base.Save();
            return true;
        }
    }
}