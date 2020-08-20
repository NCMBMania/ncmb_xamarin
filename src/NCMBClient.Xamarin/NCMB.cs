using System;
namespace NCMBClient
{
    public class NCMB
    {
        public string ApplicationKey { get; }
        public string ClientKey { get; }
        public string SessionToken { get; set; }

        public NCMB(string applicationKey, string clientKey)
        {
            this.ApplicationKey = applicationKey;
            this.ClientKey = clientKey;
        }

        public NCMBObject Object(string name)
        {
            return new NCMBObject(this, name);
        }

        public NCMBQuery Query(string name)
        {
            return new NCMBQuery(this, name);
        }

        public NCMBUser User()
        {
            return new NCMBUser(this);
        }

        public void Logout()
        {
            SessionToken = null;
        }

    }
}
