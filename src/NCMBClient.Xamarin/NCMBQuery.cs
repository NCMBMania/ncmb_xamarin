using System;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Threading.Tasks;

namespace NCMBClient
{
    public class NCMBQuery
    {
        public string Name { get; }
        private JObject where;
        private int _limit;
        private string _order;
        private string _include;
        private int _skip;
        private Boolean _count;
        public int count;

        public static NCMB _ncmb;
        public NCMBQuery(string name)
        {
            this.Name = name;
            this.where = new JObject();
        }

        public NCMBQuery Order(string name, bool descending = true)
        {
            string symbol = "";
            if (descending)
            {
                symbol = "-";
            }
            if (_order == "")
            {
                _order = $"{symbol}${name}";
            } else
            {
                _order = $"${_order},{symbol}${name}";
            }
            return this;
        }

        public NCMBQuery Limit(int num)
        {
            _limit = num;
            return this;
        }

        public NCMBQuery Include(string name)
        {
            _include = name;
            return this;
        }

        public NCMBQuery Skip(int num)
        {
            _skip = num;
            return this;
        }

        public NCMBQuery EqualTo(string name, object value)
        {
            return SetOperand(name, value);
        }

        public NCMBQuery NotEqualTo(string name, object value)
        {
            return SetOperand(name, value, "$ne");
        }

        public NCMBQuery LessThan(string name, object value)
        {
            return SetOperand(name, value, "$lt");
        }

        public NCMBQuery LessThanOrEqualTo(string name, object value)
        {
            return SetOperand(name, value, "$lte");
        }

        public NCMBQuery GreaterThan(string name, object value)
        {
            return SetOperand(name, value, "$gt");
        }

        public NCMBQuery GreaterThanOrEqualTo(string name, object value)
        {
            return SetOperand(name, value, "$gte");
        }

        public NCMBQuery InString(string name, object value)
        {
            if (value.GetType().IsArray)
            {
                var ary = value as IEnumerable;
                var obj = new JArray();
                foreach (var val in ary)
                {
                    obj.Add(val);
                }
                return SetOperand(name, obj, "$in");
            } else
            {
                throw new Exception("Only allow array.");
            }
        }

        public NCMBQuery NotInString(string name, object value)
        {
            return SetOperand(name, value, "$nin");
        }

        public NCMBQuery Exists(string name, bool value = true)
        {
            return SetOperand(name, value, "$exists");
        }

        public NCMBQuery RegularExpressionTo(string name, object value)
        {
            return SetOperand(name, value, "$regex");
        }

        public NCMBQuery InArray(string name, object value)
        {
            return SetOperand(name, value, "$inArray");
        }

        public NCMBQuery NotInArray(string name, object value)
        {
            return SetOperand(name, value, "$ninArray");
        }

        public NCMBQuery AllInArray(string name, object value)
        {
            return SetOperand(name, value, "$all");
        }

        public NCMBQuery Near(string name, NCMBGeoPoint value)
        {
            return SetOperand(name, value.ToJson(), "$nearSphere");
        }

        public NCMBQuery WithinKilometers(string name, NCMBGeoPoint value, double maxDistance)
        {
            this.SetOperand(name, value.ToJson(), "$nearSphere");
            this.where[name]["$maxDistanceInKilometers"] = maxDistance;
            return this;
        }

        public NCMBQuery WithinMiles(string name, NCMBGeoPoint value, double maxDistance)
        {
            this.SetOperand(name, value.ToJson(), "$nearSphere");
            this.where[name]["$maxDistanceInMiles"] = maxDistance;
            return this;
        }

        public NCMBQuery WithinRadians(string name, NCMBGeoPoint value, double maxDistance)
        {
            this.SetOperand(name, value.ToJson(), "$nearSphere");
            this.where[name]["$maxDistanceInRadians"] = maxDistance;
            return this;
        }

        public NCMBQuery WithinSquare(string name, NCMBGeoPoint southWestVertex, NCMBGeoPoint northEastVertex)
        {
            var box = new JObject();
            var ary = new JArray();
            ary.Add(southWestVertex.ToJson());
            ary.Add(northEastVertex.ToJson());
            box.Add("$box", ary);
            return this.SetOperand(name, box, "$within");
        }

        public NCMBQuery RelatedTo(NCMBObject obj, string name)
        {
            var className = "";
            if (obj.GetType() == typeof(NCMBUser)) {
                className = "user";
            }else if (obj.GetType() == typeof(NCMBRole))
            {
                className = "role";
            }
            else if (obj.GetType() == typeof(NCMBInstallation))
            {
                className = "installation";
            } else
            {
                className = obj.Name;
            }
            var data = new JObject();
            data["object"] = obj.ToPointer();
            data["key"] = name;
            where["$relatedTo"] = data;
            return this;
        }

        public NCMBQuery Or(NCMBQuery query)
        {
            var queries = new NCMBQuery[1];
            queries[0] = query;
            return this.Or(queries);
        }

        public NCMBQuery Or(NCMBQuery[] queries)
        {
            var or = new JArray();
            foreach (var query in queries)
            {
                or.Add(query.where);
            }
            where["$or"] = or;
            return this;
        }

