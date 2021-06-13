using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace NCMBClient
{
    public class NCMBFile: NCMBObject
    {
        public byte[] data;
        public string MimeType;
        public NCMBFile(string FileName, byte[] data, string MimeType = "application/octet-stream") : base("files")
        {
            Set("fileName", FileName);
            this.MimeType = MimeType;
            this.data = data;
        }

        static public NCMBQuery Query()
        {
            return new NCMBQuery("files");
        }

        new async public Task<bool> Save()
        {
            NCMBRequest r = new NCMBRequest();
            r.Name = Name;
            r.Method = "POST";
            r.Data = data;
            r.Fields = GetData();
            r.MimeType = MimeType;
            r.ObjectId = GetString("fileName");
            var response = await r.Exec();
            Sets(response);
            return true;
        }
    }
}
