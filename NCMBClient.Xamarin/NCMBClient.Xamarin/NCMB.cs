using System;
namespace NCMBClient
{
    public class NCMB
    {
        public string ApplicationKey { get; }
        public string ClientKey { get; }
        public string SessionToken { get; }

        public NCMB(string applicationKey, string clientKey)
        {
            this.ApplicationKey = applicationKey;
            this.ClientKey = clientKey;
        }

        public NCMBObject Object(string name)
        {
            return new NCMBObject(this, name);
        }
    }
}
