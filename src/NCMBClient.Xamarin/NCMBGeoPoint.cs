using System;
using Newtonsoft.Json.Linq;
namespace NCMBClient
{
    public class NCMBGeoPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public NCMBGeoPoint(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public JObject ToJson()
        {
            var data = new JObject();
            data.Add("__type", "GeoPoint");
            data.Add("latitude", Latitude);
            data.Add("longitude", Longitude);
            return data;
        }
    }
}
