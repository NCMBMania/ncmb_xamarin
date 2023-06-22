using NUnit.Framework;
using System;
using NCMBClient;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace NCMBClientTest
{
    [TestFixture()]
    public class NCMBGeoPointTest: Test
    {
        public NCMBGeoPointTest(): base()
        {
        }

        [Test()]
        public void TestCreateGeoPoint()
        {
            var latitude = 35.6585805;
            var longitude = 139.7454329;
            var geo = new NCMBGeoPoint(latitude, longitude);
            Assert.AreEqual(latitude, geo.Latitude);
        }

        [Test()]
        public void TestSetGeoPoint()
        {
            var latitude = 35.6585805;
            var longitude = 139.7454329;
            var geo = new NCMBGeoPoint(latitude, longitude);
            var item = new NCMBObject("Item");
            item.Set("geo", geo);
            var geo1 = (NCMBGeoPoint)item.Get("geo");
            Assert.AreEqual(geo1.Latitude, geo.Latitude);
        }

        [Test()]
        public void TestSaveGeoPoint()
        {
            Task.Run(async () =>
            {
                var latitude = 35.6585805;
                var longitude = 139.7454329;
                var geo = new NCMBGeoPoint(latitude, longitude);
                var item = new NCMBObject("Item");
                item.Set("geo", geo);
                await item.Save();
                Assert.NotNull(item.Get("objectId"));
                await item.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSaveAndGetGeoPoint()
        {
            Task.Run(async () =>
            {
                var latitude = 35.6585805;
                var longitude = 139.7454329;
                var geo = new NCMBGeoPoint(latitude, longitude);
                var item = new NCMBObject("Item");
                item.Set("geo", geo);
                await item.Save();
                await item.Fetch();
                var geo1 = (NCMBGeoPoint)item.Get("geo");
                Assert.NotNull(item.Get("objectId"));
                Assert.AreEqual(geo1.Latitude, geo.Latitude);
                await item.Delete();
            }).GetAwaiter().GetResult();
        }

        [Test()]
        public void TestSearchGeoPoints()
        {
            Task.Run(async () =>
            {
                var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));

                var text = File.ReadAllText(dir + "/yamanote.json");
                var json = JArray.Parse(text);
                foreach (var x in json)
                {
                    var p = (JObject)x;
                    var geo = new NCMBGeoPoint((double)p["latitude"], (double)p["longitude"]);
                    var item = new NCMBObject("Station");
                    await item.Set("name", p["name"].ToString()).Set("geo", geo).Save();
                }


                var query = new NCMBQuery("Station");

                var geo1 = new NCMBGeoPoint(35.6585805, 139.7454329);
                var ary = await query.Limit(5).Near("geo", geo1).FetchAll();

                Assert.AreEqual(5, ary.Length);
                Assert.AreEqual("浜松町", ((NCMBObject)ary[0]).Get("name").ToString());

                var geo2 = new NCMBGeoPoint(35.6654861, 139.7684781);
                var geo3 = new NCMBGeoPoint(35.6799926, 139.7357476);
                query = new NCMBQuery("Station");
                ary = await query.WithinSquare("geo", geo2, geo3).FetchAll();
            
                Assert.AreEqual(2, ary.Length);
                query = new NCMBQuery("Station");
                var stations = await query.Limit(100).FetchAll();
                foreach (var station in stations)
                {
                    await station.Delete();
                }
            }).GetAwaiter().GetResult();
        }
    }
}