using System;
using System.Collections.Generic;
using System.Linq;
using Dalia;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Dalia.Mapping;

namespace Dalia.DI.Unity
{
    public class UnityDaliaResolverBoostraper
    {
        IUnityContainer container;
        public IDataSources DataSources { get; }
    
        public UnityDaliaResolverBoostraper(IUnityContainer container, IDataSources dataSources, IObjectMapper mapper)
        {
            this.container = container;
            DataSources = dataSources;
            container.RegisterInstance(mapper);
            container.RegisterInstance(dataSources);

            container.RegisterType(from: typeof(IDaliaResolver), to : typeof(UnityDaliaResolver));
            container.RegisterType(from: typeof(IUnityDaliaResolver), to: typeof(UnityDaliaResolver));
            container.RegisterType<UnityDaliaResolver>(new HierarchicalLifetimeManager(), new InjectionFactory(c => new UnityDaliaResolver(c)));

            foreach (var ds in dataSources.Values)
            {
                container.RegisterInstance(ds.Key, ds);
                RegisterDbConnection(container, ds.DbConnectionType, ds.Key);
            }
        }

        private void RegisterDbConnection(IUnityContainer container, Type dbConnectionType, string dataSourceKey)
        {
            container.RegisterType(dbConnectionType,
                    dataSourceKey,
                    new HierarchicalLifetimeManager(),
                    new InjectionConstructor(new ResolvedParameter<string>()),
                    new InjectionFactory(c =>
                    {
                        var dataSource = c.Resolve<IDataSource>(dataSourceKey);
                        return Activator.CreateInstance(dataSource.DbConnectionType, dataSource.ConnectionString);
                    }));

            container.RegisterType(dbConnectionType,
                dataSourceKey + UnityDaliaResolver.ResolveNewSuffix,
                new PerResolveLifetimeManager(),
                new InjectionConstructor(new ResolvedParameter<string>()),
                new InjectionFactory(c =>
                {
                    var dataSource = c.Resolve<IDataSource>(dataSourceKey);
                    return Activator.CreateInstance(dataSource.DbConnectionType, dataSource.ConnectionString);
                }));
        }

        public void RegisterDaliaType(Type daliaType, IEnumerable<Type> constructorSignature, Func<IUnityContainer, IDataSource, string, object> unityFactoryMethod)
        {
            var dataSources = container.Resolve<IDataSources>();
            var ctor = new InjectionConstructor(constructorSignature.Select(x => new ResolvedParameter(x)).ToArray());


            //Default
            container.RegisterType(daliaType,
                new HierarchicalLifetimeManager(),
                ctor,
                new InjectionFactory(c =>
                {
                    var defaultDataSource = UnityDaliaResolver.GetDefaultDataSource(c, daliaType);
                    return unityFactoryMethod(c, defaultDataSource, defaultDataSource.Key);
                }));

            foreach (var ds in dataSources)
            {
                var dataSourceKey = ds.Key;

                //per DataSource
                container.RegisterType(daliaType,
                    dataSourceKey,
                    new HierarchicalLifetimeManager(),
                    ctor,
                    new InjectionFactory(c => unityFactoryMethod(c, c.Resolve<IDataSource>(dataSourceKey), dataSourceKey)));

                //new
                var newKey = dataSourceKey + UnityDaliaResolver.ResolveNewSuffix;
                container.RegisterType(daliaType,
                    newKey,
                    new PerResolveLifetimeManager(),
                    ctor,
                    new InjectionFactory(c => unityFactoryMethod(c, c.Resolve<IDataSource>(dataSourceKey), newKey)));
            }
        }

        public void Extend(IUnityDaliaResolverRegistrator registrator)
        {
            registrator.RegisterTypes(this);
        }
    }





}

