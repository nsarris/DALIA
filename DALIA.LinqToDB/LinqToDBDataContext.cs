using Dalia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Dynamix;
using System.Threading;
using System.Data.Common;
using LinqToDB;
using Dalia.AdoExtensions;
using Dalia.Schema;
using System.Collections.Concurrent;
using Dynamix.QueryableExtensions;

namespace Dalia.Linq2db
{

    public class LinqToDBDataContext<TDataConnection> : IDataContextAsync
        where TDataConnection : LinqToDB.Data.DataConnection
    {
        LinqToDBTransactionWrapper transaction;
        TDataConnection connection;
        public static ConcurrentDictionary<Type, SchemaModel> schemaModels = new ConcurrentDictionary<Type, SchemaModel>();

        static object schemaModelLock = new object();
        public SchemaModel SchemaModel { get; private set; }
        public DbConnection DbConnection => connection.Connection as DbConnection;
        public TDataConnection Linq2DBDataConnection
        {
            get => connection;
            set
            {
                connection = value;
                SchemaModel schemaModel = null;
                if (connection != null && !schemaModels.TryGetValue(connection.GetType(), out schemaModel))
                {
                    lock (schemaModelLock)
                    {
                        if (!schemaModels.TryGetValue(connection.GetType(), out schemaModel))
                        {
                            schemaModel = new SchemaModel();

                            //Add all ITable<> Properties of LinqToDB DataConnection
                            foreach (var tableType in connection.GetType().GetProperties()
                                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ITable<>))
                                .Select(x => x.PropertyType.GenericTypeArguments[0]))
                            {
                                connection.MappingSchema.GetEntityDescriptor(tableType);
                            }

                            //Execute SchemaInitializer
                            foreach (var initializerType in this.GetType()
                                .GetCustomAttributes(typeof(SchemaDescriptorInitializerAttribute), false)
                                .Select(x => ((SchemaDescriptorInitializerAttribute)x).InitializerType)
                                .Where(x => x != null))
                            {
                                var initializer = Activator.CreateInstance(initializerType) as Schema.ILinqToDBSchemaInitializer;
                                if (initializer!=null)
                                    initializer.Init(connection);
                            }

                            foreach(var modelType in AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(
                                    a => a.GetTypes()
                                            .Where(t => t.GetCustomAttributes(typeof(IsModelOfContextAttribute), false)
                                                .Any(at => ((IsModelOfContextAttribute)at).ContextType == this.GetType()))))
                            {
                                connection.MappingSchema.GetEntityDescriptor(modelType);
                            }   
                                        

                            //Build SchemaModel
                            foreach (var t in connection.MappingSchema.GetEntites().Select(t => connection.MappingSchema.GetEntityDescriptor(t)))
                                if (!schemaModel.ContainsType(t.ObjectType))
                                    schemaModel.Register(new Schema.DataModelDescriptor(t));
                            
                            schemaModels[connection.GetType()] = schemaModel;
                        }
                    }
                }
                
                SchemaModel = schemaModel;
            }
        }

        public LinqToDBDataContext(IDataSource dataSource)
            : this(dataSource.ProviderTypeString, dataSource.ConnectionString)
        {

        }

        public LinqToDBDataContext(string provider, string connectionString)
        {
            if (provider == "DB2.iSeries" || provider == "DB2iSeries")
            {
                var a = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(x => x.GetName().Name == "LinqToDB.DataProvider.DB2iSeries").FirstOrDefault();

                if (a == null)
                    a = AppDomain.CurrentDomain.Load("LinqToDB.DataProvider.DB2iSeries");

                Linq2DBDataConnection = Activator.CreateInstance(typeof(TDataConnection),
                    Activator.CreateInstance(a.GetType("LinqToDB.DataProvider.DB2iSeries.DB2iSeriesDataProvider")) as LinqToDB.DataProvider.IDataProvider,
                    connectionString
                ) as TDataConnection;
            }
            else
                Linq2DBDataConnection = Activator.CreateInstance(typeof(TDataConnection),
                    provider,
                    connectionString
                    ) as TDataConnection;
        }

        public bool SupportsAsync => true;

        public bool SupportsTransactions => true;

        public bool SupportsQueryable => true;



        public bool TransactionState => transaction != null && transaction.Active;

        

        public ITransaction BeginTransaction()
        {
            transaction = new LinqToDBTransactionWrapper(connection.BeginTransaction());
            //transaction = new DbTransactionWrapper(connection.Connection.BeginTransaction());
            return transaction;
        }

        public void CommitTransaction()
        {
            transaction.Commit();
        }

        public void RollBackTransaction()
        {
            transaction.Rollback();
        }


        public void BulkInsert<T>(IEnumerable<T> obj) where T : class
        {
            throw new NotImplementedException();
        }

        public Task BulkInsertAsync<T>(IEnumerable<T> obj) where T : class
        {
            throw new NotImplementedException();
        }



        public void Delete<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(IEnumerable<T> objKeys) where T : class
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(object parameters) where T : class
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(IEnumerable<T> objKeys) where T : class
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(object parameters) where T : class
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public List<DynamicType> ExecuteDynamic(string query, QueryParameters parameters = null)
        {
            return DbConnection.GetDynamicTypeList(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public Task<List<DynamicType>> ExecuteDynamicAsync(string query, QueryParameters parameters = null)
        {
            return DbConnection.GetDynamicTypeListAsync(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public int ExecuteNonQuery(string command, QueryParameters parameters = null)
        {
            return DbConnection.ExecuteNonQuery(command, parameters, this.transaction?.UnderlyingTransaction);
        }

        public Task<int> ExecuteNonQueryAsync(string command, QueryParameters parameters = null)
        {
            return DbConnection.ExecuteNonQueryAsync(command, parameters, this.transaction?.UnderlyingTransaction);
        }

        public object ExecuteScalar(string query, QueryParameters parameters = null, object nullvalue = null)
        {
            return DbConnection.ExecuteScalar(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public T ExecuteScalar<T>(string query, QueryParameters parameters = null, T nullvalue = default(T))
        {
            if (!typeof(T).IsDbConvertible())
                throw new ArgumentException("Type " + typeof(T).Name + " is not scalar");
            return DbConnection.ExecuteScalar<T>(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public Task<object> ExecuteScalarAsync(string query, QueryParameters parameters = null, object nullvalue = null)
        {
            return DbConnection.ExecuteScalarAsync(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public Task<T> ExecuteScalarAsync<T>(string query, QueryParameters parameters = null, T nullvalue = default(T))
        {
            if (!typeof(T).IsDbConvertible())
                throw new ArgumentException("Type " + typeof(T).Name + " is not scalar");
            return DbConnection.ExecuteScalarAsync<T>(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public List<T> ExecuteScalarList<T>(string query, QueryParameters parameters = null)
        {
            if (!typeof(T).IsDbConvertible())
                throw new ArgumentException("Type " + typeof(T).Name + " is not scalar");
            return DbConnection.GetList<T>(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public Task<List<T>> ExecuteScalarListAsync<T>(string query, QueryParameters parameters = null)
        {
            if (!typeof(T).IsDbConvertible())
                throw new ArgumentException("Type " + typeof(T).Name + " is not scalar");
            return DbConnection.GetListAsync<T>(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public void Insert<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public void Load<T>(T obj, object parameters = null) where T : class
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync<T>(T obj, object parameters = null) where T : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return connection.GetTable<T>();
        }



        public void Save<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public List<object> SelectFromQuery(Type type, string query, QueryParameters parameters = null)
        {
            return DbConnection.GetList(type, query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public List<T> SelectFromQuery<T>(string query, QueryParameters parameters = null) where T : class
        {
            return DbConnection.GetList<T>(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public Task<List<object>> SelectFromQueryAsync(Type type, string query, QueryParameters parameters = null)
        {
            return DbConnection.GetListAsync(type, query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public Task<List<T>> SelectFromQueryAsync<T>(string query, QueryParameters parameters) where T : class
        {
            return DbConnection.GetListAsync<T>(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public void Update<T>(T objectKey, Delta<T> delta) where T : class
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<T>(T objectKey, Delta<T> delta) where T : class
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }





        public Task<List<List<DynamicType>>> ExecuteDynamicMultiAsync(string query, QueryParameters parameters = null)
        {
            return DbConnection.GetDynamicTypeListMultiAsync(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public Task<List<IDictionary<string, object>>> ExecuteDictionaryAsync(string query, QueryParameters parameters = null)
        {
            return DbConnection.GetDictionaryAsync(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public List<List<DynamicType>> ExecuteDynamicMulti(string query, QueryParameters parameters = null)
        {
            return DbConnection.GetDynamicTypeListMulti(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public List<IDictionary<string, object>> ExecuteDictionary(string query, QueryParameters parameters = null)
        {
            return DbConnection.GetDictionary(query, parameters, this.transaction?.UnderlyingTransaction);
        }

        public Task<T> SelectByIdAsync<T>(QueryParameters id) where T : class
        {
            var predicate = SchemaModel.GetKeyPredicateFromValues<T>(id);
            return Query<T>().FirstOrDefaultAsync(predicate);
        }

        public T SelectById<T>(QueryParameters id) where T : class
        {
            var predicate = SchemaModel.GetKeyPredicateFromValues<T>(id);
            return Query<T>().FirstOrDefault(predicate);
        }

        public T SelectById<T>(object id) where T : class
        {
            var predicate = SchemaModel.GetKeyPredicateFromValues<T>(id);
            return Query<T>().FirstOrDefault(predicate);
        }

        public Task<T> SelectByIdAsync<T>(object id) where T : class
        {
            var predicate = SchemaModel.GetKeyPredicateFromValues<T>(id);
            return Query<T>().FirstOrDefaultAsync(predicate);
        }

        public SingleQueryable<T> QueryById<T>(object id) where T : class
        {
            var predicate = SchemaModel.GetKeyPredicateFromValues<T>(id);
            return Query<T>().Where(predicate).ToSingleQueryable();
        }
    }

    public class LinqToDBDataContext : LinqToDBDataContext<LinqToDB.Data.DataConnection>
    {
        public LinqToDBDataContext(IDataSource dataSource) : base(dataSource)
        {
        }

        public LinqToDBDataContext(string provider, string connectionString) : base(provider, connectionString)
        {
        }
    }


}
