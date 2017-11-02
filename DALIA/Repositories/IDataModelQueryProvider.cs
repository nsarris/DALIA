using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalia.AsyncExtensions;

namespace Dalia.Repositories
{
    public interface IQueryProvider : IDisposable { }
    

    public interface IQueryProvider<T> : IQueryProvider
        where T : class
    {
        bool OwnsContext { get; set; }
        bool SupportsAsync { get; }
        T GetById(object id);
        Task<T> GetByIdAsync(object id);
        bool SupportsQueryable { get; }
        IQueryable<T> ToQueryable();
        IDataContextAsync Context { get; }
    }

    public interface IQueryProvider<T,TKey> : IQueryProvider<T>
         where T : class
    {
        T GetById(TKey id);
        Task<T> GetByIdAsync(TKey id);
    }
}
