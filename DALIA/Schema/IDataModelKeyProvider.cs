using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public interface IEntityKeyProvider
    {
        object GetKey(object entity);
        IReadOnlyList<PropertyInfoEx> GetKeyProperties(object entity);
        IReadOnlyDictionary<string, PropertyInfoEx> GetKeyPropertiesDictionary(object entity);
        IReadOnlyList<object> GetKeyValues(object entity);
        IReadOnlyDictionary<PropertyInfoEx, object> GetKeyPropertyDictionary(object entity);
        IReadOnlyDictionary<string, object> GetKeyDictionary(object entity);
        IReadOnlyDictionary<string, object> GetKeyColumnDictionary(object entity);
        Expression<Func<T, bool>> GetKeyPredicateFromValues<T>(object keyValue);
        Expression<Func<T, bool>> GetKeyPredicate<T>(T entity);
        Expression<Func<T, bool>> GetKeyPredicate<T>(IEnumerable<T> entities);
        
    }
}
