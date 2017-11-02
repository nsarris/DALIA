using Dalia.Repositories;
using System.Collections.Generic;

namespace Dalia.DI.Unity
{
    public interface IDaliaResolver
    {
        TContext ResolveNewContext<TContext>() where TContext : IDataContextAsync;
        TContext ResolveNewContext<TContext>(string dataSourceKey) where TContext : IDataContextAsync;
        TContext ResolveSingleContext<TContext>() where TContext : IDataContextAsync;
        TContext ResolveSingleContext<TContext>(string dataSourceKey) where TContext : IDataContextAsync;

        TRepository ResolveRepository<TRepository>(IEnumerable<IDataContextAsync> contexts) where TRepository : IRepository;
        TRepository ResolveRepository<TRepository>(params IDataContextAsync[] contexts) where TRepository : IRepository;
        TRepository ResolveRepository<TRepository>(bool useOwnContext = false) where TRepository : IRepository;
        TRepository ResolveRepository<TRepository>(string dataSourceKey, bool useOwnContext = false) where TRepository : IRepository;

        TQueryProvider ResolveQueryProvider<TQueryProvider>(IEnumerable<IDataContextAsync> contexts) where TQueryProvider : IQueryProvider;
        TQueryProvider ResolveQueryProvider<TQueryProvider>(params IDataContextAsync[] contexts) where TQueryProvider : IQueryProvider;
        TQueryProvider ResolveQueryProvider<TQueryProvider>(bool useOwnContext = false) where TQueryProvider : IQueryProvider;
        TQueryProvider ResolveQueryProvider<TQueryProvider>(string dataSourceKey, bool useOwnContext = false) where TQueryProvider : IQueryProvider;
    }
}