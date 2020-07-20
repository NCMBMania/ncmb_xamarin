using System;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace ncmb_xamarin
{
    public class NCMBQuery
    {
        public string name;
        private JObject where;
        private int _limit;
        private NCMB _ncmb;
        public NCMBQuery(NCMB ncmb, string name)
        {
            this.name = name;
            this._ncmb = ncmb;
            this.where = new JObject();
        }

        public NCMBQuery limit(int num)
        {
            _limit = num;
            return this;
        }

        public NCMBQuery equalTo(string name, object value)
        {
            return setOperand(name, value);
        }

        public NCMBQuery notEqualTo(string name, object value)
        {
            return setOperand(name, value, "$ne");
        }

        public NCMBQuery lessThan(string name, object value)
        {
            return setOperand(name, value, "$lt");
        }

        public NCMBQuery lessThanOrEqualTo(string name, object value)
        {
            return setOperand(name, value, "$lte");
        }

        public NCMBQuery greaterThan(string name, object value)
        {
            return setOperand(name, value, "$gt");
        }

        public NCMBQuery greaterThanOrEqualTo(string name, object value)
        {
            return setOperand(name, value, "$gte");
        }

        public NCMBQuery inString(string name, object value)
        {
            return setOperand(name, value, "$in");
        }

        public NCMBQuery notInString(string name, object value)
        {
            return setOperand(name, value, "$nin");
        }

        public NCMBQuery exists(string name, bool value = true)
        {
            return setOperand(name, value, "$exists");
        }

        public NCMBQuery regularExpressionTo(string name, object value)
        {
            return setOperand(name, value, "$regex");
        }

        public NCMBQuery inArray(string name, object value)
        {
            return setOperand(name, value, "$inArray");
        }

        public NCMBQuery notInArray(string name, object value)
        {
            return setOperand(name, value, "$ninArray");
        }

        public NCMBQuery allInArray(string name, object value)
        {
            return setOperand(name, value, "$all");
        }

        public NCMBQuery setOperand(string name, object value, string ope = null)
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

        public NCMBObject[] find()
        {
            var queries = new JObject();
            if (where.Count > 0)
            {
                queries.Add("where", where);
            }
            var r = new NCMBRequest(_ncmb);
            var results = r.get(name, queries);
            var ary = (JArray)results.GetValue("results");
            var count = ary.Count;
            var objs = new NCMBObject[count];
            var i = 0;
            foreach (var row in ary)
            {
                var obj = _ncmb.Object(name);
                obj.sets((JObject) row);
                objs[i] = obj;
                i++;
            }
            return objs;
        }
    }
}
