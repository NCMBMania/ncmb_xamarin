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
            NCMBObject._ncmb = this;
            NCMBQuery._ncmb = this;
            NCMBUser._ncmb = this;
            NCMBRequest._ncmb = this;
            NCMBFile._ncmb = this;
        }
    }
}
