//using Dalia.Linq2db;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity;
//using Unity.Injection;
//using Unity.Lifetime;
//using Unity.Resolution;

//namespace Dalia.DI.Unity
//{
//    [DataContextFactory(typeof(ILinqToDBDataContext), typeof(UnityLinqToDBDataConnectionFactory))]
//    class UnityLinqToDBDataContextFactory : IUnityLinqToDBDataContextFactory
//    {
//        private IUnityContainer container;
//        private UnityLinqToDBDataConnectionFactory factory;

//        public UnityLinqToDBDataContextFactory(IUnityContainer container)
//        {
//            this.container = container;
//            factory = container.Resolve<UnityLinqToDBDataConnectionFactory>();
//        }

//        public T Current<T>()
//            where T : ILinqToDBDataContext
//        {
//            return (T)Current(typeof(T), container.Resolve<IDataSource>(GetDefaultIDataSourceKey(typeof(T))));
//        }

//        public T Current<T>(string key)
//            where T : ILinqToDBDataContext
//        {
//            return (T)Current(typeof(T), container.Resolve<IDataSource>(key));
//        }

//        public T Current<T>(IDataSource dataSource)
//            where T : ILinqToDBDataContext
//        {
//            return (T)Current(typeof(T), dataSource);
//        }

//        public ILinqToDBDataContext Current(Type type, IDataSource dataSource)
//        {
//            var dbtype = GetDataConnectionType(type);
//            var genericType = typeof(LinqToDBDataContext<>).MakeGenericType(dbtype);

//            var name = type.FullName + "_" + dataSource.Key;

//            if (!container.IsRegistered(typeof(ILinqToDBDataContext), name))
//            {
//                var con = factory.Current(dbtype, dataSource);
//                ILinqToDBDataContext dc = Create(type, con);

//                container.RegisterInstance(typeof(ILinqToDBDataContext), name, dc, new HierarchicalLifetimeManager());
//                container.RegisterInstance(genericType, name, dc, new HierarchicalLifetimeManager());

//                if (type != genericType)
//                    container.RegisterInstance(dc.GetType(), name, dc, new HierarchicalLifetimeManager());

//                return dc;
//            }
//            else
//                return (ILinqToDBDataContext)container.Resolve(typeof(ILinqToDBDataContext), name);
//        }

//        public ILinqToDBDataContext Current(Type type, string key)
//        {
//            return Current(type, container.Resolve<IDataSource>(key));
//        }

//        public ILinqToDBDataContext Current(Type type)
//        {
//            return Current(type, container.Resolve<IDataSource>(GetDefaultIDataSourceKey(type)));
//        }





//        public T Create<T>()
//            where T : ILinqToDBDataContext
//        {
//            return (T)Create(typeof(T), container.Resolve<IDataSource>(GetDefaultIDataSourceKey(typeof(T))));
//        }

//        public ILinqToDBDataContext Create(Type type)
//        {
//            return Create(type, container.Resolve<IDataSource>(GetDefaultIDataSourceKey(type)));
//        }

//        public T Create<T>(LinqToDBDataConnection connection)
//            where T : ILinqToDBDataContext
//        {
//            return (T)Create(typeof(T), connection);
//        }

//        public ILinqToDBDataContext Create(Type type, IDataSource dataSource)
//        {
//            var dbtype = GetDataConnectionType(type);
//            //var genericType = typeof(LinqToDBDataContext<>).MakeGenericType(dbtype);
            
//            var key = type.FullName + "_noDataConnection";

//            if (!container.IsRegistered(type, key))
//                container.RegisterType(type, key,
//                    new InjectionConstructor(
//                        new ResolvedParameter<IDataSource>()));

//            return (ILinqToDBDataContext)container.Resolve(type, key,
//                new DependencyOverride<IDataSource>(dataSource));
//        }

//        public T Create<T>(IDataSource dataSource)
//            where T : ILinqToDBDataContext
//        {
//            return (T)Create(typeof(T), dataSource);
//        }

//        public ILinqToDBDataContext Create(Type type, LinqToDBDataConnection connection)
//        {
//            var dbtype = GetDataConnectionType(type);

//            var key = type.FullName + "_withDataConnection";

//            if (!container.IsRegistered(type, key))
//                container.RegisterType(type, key,
//                    new InjectionConstructor(
//                        new ResolvedParameter(dbtype)));

//            return (ILinqToDBDataContext)container.Resolve(type, key,
//                new DependencyOverride(dbtype,connection));
//        }

//        private Type GetDataConnectionType(Type contextType)
//        {
//            if (!contextType.BaseType.IsGenericType || typeof(LinqToDBDataContext<>) != contextType.BaseType.GetGenericTypeDefinition())
//                throw new Exception("DataContext type must inherit LinqToDBDataContext<>");

//            var dbtype = contextType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ILinqToDBDataContext<>)).FirstOrDefault().GetGenericArguments()[0];

//            return dbtype;
//        }

//        private string GetDefaultIDataSourceKey(Type dataContextType)
//        {
//            var key = (dataContextType
//                .GetCustomAttributes(typeof(DefaultDataSourceAttribute), false)
//                .FirstOrDefault() as DefaultDataSourceAttribute)?.Key;

//            if (key == null)
//                throw new Exception("Type " + dataContextType.Name + " is not decorated with the DefaultIDataSource attribute");

//            return key;
//        }
        
        

//        IDataContextAsync IDataContextFactory.Create(Type type, IDataSource dataSource)
//        {
//            return Create(type,dataSource);
//        }

//        T IDataContextFactory.Create<T>()
//        {
//            return (T)Create(typeof(T));
//        }

//        T IDataContextFactory.Create<T>(IDataSource dataSource)
//        {
//            return (T)Create(typeof(T),dataSource);
//        }

//        IDataContextAsync IDataContextFactory.Create(Type type)
//        {
//            return Create(type);
//        }

//        IDataContextAsync IDataContextFactory.Current(Type type)
//        {
//            return Create(type);
//        }

//        IDataContextAsync IDataContextFactory.Current(Type type, IDataSource dataSource)
//        {
//            return Current(type,dataSource);
//        }

//        IDataContextAsync IDataContextFactory.Current(Type type, string key)
//        {
//            return Current(type, key);
//        }

//        T IDataContextFactory.Current<T>()
//        {
//            return (T)Current(typeof(T));
//        }

//        T IDataContextFactory.Current<T>(IDataSource dataSource)
//        {
//            return (T)Current(typeof(T),dataSource);
//        }

//        T IDataContextFactory.Current<T>(string key)
//        {
//            return (T)Current(typeof(T), key);
//        }
//    }
//}
