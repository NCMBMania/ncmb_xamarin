using System;
namespace NCMBClient
{
    public class NCMBPush: NCMBObject
    {
        public NCMBPush() : base("push")
        {
        }

        static public NCMBQuery Query()
        {
            return new NCMBQuery("push");
        }

        new public Boolean Save()
        {
            if (this.Get("deliveryTime") == null && this.Get("immediateDeliveryFlag") == null)
            {
                throw new Exception("deliveryTime or immediateDeliveryFlag is required.");
            }
            if (Array.IndexOf((string[]) this.Get("target"), "ios") == -1 && Array.IndexOf((string[])this.Get("target"), "android") == -1)
            {
                throw new Exception($"target allows only ios or android.");
            }
            return base.Save();
        }

    }
}
