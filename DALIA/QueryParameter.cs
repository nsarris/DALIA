using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Dalia.AdoExtensions;
using Dynamix.Reflection;

namespace Dalia
{
    public class QueryParameter
    {
        public string Name { get; internal set; }
        private object value;
        public object Value
        {
            get => value;
            set
            {
                if (value != null && !value.GetType().IsDbConvertible())
                    throw new ArgumentException("value has to be a scalar/value type");

                this.value = value;
            }
        }


        public QueryParameter(string name)
        {
            Name = name;
        }

        public QueryParameter(object value, string name = null)
        {
            Name = name;
            Value = value;
        }

    }

    public class QueryParameters : KeyedCollection<string, QueryParameter>
    {
        public QueryParameters()
        {

        }

        public QueryParameters(IEnumerable<QueryParameter> queryParameters)
        {
            foreach (var p in queryParameters) Add(p);
        }
        public bool HasNamedParameters { get; set; }

        public static QueryParameters FromKeyValuePairs(IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            var q = new QueryParameters();
            foreach (var t in keyValuePairs)
            {
                q.Add(new QueryParameter(t.Value, t.Key));
            }
            q.HasNamedParameters = true;
            return q;
        }

        public static QueryParameters FromDictionary(IDictionary<string, object> dictionary)
        {
            var q = new QueryParameters();
            foreach (var t in dictionary)
            {
                q.Add(new QueryParameter(t.Value, t.Key));
            }
            q.HasNamedParameters = true;
            return q;
        }

        public static QueryParameters FromTuples(IEnumerable<Tuple<string, object>> tuples)
        {
            var q = new QueryParameters();
            foreach (var t in tuples)
            {
                q.Add(new QueryParameter(t.Item2, t.Item1));
            }
            q.HasNamedParameters = true;
            return q;


        }

        public static QueryParameters FromObjectProperties(object value)
        {
            if (value == null) throw new ArgumentNullException("value cannot be null");
            var q = new QueryParameters();
            foreach (var prop in value.GetType().GetPropertiesEx().Where(x => x.Type.IsDbConvertible()))
            {
                q.Add(new QueryParameter(prop.Get(value), prop.Name));
            }
            q.HasNamedParameters = true;
            return q;
        }

        public static QueryParameters InferFrom(object value)
        {
            if (value == null || value.GetType().IsDbConvertible())
                return FromValue(value);


            if (value is IEnumerable<Tuple<string, object>> tuples)
                return FromTuples(tuples);
            //else if (value is IEnumerable<(string, object)> valuetuples)
            //  return From(valuetuples);
            else if (value is IDictionary<string, object> dictionary)
                return FromDictionary(dictionary);
            else if (value is IEnumerable<KeyValuePair<string, object>> keyValuePairs)
                return FromKeyValuePairs(keyValuePairs);
            else if (value is IEnumerable<QueryParameter> qpvalues)
                return new QueryParameters(qpvalues);
            else if (value is IEnumerable<object> values)
                return FromValues(values);
            else
                return FromObjectProperties(value);
        }

        public static QueryParameters FromValues(IEnumerable<object> values)
        {
            var q = new QueryParameters();
            foreach (var v in values)
            {
                q.Add(new QueryParameter(v, null));
            }
            //q.HasNamedParameters = false;

            return q;
        }

        public static QueryParameters FromValues(params object[] values)
        {
            return FromValues(values);
        }

        public static QueryParameters FromValue(object value, string name = null)
        {
            var q = new QueryParameters
            {
                new QueryParameter(value, name)
            };
            return q;
        }

        protected override string GetKeyForItem(QueryParameter item)
        {
            if (string.IsNullOrEmpty(item.Name))
                item.Name = "p" + this.Count + 1;

            return item.Name;
        }
    }
}
