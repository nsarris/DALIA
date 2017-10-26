using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalia.AsyncExtensions;

namespace Dalia.Repositories
{
    public interface IDataModelQueryProvider<T>
        where T : class
    {
        bool SupportsAsync { get; }
        T GetById(object id);
        Task<T> GetByIdAsync(object id);
        bool SupportsQueryable { get; }
        IQueryable<T> ToQueryable();
    }

    public interface IDataModelQueryProvider<T,TKey> : IDataModelQueryProvider<T>
         where T : class
    {
        T GetById(TKey id);
        Task<T> GetByIdAsync(TKey id);
    }
}