        public NCMBQuery Select(string name, string subKey, NCMBQuery query)
        {
            var condition = new JObject();
            var queryParams = new JObject();
            queryParams.Add("query", GetSubQueryCondition(query));
            queryParams.Add("key", subKey);
            condition.Add("$select", queryParams);
            where.Add(name, condition);
            return this;
        }

        public NCMBQuery InQuery(string name, NCMBQuery query)
        {
            
            var condition = new JObject();
            condition.Add("$inQuery", GetSubQueryCondition(query));
            where.Add(name, condition);
            return this;
        }

        private string GetSubQueryClassName(NCMBQuery query)
        {
            var className = "";
            if (query.Name == "users")
            {
                className = "user";
            }
            else if (query.Name == "roles")
            {
                className = "role";
            }
            else if (query.Name == "installations")
            {
                className = "installation";
            }
            else if (query.Name == "files")
            {
                className = "file";
            }
            else
            {
                className = query.Name;
            }
            return className;
        }

        private JObject GetSubQueryCondition(NCMBQuery query)
        {
            var className = GetSubQueryClassName(query);
            var parameters = new JObject();
            parameters.Add("className", className);
            parameters.Add("where", query.where);

            if (query._limit > 0)
            {
                parameters.Add("limit", query._limit);
            }
            if (query._skip > 0)
            {
                parameters.Add("skip", query._skip);
            }
            return parameters;
        }

        public NCMBQuery SetOperand(string name, object value, string ope = null)
        {
            if (value is DateTime)
            {
                var obj = new JObject();
                obj["__type"] = "Date";
                obj["iso"] = ((DateTime) value).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                value = obj;
            }

            if (ope == null)
            {
                if (value is string)
                {
                    where[name] = value.ToString();
                }
                else if (value is int)
                {
                    where[name] = new JValue(value);
                }
                else
                {
                    where[name] = (JToken)value;
                }
            }
            else
            {
                where[name] = new JObject();
                if (value is string)
                {
                    where[name][ope] = value.ToString();
                }
                else if (value is int)
                {
                    where[name][ope] = new JValue(value);
                }
                else
                {
                    where[name][ope] = (JToken)value;
                }
            }
            return this;
        }

        public NCMBObject[] FetchAll()
        {
            var r = GetClient();
            var results = r.Exec();
            return ConvertResults(results);
        }

        public async Task<NCMBObject[]> FetchAllAsync()
        {
            var r = GetClient();
            var results = await r.ExecAsync();
            return ConvertResults(results);
        }

        public NCMBObject[] FetchAllWithCount()
        {
            _count = true;
            var r = GetClient();
            var results = r.Exec();
            count = (int)results.GetValue("count");
            return ConvertResults(results);
        }

        public async Task<NCMBObject[]> FetchAllWithCountAsync()
        {
            _count = true;
            var r = GetClient();
            var results = await r.ExecAsync();
            count = (int)results.GetValue("count");
            return ConvertResults(results);
        }

        public int GetCount()
        {
            return count;
        }

        public NCMBObject Fetch()
        {
            _limit = 1;
            var r = GetClient();
            var results = r.Exec();
            return ConvertResults(results)[0];
        }

        public async Task<NCMBObject> FetchAsync()
        {
            _limit = 1;
            var r = GetClient();
            var results = await r.ExecAsync();
            return (NCMBObject) ConvertResults(results)[0];
        }

        private NCMBRequest GetClient()
        {
            var queries = new JObject();
            if (where.Count > 0)
            {
                queries.Add("where", where);
            }
            if (_limit > 0)
            {
                queries.Add("limit", _limit);
            }
            if (!(_order is null))
            {
                queries.Add("order", _order);
            }
            if (!(_include is null))
            {
                queries.Add("include", _include);
            }
            if (_skip > 0)
            {
                queries.Add("skip", _skip);
            }
            if (_count)
            {
                queries.Add("count", 1);
            }
            
            var r = new NCMBRequest();
            r.Name = Name;
            r.Queries = queries;
            r.Method = "GET";
            return r;
        }

        private NCMBObject[] ConvertResults(JObject results)
        {
            var ary = (JArray)results.GetValue("results");
            var count = ary.Count;
            var objs = new NCMBObject[count];
            var i = 0;
            foreach (var row in ary)
            {
                switch (Name)
                {
                    case "roles":
                        {
                            var obj = new NCMBRole();
                            obj.Sets((JObject)row);
                            objs[i] = obj;
                        }
                        break;
                    case "installations":
                        {
                            var obj = new NCMBInstallation();
                            obj.Sets((JObject)row);
                            objs[i] = obj;
                        }
                        break;
                    case "users":
                        {
                            var obj = new NCMBUser();
                            obj.Sets((JObject)row);
                            objs[i] = obj;
                        }
                        break;
                    default:
                        {
                            var obj = new NCMBObject(Name);
                            obj.Sets((JObject)row);
                            objs[i] = obj;
                        }
                        break;
                }
                i++;
            }
            return objs;
        }
    }
}
