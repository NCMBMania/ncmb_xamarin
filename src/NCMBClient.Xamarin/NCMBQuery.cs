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
        private NCMB _ncmb;
        public NCMBQuery(NCMB ncmb, string name)
        {
            this.Name = name;
            this._ncmb = ncmb;
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
            return SetOperand(name, value, "$in");
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

        public NCMBObject[] FindAll()
        {
            var r = GetClient();
            var results = r.Exec();
            return ConvertResults(results);
        }

        public async Task<NCMBObject[]> FindAllAsync()
        {
            var r = GetClient();
            var results = await r.ExecAsync();
            return ConvertResults(results);
        }

        public NCMBObject Find()
        {
            _limit = 1;
            var r = GetClient();
            var results = r.Exec();
            return ConvertResults(results)[0];
        }

        public async Task<NCMBObject> FindAsync()
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
            var r = new NCMBRequest(_ncmb);
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
                var obj = _ncmb.Object(Name);
                obj.Sets((JObject)row);
                objs[i] = obj;
                i++;
            }
            return objs;
        }
    }
}
