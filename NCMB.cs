using System;
namespace ncmb_xamarin
{
    public class NCMB
    {
        public string application_key;
        public string client_key;
        public string sessionToken;

        public NCMB(string application_key, string client_key)
        {
            this.application_key = application_key;
            this.client_key = client_key;
        }

        public NCMBObject Object(string name)
        {
            return new NCMBObject(this, name);
        }

        public NCMBQuery Query(string name)
        {
            return new NCMBQuery(this, name);
        }
    }
}
