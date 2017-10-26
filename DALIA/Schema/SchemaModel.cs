using Dalia.Internal;
using Dynamix.Expressions;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public class SchemaModel : IEntityKeyProvider
    {
        DualKeyDictionary<string, Type, IDataModelDescriptor> entities
            = new DualKeyDictionary<string, Type, IDataModelDescriptor>();

        static object dlock = new object();

        public void Register(IDataModelDescriptor entitySchemaModel)
        {

            bool exists = false;
            if (!(exists = entities.ContainsKey2(entitySchemaModel.EntityType)))
            {
                lock (dlock)
                {
                    if (!(exists = entities.ContainsKey2(entitySchemaModel.EntityType)))
                        entities.Add(entitySchemaModel.EntityType.Name, entitySchemaModel.EntityType, entitySchemaModel);
                }
            }

            if (exists)
                throw new Exception("Type " + entitySchemaModel.EntityType.Name + " has already been registerd");
        }

        public IDataModelDescriptor Get(string Name)
        {
            IDataModelDescriptor e = null;
            if (entities.TryGetValue(Name, out e))
                return e;
            else
                throw new ArgumentException("No entity named: " + Name + " exists in SchemaModel");
        }

        public IDataModelDescriptor Get(Type Type)
        {
            IDataModelDescriptor e = null;
            if (entities.TryGetValue(Type, out e))
                return e;
            else
                throw new ArgumentException("No entity of Type: " + Type.Name + " exists in SchemaModel");
        }

        public IDataModelDescriptor Get<T>()
        {
            return Get(typeof(T));
        }

        public object GetKey(object entity)
        {
            throw new NotImplementedException();
        }

        public IDataModelDescriptor TryGetOrNull(string Name)
        {
            IDataModelDescriptor e = null;
            entities.TryGetValue(Name, out e);
            return e;
        }

        public IDataModelDescriptor TryGetOrNull(Type Type)
        {
            IDataModelDescriptor e = null;
            entities.TryGetValue(Type, out e);
            return e;
        }

        public IDataModelDescriptor TryGetOrNull<T>()
        {
            return TryGetOrNull(typeof(T));
        }

        public bool ContainsType(Type type)
        {
            return entities.ContainsKey2(type);
        }

        public virtual IReadOnlyList<PropertyInfoEx> GetKeyProperties(object entity)
        {
            return Get(entity.GetType()).Keys
                .Select(x => x.PropertyInfoEx).ToList().AsReadOnly();
        }

        public IReadOnlyDictionary<string, PropertyInfoEx> GetKeyPropertiesDictionary(object entity)
        {
            return Get(entity.GetType()).Keys
                .ToDictionary(x => x.Name, x => x.PropertyInfoEx);
        }

        public IReadOnlyList<object> GetKeyValues(object entity)
        {
            return Get(entity.GetType()).Keys
                .Select(x => x.PropertyInfoEx.Get(entity))
                .ToList().AsReadOnly();
        }

        public IReadOnlyDictionary<PropertyInfoEx, object> GetKeyPropertyDictionary(object entity)
        {
            return Get(entity.GetType()).Keys
                .ToDictionary(x => x.PropertyInfoEx, x => x.PropertyInfoEx.Get(entity));
        }

        public IReadOnlyDictionary<string, object> GetKeyDictionary(object entity)
        {
            return Get(entity.GetType()).Keys
                .ToDictionary(x => x.Name, x => x.PropertyInfoEx.Get(entity));
        }

        public IReadOnlyDictionary<string, object> GetKeyColumnDictionary(object entity)
        {
            return Get(entity.GetType()).Keys
                .ToDictionary(x => x.ColumnName, x => x.PropertyInfoEx.Get(entity));
        }

        public Expression<Func<T, bool>> GetKeyPredicate<T>(T entity)
        {
            return ExpressionBuilder.GetPredicate(entity, GetKeyProperties(entity));
        }

        public Expression<Func<T, bool>> GetKeyPredicate<T>(IEnumerable<T> entities)
        {
            return ExpressionBuilder.GetPredicate(entities, GetKeyProperties(entities.First()));
        }
        public Expression<Func<T, bool>> GetKeyPredicateFromQueryParameters<T>(QueryParameters queryParameters)
        {
            var keyProperties = Get<T>().Keys;
            if (queryParameters.Count < keyProperties.Count)
                throw new ArgumentException("Parameters infered from keyValue are less than the number of key properties for model");

            List<object> values;
            if (!queryParameters.HasNamedParameters)
            {
                values = queryParameters.Select(x => x.Value).ToList();
            }
            else
            {
                values = new List<object>();
                foreach (var key in keyProperties)
                {
                    try
                    {
                        values.Add(queryParameters[key.Name]);
                    }
                    catch//(Exception e)
                    {
                        throw new ArgumentException("Key property " + key.Name + " missing form keyValue");
                    }
                }
            }
            var keys = Get<T>().Keys.Select((x, i) => new Tuple<PropertyInfoEx, object>(x.PropertyInfoEx, values[i]));

            return ExpressionBuilder.GetPredicate<T>(keys);
        }

        public Expression<Func<T, bool>> GetKeyPredicateFromValues<T>(object keyValue)
        {
            return GetKeyPredicateFromQueryParameters<T>(QueryParameters.InferFrom(keyValue));
        }
    }
}