using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamix;
using System.Linq.Expressions;
using System.Threading;
using Dalia.Schema;
using Dynamix.QueryableExtensions;

namespace Dalia
{
    public interface ISchemaModelProvider
    {
        //string GetTableName(Type entityType);
        //string GetColumnName<T, TMember>(Expression<Func<T, TMember>> expression);
        //string GetColumnName<T, TMember>(Expression<Func<T, TMember>> expression, out string TableName);
        //string GetColumnName(MemberExpression expression);
        //string GetColumnName(MemberExpression expression, out string TableName);

        //List<PropertyInfo> GetKeyProperties(Type T);
        //Expression<Func<T, bool>> GetKeyPredicate<T>(T entity);
        
        SchemaModel SchemaModel { get; }
    }

    public interface IQueryContext : ISchemaModelProvider,IDisposable
    {
        //TODO: Add Execute Scalar Multi
        object ExecuteScalar(string query, QueryParameters parameters = null, object nullvalue = null);
        T ExecuteScalar<T>(string query, QueryParameters parameters = null, T nullvalue = default(T));
        List<T> ExecuteScalarList<T>(string query, QueryParameters parameters = null);
        List<DynamicType> ExecuteDynamic(string query, QueryParameters parameters = null);
        List<List<Dynamix.DynamicType>> ExecuteDynamicMulti(string query, QueryParameters parameters = null);
        List<IDictionary<string, object>> ExecuteDictionary(string query, QueryParameters parameters = null);

        List<object> SelectFromQuery(Type Type, string query, QueryParameters parameters = null);
        List<T> SelectFromQuery<T>(string query, QueryParameters parameters = null) where T : class;
        T SelectById<T>(object id) where T : class;
        T SelectById<T>(QueryParameters id) where T : class;

        //List<T> SelectAll<T>(string OrderBy = null, int Limit = -1) where T : class;
        //List<T> Select<T>(object parameters, string OrderBy = null, int Limit = -1) where T : class;
        //T SelectOne<T>(object parameters) where T : class;

        //IEnumerable<TRelation> ExpandCollection<T, TRelation>(T obj, Expression<Func<T, ICollection<TRelation>>> expression = null)
        //    where T : class
        //    where TRelation : class;

        //TLookUp ExpandLookup<T, TLookUp>(T obj, Expression<Func<T, TLookUp>> expression = null)
        //    where T : class
        //    where TLookUp : class;

        //IEnumerable<TRelation> ExpandCollection<T, TRelation>(IEnumerable<T> listobj, Expression<Func<T, ICollection<TRelation>>> expression = null)
        //    where T : class
        //    where TRelation : class;

        //IEnumerable<TLookUp> ExpandLookup<T, TLookUp>(IEnumerable<T> listobj, Expression<Func<T, TLookUp>> expression = null)
        //    where T : class
        //    where TLookUp : class;
    }

    public interface IQueryContextAsync : IQueryContext
    {

        Task<object> ExecuteScalarAsync(string query, QueryParameters parameters = null, object nullvalue = null);
        Task<T> ExecuteScalarAsync<T>(string query, QueryParameters parameters = null, T nullvalue = default(T));
        Task<List<T>> ExecuteScalarListAsync<T>(string query, QueryParameters parameters = null);
        Task<List<DynamicType>> ExecuteDynamicAsync(string query, QueryParameters parameters = null);
        Task<List<List<Dynamix.DynamicType>>> ExecuteDynamicMultiAsync(string query, QueryParameters parameters = null);
        Task<List<IDictionary<string, object>>> ExecuteDictionaryAsync(string query, QueryParameters parameters = null);

        Task<List<object>> SelectFromQueryAsync(Type Type, string query, QueryParameters parameters = null);
        Task<List<T>> SelectFromQueryAsync<T>(string query, QueryParameters parameters = null) where T : class;
        Task<T> SelectByIdAsync<T>(object id) where T : class;
        Task<T> SelectByIdAsync<T>(QueryParameters id) where T : class;
    }

    public interface IQueryableContext : ISchemaModelProvider, IDisposable
    {
        //Queryable
        //bool SupportsQueryable { get; }
        //DynamicQueryable Query(Type entityType);
        IQueryable<T> Query<T>() where T : class;
        SingleQueryable<T> QueryById<T>(object id) where T : class;
        //IQueryable<TEntity> Query<TEntity>(QueryFluent<TEntity> query) where TEntity : class;
        //IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> filter = null,
        //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //    List<Expression<Func<TEntity, object>>> includes = null,
        //    int? page = null,
        //    int? pageSize = null)
        //    where TEntity : class;
        //IQueryable<T> Query<T>(string FuncName, params QueryParameter[] Parameters) where T : class;
        ////IQueryable<TEntity> Query<TEntity>(QueryFluent<TEntity> query) where TEntity : class;
        ////IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> filter = null,
        ////    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        ////    List<Expression<Func<TEntity, object>>> includes = null,
        ////    int? page = null,
        ////    int? pageSize = null)
        ////    where TEntity : class;

    }

    public interface ICommandContext : ISchemaModelProvider, IDisposable
    {
        int ExecuteNonQuery(string command, QueryParameters parameters = null);

        void BulkInsert<T>(IEnumerable<T> obj) where T : class;
        void Insert<T>(T obj) where T : class;
        void Update<T>(T objectKey, Delta<T> delta) where T : class;
        void Update<T>(T obj) where T : class;
        void Delete<T>(T obj) where T : class;
        void Delete<T>(Expression<Func<T, bool>> predicate) where T : class;
        void Delete<T>(IEnumerable<T> objKeys) where T : class;
        void Delete<T>(object parameters) where T : class;
        void Load<T>(T obj, object parameters = null) where T : class;
        void Save<T>(T obj) where T : class;

        //T SaveFromDTO<T, TDTO>(TDTO obj, Action<TDTO, T> updateMapper)
        //    where T : class
        //    where TDTO : class;
    }

    public interface ICommandContextAsync : ICommandContext
    {
        Task<int> ExecuteNonQueryAsync(string command, QueryParameters parameters = null);

        Task BulkInsertAsync<T>(IEnumerable<T> obj) where T : class;
        Task InsertAsync<T>(T obj) where T : class;
        Task UpdateAsync<T>(T objectKey, Delta<T> delta) where T : class;
        Task UpdateAsync<T>(T obj) where T : class;
        Task DeleteAsync<T>(T obj) where T : class;
        Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task DeleteAsync<T>(IEnumerable<T> objKeys) where T : class;
        Task DeleteAsync<T>(object parameters) where T : class;
        Task LoadAsync<T>(T obj, object parameters = null) where T : class;
        Task SaveAsync<T>(T obj) where T : class;

        //T SaveFromDTOAsync<T, TDTO>(TDTO obj, Action<TDTO, T> updateMapper)
        //    where T : class
        //    where TDTO : class;
    }

    public interface ITransactionContext
    {
        bool TransactionState { get; }
        ITransaction BeginTransaction();
        void CommitTransaction();
        void RollBackTransaction();
    }

  
    public interface IDataContext : IQueryContext, IQueryableContext, ICommandContext, ITransactionContext, IDisposable
    {
        bool SupportsAsync { get; }
        bool SupportsTransactions { get; }
        bool SupportsQueryable { get; }
    }

    public interface IDataContextAsync : IDataContext, IQueryContextAsync, ICommandContextAsync
    {

    }
}
