using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;

namespace NCMBClient
{
    public class NCMBRelation
    {
        private string __op;

        private ArrayList _objects;
        public NCMBRelation()
        {
            _objects = new ArrayList();
        }

        public NCMBRelation Add(NCMBObject obj)
        {
            this.__op = "AddRelation";
            this._objects.Add(obj);
            return this;
        }

        public NCMBRelation Remove(NCMBObject obj)
        {
            this.__op = "RemoveRelation";
            this._objects.Add(obj);
            return this;
        }

        public JObject ToJson()
        {
            var json = new JObject();
            json["__op"] = this.__op;
            var objects = new JArray();
            for (int i = 0; i < _objects.Count; i++)
            {
                objects.Add(((NCMBObject)_objects[i]).ToPointer());
            }
            json["objects"] = objects;
            return json;
        }
    }
}
