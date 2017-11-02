//using System;
//using System.Collections.Generic;
//using System.Data;
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
//    internal class UnityDbConnectionFactory : IDbConnectionFactory, IDisposable
//    {
//        private IUnityContainer container;
//        private IUnityContainer singletonContainer;

//        public UnityDbConnectionFactory(IUnityContainer container)
//        {
//            this.container = container;
//            singletonContainer = container.CreateChildContainer();
//        }

//        private void RegisterType(Type dbConnectionType)
//        {
//            if (!container.IsRegistered(dbConnectionType))
//                container.RegisterType(dbConnectionType,
//                    new InjectionConstructor(
//                        new ResolvedParameter<string>()));
//        }


//        public T Current<T>()
//            where T : DbConnection
//        {
//            var registrations = container.Registrations.Where(x => x.RegisteredType == typeof(T) && !string.IsNullOrEmpty(x.Name));
//            if (!registrations.Any())
//                throw new Exception("No instances of " + typeof(T) + " found in container, please provide a key");
//            else if (registrations.Count() > 1)
//                throw new Exception("Multiple " + typeof(T) + " found in container, please provide a key");

//            return Current<T>(registrations.Single().Name);
//        }

//        public T Current<T>(string key)
//            where T : DbConnection
//        {
//            return (T)Current(container.Resolve<IDataSource>(key));
//        }

//        public DbConnection Current(string key)
//        {
//            return Current(container.Resolve<IDataSource>(key));
//        }

//        public DbConnection Current(IDataSource dataSource)
//        {
//            return Current(dataSource.Key, dataSource.DbConnectionType, dataSource.ConnectionString);
//        }

//        private DbConnection Current(string key, Type type, string connectionString)
//        {
//            if (type == typeof(DbConnection))
//                throw new Exception("Cannot resolve abstract class DbConnection, please specify a concrete connection type");

//            if (!string.IsNullOrEmpty(connectionString)
//                && !container.IsRegistered(typeof(DbConnection), key))
//            {
//                var con = Create(type, connectionString);
//                container.RegisterInstance(typeof(DbConnection), key, con, new HierarchicalLifetimeManager());
//                container.RegisterInstance(typeof(IDbConnection), key, con, new HierarchicalLifetimeManager());
//                container.RegisterInstance(con.GetType(), key, con, new HierarchicalLifetimeManager());
//                if (type != con.GetType() || type != typeof(DbConnection) || type != typeof(IDbConnection))
//                    container.RegisterInstance(type, key, con, new HierarchicalLifetimeManager());

//                //container.RegisterType(from: typeof(DbConnection), to: type, key, new Unity.Lifetime.HierarchicalLifetimeManager());
//                //container.RegisterType(type, key, new Unity.Lifetime.HierarchicalLifetimeManager());
//                //container.RegisterType(typeof(IDbConnection), key, new Unity.Lifetime.HierarchicalLifetimeManager());

//                return con;


//            }
//            else
//                return (DbConnection)container.Resolve(typeof(DbConnection), key);
//        }






//        public T Create<T>(string connectionString)
//            where T : DbConnection
//        {
//            return (T)Create(typeof(T), connectionString);
//        }

//        public DbConnection Create(IDataSource dataSource)
//        {
//            return Create(dataSource.DbConnectionType, dataSource.ConnectionString);
//        }

//        public DbConnection Create(Type type, string connectionString)
//        {
//            if (type == typeof(DbConnection) || type.IsAbstract)
//                throw new Exception("Cannot resolve abstract class DbConnection, please specify a concrete connection type");

//            RegisterType(type);

//            return (DbConnection)container.Resolve(type, new DependencyOverride<string>(connectionString));


//        }

//        public void Dispose()
//        {
//            singletonContainer.Dispose();
//        }
//    }
//}
