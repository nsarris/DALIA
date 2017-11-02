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
//    class UnityLinqToDBDataConnectionFactory : ILinqToDBDataConnectionFactory
//    {
//        private IUnityContainer container;
//        private UnityDbConnectionFactory factory;
//        public UnityLinqToDBDataConnectionFactory(IUnityContainer container)
//        {
//            this.container = container;
//            factory = container.Resolve<UnityDbConnectionFactory>();
//        }

//        public T Current<T>(string key)
//            where T : LinqToDBDataConnection
//        {
//            return (T)Current(typeof(T), container.Resolve<IDataSource>(key));
//        }

//        public T Current<T>(IDataSource dataSource)
//            where T : LinqToDBDataConnection
//        {
//            return (T)Current(typeof(T), dataSource);
//        }

//        public LinqToDBDataConnection Current(Type type, IDataSource dataSource)
//        {
//            var name = type.FullName + "_" + dataSource.Key;

//            if (!container.IsRegistered(typeof(LinqToDBDataConnection), name))
//            {
//                var con = factory.Current(dataSource);
//                var dc = Create(type, dataSource, con);

//                container.RegisterInstance(typeof(LinqToDBDataConnection), name, dc, new HierarchicalLifetimeManager());
//                //container.RegisterInstance(typeof(IDaliaLinqToDBDataConnection), name, dc, new Unity.Lifetime.HierarchicalLifetimeManager());
//                container.RegisterInstance(type, name, dc, new HierarchicalLifetimeManager());

//                return dc;
//            }
//            else
//                return (LinqToDBDataConnection)container.Resolve(typeof(LinqToDBDataConnection), name);
//        }

//        public LinqToDBDataConnection Current(Type type, string key)
//        {
//            return Current(type, container.Resolve<IDataSource>(key));
//        }







//        public T Create<T>(IDataSource dataSource, DbConnection connection)
//            where T : LinqToDBDataConnection
//        {
//            return (T)Create(typeof(T), dataSource, connection);
//        }

//        public LinqToDBDataConnection Create(Type type, IDataSource dataSource)
//        {
//            if (type == typeof(LinqToDBDataConnection) || type.IsAbstract)
//                throw new Exception("Cannot resolve abstract class DbConnection, please specify a concrete connection type");

//            var key = type.FullName + "_nodbcon";

//            if (!container.IsRegistered(type, key))
//                container.RegisterType(type, key,
//                    new InjectionConstructor(
//                        new ResolvedParameter<IDataSource>()));

//            return (LinqToDBDataConnection)container.Resolve(type, key,
//                new DependencyOverride<IDataSource>(dataSource));
//        }

//        public T Create<T>(IDataSource dataSource)
//            where T : LinqToDBDataConnection
//        {
//            return (T)Create(typeof(T), dataSource);
//        }

//        public LinqToDBDataConnection Create(Type type, IDataSource dataSource, DbConnection connection)
//        {
//            if (type == typeof(LinqToDBDataConnection) || type.IsAbstract)
//                throw new Exception("Cannot resolve abstract class DbConnection, please specify a concrete connection type");

//            var key = type.FullName + "_withdbcon";

//            if (!container.IsRegistered(type, key))
//                container.RegisterType(type, key,
//                    new InjectionConstructor(
//                        new ResolvedParameter<IDataSource>(),
//                        new ResolvedParameter<DbConnection>()));

//            return (LinqToDBDataConnection)container.Resolve(type, key,
//                new DependencyOverride<IDataSource>(dataSource),
//                new DependencyOverride<DbConnection>(connection));
//        }

//        public LinqToDBDataConnection Create(Type type)
//        {
//            return Create(type, container.Resolve<IDataSource>(GetDefaultIDataSourceKey(type)));
//        }

//        public T Create<T>() where T : LinqToDBDataConnection
//        {
//            return (T)Create(typeof(T), container.Resolve<IDataSource>(GetDefaultIDataSourceKey(typeof(T))));
//        }

//        public LinqToDBDataConnection Current(Type type)
//        {
//            return Current(type, container.Resolve<IDataSource>(GetDefaultIDataSourceKey(type)));
//        }

//        public T Current<T>() where T : LinqToDBDataConnection
//        {
//            return (T)Current(typeof(T), container.Resolve<IDataSource>(GetDefaultIDataSourceKey(typeof(T))));
//        }

//        private string GetDefaultIDataSourceKey(Type dataConnectionType)
//        {
//            var key = (dataConnectionType
//                .GetCustomAttributes(typeof(DefaultDataSourceAttribute), false)
//                .FirstOrDefault() as DefaultDataSourceAttribute)?.Key;

//            if (key == null)
//                throw new Exception("Type " + dataConnectionType.Name + " is not decorated with the DefaultIDataSource attribute");

//            return key;
//        }
//    }
//}
