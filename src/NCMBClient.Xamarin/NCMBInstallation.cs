using System;
namespace NCMBClient
{
    public class NCMBInstallation : NCMBObject
    {
        public NCMBInstallation(): base("installations")
        {
        }

        static public NCMBQuery Query()
        {
            return new NCMBQuery("installations");
        }

        new public Boolean Save()
        {
            Console.WriteLine("Save in Installation");
            var ary = new string[] { "deviceToken", "deviceType" };
            foreach (var key in ary)
            {
                if (!this._fields.ContainsKey(key))
                {
                    throw new Exception($"{key} is required.");
                }
            }
            var deviceType = new string[] { "ios", "android" };
            if (Array.IndexOf(deviceType, this.Get("deviceType"), 0) == -1)
            {
                throw new Exception($"deviceType allows only ios or android.");
            }
            return base.Save();
        }
    }
}
