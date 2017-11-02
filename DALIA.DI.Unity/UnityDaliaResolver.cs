using System;
using System.Linq;
using Dalia;
using Unity;
using Dalia.DI;
using Dalia.Repositories;
using Unity.Resolution;
using Dynamix.Reflection;
using System.Collections.Generic;

namespace Dalia.DI.Unity
{
    public class UnityDaliaResolver : IUnityDaliaResolver
    {
        internal static readonly string ResolveNewSuffix = "_new";

        IUnityContainer container;

        public UnityDaliaResolver(IUnityContainer container)
        {
            this.container = container;

        }

        internal static IDataSource GetDefaultDataSource(IUnityContainer container, Type contextType)
        {
            var key = (contextType.GetCustomAttributes(typeof(DefaultDataSourceAttribute), false).FirstOrDefault() as DefaultDataSourceAttribute)?.Key;
            if (key == null) throw new Exception("No default ds found");
            var ds = container.Resolve<IDataSource>(key);
            if (ds == null) throw new Exception("No ds found with key" + key);

            return ds;
        }


        public TContext ResolveSingleContext<TContext>()
            where TContext : IDataContextAsync
        {
            return container.Resolve<TContext>();
        }

        public TContext ResolveSingleContext<TContext>(string dataSourceKey)
            where TContext : IDataContextAsync
        {
            return container.Resolve<TContext>(dataSourceKey);
        }

        public TContext ResolveNewContext<TContext>()
            where TContext : IDataContextAsync
        {
            var dataSourceKey = GetDefaultDataSource(container, typeof(TContext)).Key;
            return container.Resolve<TContext>(dataSourceKey + ResolveNewSuffix);
        }

        public TContext ResolveNewContext<TContext>(string dataSourceKey)
            where TContext : IDataContextAsync
        {
            if (string.IsNullOrEmpty(dataSourceKey))
                dataSourceKey = GetDefaultDataSource(container, typeof(TContext)).Key;

            return container.Resolve<TContext>(dataSourceKey + ResolveNewSuffix);
        }

        public TRepository ResolveRepository<TRepository>(IEnumerable<IDataContextAsync> contexts) where TRepository : IRepository
        {
            return container.Resolve<TRepository>(contexts.Select(x => new DependencyOverride(contexts.GetType(), contexts)).ToArray());
        }

        public TRepository ResolveRepository<TRepository>(params IDataContextAsync[] contexts) where TRepository : IRepository
        {
            return ResolveRepository<TRepository>(contexts);
        }

        public TRepository ResolveRepository<TRepository>(bool useOwnContext = false) where TRepository : IRepository
        {
            if (!useOwnContext)
                return container.Resolve<TRepository>();
            else
            {
                var contextOverrides = new List<DependencyOverride>();

                foreach (var p in typeof(TRepository).GetConstructorsEx().Aggregate((x1, x2) => x1.Signature.Count() > x2.Signature.Count() ? x1 : x2)
                    .Signature.Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Any(i => i == typeof(IDataContextAsync))))
                {
                    var dataSourceKey = GetDefaultDataSource(container, p).Key + ResolveNewSuffix;
                    contextOverrides.Add(new DependencyOverride(p, container.Resolve(p, dataSourceKey)));
                }

                return container.Resolve<TRepository>(contextOverrides.ToArray());
            }
        }

        public TRepository ResolveRepository<TRepository>(string dataSourceKey, bool useOwnContext = false) where TRepository : IRepository
        {
            var contextOverrides = new List<DependencyOverride>();

            foreach (var p in typeof(TRepository).GetConstructorsEx().Aggregate((x1, x2) => x1.Signature.Count() > x2.Signature.Count() ? x1 : x2)
                .Signature.Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Any(i => i == typeof(IDataContextAsync))))
            {
                contextOverrides.Add(new DependencyOverride(p, container.Resolve(p, dataSourceKey + (useOwnContext ? ResolveNewSuffix : ""))));
            }

            return container.Resolve<TRepository>(contextOverrides.ToArray());
        }

        public TQueryProvider ResolveQueryProvider<TQueryProvider>(params IDataContextAsync[] contexts) where TQueryProvider : Repositories.IQueryProvider
        {
            return ResolveQueryProvider<TQueryProvider>(contexts);
        }


        public TQueryProvider ResolveQueryProvider<TQueryProvider>(IEnumerable<IDataContextAsync> contexts) where TQueryProvider : Repositories.IQueryProvider
        {
            return container.Resolve<TQueryProvider>(contexts.Select(x => new DependencyOverride(contexts.GetType(), contexts)).ToArray());
        }

        public TQueryProvider ResolveQueryProvider<TQueryProvider>(bool useOwnContext = false) where TQueryProvider : Repositories.IQueryProvider
        {
            if (!useOwnContext)
                return container.Resolve<TQueryProvider>();
            else
            {
                var contextOverrides = new List<DependencyOverride>();

                foreach (var p in typeof(TQueryProvider).GetConstructorsEx().Aggregate((x1, x2) => x1.Signature.Count() > x2.Signature.Count() ? x1 : x2)
                    .Signature.Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Any(i => i == typeof(IDataContextAsync))))
                {
                    var dataSourceKey = GetDefaultDataSource(container, p).Key + ResolveNewSuffix;
                    contextOverrides.Add(new DependencyOverride(p, container.Resolve(p, dataSourceKey)));
                }

                return container.Resolve<TQueryProvider>(contextOverrides.ToArray());
            }
        }

        public TQueryProvider ResolveQueryProvider<TQueryProvider>(string dataSourceKey, bool useOwnContext = false) where TQueryProvider : Repositories.IQueryProvider
        {
            var contextOverrides = new List<DependencyOverride>();

            foreach (var p in typeof(TQueryProvider).GetConstructorsEx().Aggregate((x1, x2) => x1.Signature.Count() > x2.Signature.Count() ? x1 : x2)
                .Signature.Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Any(i => i == typeof(IDataContextAsync))))
            {
                contextOverrides.Add(new DependencyOverride(p, container.Resolve(p, dataSourceKey + (useOwnContext ? ResolveNewSuffix : ""))));
            }

            return container.Resolve<TQueryProvider>(contextOverrides.ToArray());
        }

        
    }
    
}

