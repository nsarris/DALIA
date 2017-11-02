//using Dalia.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity;
//using Unity.Injection;
//using Unity.Lifetime;
//using Unity.Resolution;

//namespace Dalia.DI.Unity
//{
//    internal class UnityDataContextFactory : IDataContextFactory
//    {
//        static Dictionary<Type, Type> providerMap = new Dictionary<Type, Type>();

//        public static void RegisterFactories(IUnityContainer container)
//        {
//            container.RegisterType<UnityDbConnectionFactory>(
//                new HierarchicalLifetimeManager(),
//                    new InjectionFactory((c) => new UnityDbConnectionFactory(c)));

//            container.RegisterType<UnityDataContextFactory>(
//                new HierarchicalLifetimeManager(),
//                    new InjectionFactory((c) => new UnityDataContextFactory(c)));

//            container.RegisterType(from: typeof(IDataContextFactory), to: typeof(UnityDataContextFactory));

//            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
//            var loadedPaths = loadedAssemblies.Where(x => !x.IsDynamic).Select(a => a.Location).ToArray();

//            var referencedPaths = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
//            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

//            foreach (var path in toLoad)
//            {
//                var assemblyName = System.Reflection.AssemblyName.GetAssemblyName(path);
//                if (assemblyName.Name.StartsWith("Dalia"))
//                    loadedAssemblies.Add(AppDomain.CurrentDomain.Load(assemblyName));
//            }


//            foreach (var factoryType in AppDomain.CurrentDomain.GetAssemblies()
//                .SelectMany(x => x.GetTypes().Where(t => t.GetInterfaces().Any(i => i == typeof(IDataContextFactory)))))
//            {
//                var attr = factoryType.GetCustomAttributes(typeof(DataContextFactoryAttribute), true).FirstOrDefault() as DataContextFactoryAttribute;
//                if (attr != null)
//                {
//                    container.RegisterType(factoryType,
//                        new HierarchicalLifetimeManager(),
//                            //new InjectionFactory((c) => c.Resolve(factoryType, new DependencyOverride<IUnityContainer>(c))));
//                            new InjectionFactory((c) => Activator.CreateInstance(factoryType, c)));

//                    container.RegisterType(attr.UnderlyingProviderFactoryType,
//                        new HierarchicalLifetimeManager(),
//                            //new InjectionFactory((c) => c.Resolve(factoryType, new DependencyOverride<IUnityContainer>(c))));
//                            new InjectionFactory((c) => Activator.CreateInstance(attr.UnderlyingProviderFactoryType, c)));

//                    providerMap.Add(attr.ContextInterface, factoryType);
//                }
//            }

//            RegisterContexts(container);
//            RegisterQueryProviders(container);
//            RegisterRepositories(container);
//        }

//        private static void RegisterContexts(IUnityContainer container)
//        {
//            var contextTypes = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic)
//                 .SelectMany(x => x.GetTypes().Where(t => !t.IsAbstract && !string.IsNullOrEmpty(t.Namespace) && !t.Namespace.StartsWith("Dalia") && t.GetInterfaces()
//                    .Any(i => i == typeof(IDataContextAsync)))).ToList();

//            foreach (var contextType in contextTypes)
//            {
//                if (contextType.GetCustomAttributes(typeof(DefaultDataSourceAttribute), true).FirstOrDefault() is DefaultDataSourceAttribute attr)
//                {
//                    container.RegisterType(contextType
//                        , new InjectionConstructor(container.Resolve<IDataSource>(attr.Key)));
//                }
//            }
//        }

//        private static void RegisterQueryProviders(IUnityContainer container)
//        {
//            var repositoryTypes = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic)
//                 .SelectMany(x => x.GetTypes().Where(t => !t.IsAbstract && !string.IsNullOrEmpty(t.Namespace) && !t.Namespace.StartsWith("Dalia") && t.GetInterfaces()
//                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryProvider<>)))).ToList();

//            var groups = repositoryTypes.SelectMany(
//                x => x.GetInterfaces()
//                        .Where(i => IsGenericTypeDefinition(i, typeof(IQueryProvider<>)))
//                        .Select(i => new
//                        {
//                            RepositoryType = x,
//                            SourceType = x.GetInterfaces().Where(ii => IsGenericTypeDefinition(ii, typeof(IDtoQueryProvider<,>))).FirstOrDefault()?.GetGenericArguments()?.FirstOrDefault(),
//                            QueryType = i.GetGenericArguments()[0],
//                            Interface = i
//                        }))
//                        .GroupBy(i => i.QueryType)
//                        .ToList();

//            var ownsContextPropertyName = Dynamix.Reflection.ReflectionHelper.GetMemberName((IQueryProvider<IUnityContainer> t) => t.OwnsContext);
//            foreach (var repoTypeGroup in groups)
//            {
//                if (repoTypeGroup.Count() == 1)
//                    container.RegisterType(from: repoTypeGroup.First().Interface, to: repoTypeGroup.First().RepositoryType, injectionMembers: new InjectionProperty(ownsContextPropertyName, true));
//                else
//                {
//                    var defaultRepos = repoTypeGroup.Where(x => x.RepositoryType.GetCustomAttributes(typeof(DefaultRepositoryAttribute), false).Any()).ToList();
//                    if (defaultRepos.Count == 1)
//                        container.RegisterType(from: defaultRepos.First().Interface, to: defaultRepos.First().RepositoryType, injectionMembers: new InjectionProperty(ownsContextPropertyName, true));

