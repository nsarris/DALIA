using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dalia
{
    public class Delta<T>
    {
        static Dictionary<string, PropertyInfoEx> propertyCache;
        static string GetMemberName<TMember>(Expression<Func<T, TMember>> expression)
        {
            var member = expression.Body as MemberExpression;

            if (member == null && expression.Body is UnaryExpression)
                member = ((UnaryExpression)expression.Body).Operand as MemberExpression;

            if (member != null)
                return member.Member.Name;

            throw new ArgumentException("Expression is not a member accessor", "expression");
        }

        static bool IsConvertibleTo(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            IConvertible convertible = value as IConvertible;

            if (convertible == null)
            {
                return false;
            }

            return true;
        }

        static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        static object olock = new object();
        static Dictionary<string, PropertyInfoEx> GetProperties()
        {
            if (propertyCache == null)
            {
                lock (olock)
                {
                    if (propertyCache == null)
                        propertyCache = typeof(T).GetProperties().ToDictionary(x => x.Name, x => new PropertyInfoEx(x));
                }
            }
            return propertyCache;

        }

        Dictionary<string, object> changedValues = new Dictionary<string, object>();

        public T KeyValues { get; private set; }

        public Delta()
        {

        }

        public Delta(T keyValues)
        {
            this.KeyValues = keyValues;
        }

        //public Delta(T newValues, T oldValues)
        //{
        //    foreach (var p in GetProperties().Values)
        //    {
        //        //TODO
        //        //if (p.PropertyInfo.PropertyType.IsDbPrimitive())
        //        //{
        //        //    var newVal = p.Get(newValues);
        //        //    var oldVal = p.Get(oldValues);

        //        //    if (newVal != oldVal)
        //        //        changedValues[p.PropertyInfo.Name] = newVal;
        //        //}
        //    }
        //}

        public Delta(T source, IEnumerable<Expression<Func<T, object>>> props)
        {
            foreach (var p in props)
            {
                this.SetValue(p, source);
            }
        }

        public Delta<T> SetValue<TMember>(Expression<Func<T, TMember>> selector, TMember value)
        {
            var member = GetMemberName(selector);
            //PropertyInfo prop = null;
            //if (!GetProperties().TryGetValue(member, out prop))
            //    throw new Exception("Member is not")
            //var initval = initialValues.TryGetValue()
            changedValues[member] = value;

            return this;
        }

        public Delta<T> SetValue<TMember>(Expression<Func<T, TMember>> selector, T source)
        {
            var member = GetMemberName(selector);
            var prop = GetProperties()[member];
            //PropertyInfo prop = null;
            //if (!GetProperties().TryGetValue(member, out prop))
            //    throw new Exception("Member is not")
            //var initval = initialValues.TryGetValue()
            changedValues[member] = prop.Get(source);

            return this;
        }

        public Delta<T> SetValues(IDictionary<string, object> values)
        {
            foreach (var item in values)
            {
                PropertyInfoEx prop = null;
                if (GetProperties().TryGetValue(item.Key, out prop)
                    && prop.CanGet)
                {
                    //if (IsConvertibleTo(item.Value,prop.PropertyType))
                    changedValues[item.Key] = ChangeType(item.Value, prop.PropertyInfo.PropertyType);
                    //else
                    //    throw new ArgumentException("Ivalid value Member [" + member + "] is not a valid property of Type [" + typeof(T).Name + "]");
                }
            }
            return this;
        }

        public Delta<T> SetValues(object values)
        {
            foreach (var p in values.GetType().GetProperties())
            {
                PropertyInfoEx prop = null;
                if (GetProperties().TryGetValue(p.Name, out prop)
                    && p.GetGetMethod() != null)
                    //throw new ArgumentException("Member [" + member + "] is not a valid property of Type [" + typeof(T).Name + "]");
                    changedValues[p.Name] = ChangeType(p.GetValue(values, null), prop.PropertyInfo.PropertyType);
            }
            return this;
        }

        public Dictionary<string, object> GetChangedValues()
        {
            return changedValues.ToDictionary(x => x.Key, x => x.Value);
        }

        public void Patch(T obj)
        {
            foreach (var item in changedValues)
            {
                var prop = GetProperties()[item.Key];
                prop.Set(obj, item.Value);
            }
        }

        public T CreateAndPatch()
        {
            var o = Activator.CreateInstance<T>();
            Patch(o);
            return o;
        }

        public bool Contains(string propertyName)
        {
            return changedValues.ContainsKey(propertyName);
        }
    }

}
