using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix.Reflection;
using System.Dynamic;

namespace Dalia.AdoExtensions
{
    public static class DbDataReaderExtensions
    {
        public static T MapTo<T>(this IDataReader reader)
        {
            return (T)MapTo(reader, typeof(T));
        }

        public static T MapTo<T>(this IDataReader reader, object obj)
        {
            return (T)MapTo(reader, typeof(T), obj);
        }

        public static object MapTo(this IDataReader reader, Type type)
        {
            return MapTo(reader, type, null);
        }

        public static object MapTo(this IDataReader reader, Type type, object obj)
        {
            if (obj == null && type.IsClass)
                obj = ActivatorEx.CreateInstance(type);

            if (type.IsDbConvertible())
            {
                if (!reader.IsDBNull(0))
                    obj = reader[0];
            }
            else
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var prop = type.GetPropertyEx(reader.GetName(i));
                    if (prop != null)
                    {
                        var val = reader.IsDBNull(i) ? null : reader[i];

                        if (val == null)
                            if (!prop.PropertyInfo.PropertyType.IsValueType)
                                prop.Set(obj, null);
                            else
                                prop.Set(obj, Activator.CreateInstance(prop.PropertyInfo.PropertyType));
                        else
                            prop.Set(obj, val);
                    }
                }
            }
            return obj;
        }

        public static ExpandoObject ToExpandoObject(this IDataReader reader)
        {
            var o = new ExpandoObject() as IDictionary<string, object>;
            for (int i = 0; i < reader.FieldCount; i++)
                o.Add(reader.GetName(i), reader.GetValue(i));
            return (ExpandoObject)o;
        }

        public static IDictionary<string, object> ToDictionary(this IDataReader reader)
        {
            var o = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
                o.Add(reader.GetName(i), reader.GetValue(i));
            return o;
        }
    }
}