//                    else if (defaultRepos.Count > 1)
//                        throw new Exception("More than one default repositories of type " + repoTypeGroup.Key.Name + " found");

//                    else if (repoTypeGroup.Count() > 1)
//                        throw new Exception("More than one repositories of type " + repoTypeGroup.Key.Name + " found, use default the DefaultRepository attribute");
//                }
//            }
//        }

//        private static void RegisterRepositories(IUnityContainer container)
//        {
//            var repositoryTypes = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic)
//                 .SelectMany(x => x.GetTypes().Where(t => !t.IsAbstract && !string.IsNullOrEmpty(t.Namespace) && !t.Namespace.StartsWith("Dalia") && t.GetInterfaces()
//                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>)))).ToList();

//            var groups = repositoryTypes.SelectMany(
//                x => x.GetInterfaces()
//                        .Where(i => IsGenericTypeDefinition(i, typeof(IRepository<>)))
//                        .Select(i => new
//                        {
//                            RepositoryType = x,
//                            SourceType = x.GetInterfaces().Where(ii => IsGenericTypeDefinition(ii, typeof(IMappedRepository<,>))).FirstOrDefault()?.GetGenericArguments()?.FirstOrDefault(),
//                            QueryType = i.GetGenericArguments()[0],
//                            Interface = i
//                        }))
//                        .GroupBy(i => i.QueryType)
//                        .ToList();

//            var ownsContextPropertyName = Dynamix.Reflection.ReflectionHelper.GetMemberName((IQueryProvider<IUnityContainer> t) => t.OwnsContext);
//            foreach (var repoTypeGroup in groups)
//            {
//                if (repoTypeGroup.Count() == 1)
//                    container.RegisterType(from: repoTypeGroup.First().Interface, to: repoTypeGroup.First().RepositoryType, injectionMembers: new InjectionProperty(ownsContextPropertyName, true));
//                else
//                {
//                    var defaultRepos = repoTypeGroup.Where(x => x.RepositoryType.GetCustomAttributes(typeof(DefaultRepositoryAttribute), false).Any()).ToList();
//                    if (defaultRepos.Count == 1)
//                        container.RegisterType(from: defaultRepos.First().Interface, to: defaultRepos.First().RepositoryType, injectionMembers: new InjectionProperty(ownsContextPropertyName, true));

//                    else if (defaultRepos.Count > 1)
//                        throw new Exception("More than one default repositories of type " + repoTypeGroup.Key.Name + " found");

//                    else if (repoTypeGroup.Count() > 1)
//                        throw new Exception("More than one repositories of type " + repoTypeGroup.Key.Name + " found, use default the DefaultRepository attribute");
//                }
//            }
//        }



//        private static bool IsGenericTypeDefinition(Type t, Type generticTypeDefinition)
//        {
//            return t.IsGenericType && t.GetGenericTypeDefinition() == generticTypeDefinition;
//        }


//        #region Factory Wrapper

//        private IUnityContainer container;

//        private IDataContextFactory GetFactory(Type resolvedType)
//        {
//            foreach (var map in providerMap)
//            {
//                if (resolvedType.GetInterfaces().Any(x => x == map.Key))
//                    return (IDataContextFactory)container.Resolve(map.Value);
//            }

//            throw new Exception("No factory provider registered for resolution of type " + resolvedType.Name);
//        }

//        public UnityDataContextFactory(IUnityContainer container)
//        {
//            this.container = container;
//        }
//        public IDataContextAsync Create(Type type, IDataSource dataSource)
//        {
//            return GetFactory(type).Create(type, dataSource);
//        }

//        public T Create<T>() where T : IDataContextAsync
//        {
//            return GetFactory(typeof(T)).Create<T>();
//        }

//        public T Create<T>(IDataSource dataSource) where T : IDataContextAsync
//        {
//            return GetFactory(typeof(T)).Create<T>(dataSource);
//        }

//        public IDataContextAsync Create(Type type)
//        {
//            return GetFactory(type).Create(type);
//        }

//        public IDataContextAsync Current(Type type)
//        {
//            return GetFactory(type).Current(type);
//        }

//        public IDataContextAsync Current(Type type, IDataSource dataSource)
//        {
//            return GetFactory(type).Current(type, dataSource);
//        }

//        public IDataContextAsync Current(Type type, string key)
//        {
//            return GetFactory(type).Current(type, key);
//        }

//        public T Current<T>() where T : IDataContextAsync
//        {
//            return GetFactory(typeof(T)).Current<T>();
//        }

//        public T Current<T>(IDataSource dataSource) where T : IDataContextAsync
//        {
//            return GetFactory(typeof(T)).Current<T>(dataSource);
//        }

//        public T Current<T>(string key) where T : IDataContextAsync
//        {
//            return GetFactory(typeof(T)).Current<T>(key);
//        }

//        #endregion
//    }
//}
