using Dalia.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.DI
{
    public interface IRepositoryFactory
    {
        IQueryProvider<T> CreateQueryProvider<T>() where T : class;
        IQueryProvider<T, TKey> CreateQueryProvider<T, TKey>() where T : class;
        TQueryProvider ResolveQueryProvider<TQueryProvider>();
        
        IRepository<T> CreateRepository<T>() where T : class;
        IRepository<T, TKey> CreateRepository<T, TKey>() where T : class;
        TRepository ResolveRepository<TRepository>();
    }
}
