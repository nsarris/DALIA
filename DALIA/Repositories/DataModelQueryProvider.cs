using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalia.AsyncExtensions;
using Dynamix.QueryableExtensions;

namespace Dalia.Repositories
{
    public class DataModelQueryProvider<T> : IDataModelQueryProvider<T>
           where T : class
    {
        protected IDataContextAsync Context { get; private set; }

        public DataModelQueryProvider(IDataContextAsync context)
        {
            this.Context = context;
        }

        public bool SupportsAsync => Context.SupportsAsync;

        public bool SupportsQueryable => Context.SupportsQueryable;

        public T GetById(object id)
        {
            if (SupportsQueryable)
                return Context.QueryById<T>(id).SingleOrDefault();
            else
                return Context.SelectById<T>(id);
        }

        public Task<T> GetByIdAsync(object id)
        {
            if (SupportsQueryable)
                return Context.QueryById<T>(id).SingleOrDefaultAsync();
            else
                return Context.SelectByIdAsync<T>(id);
        }

        public IQueryable<T> ToQueryable()
        {
            if (!SupportsQueryable)
                throw new NotImplementedException();
            else
                return Context.Query<T>();
        }

        public SingleQueryable<T> QueryById(object id)
        {
            if (!SupportsQueryable)
                throw new NotSupportedException();
            else
                return Context
                    .QueryById<T>(id);
        }
    }

    public class DataModelQueryProvider<TDataModel, TKey> : DataModelQueryProvider<TDataModel>, IDataModelQueryProvider<TDataModel, TKey>
        where TDataModel : class
    {

        public DataModelQueryProvider(IDataContextAsync context)
            : base(context)
        {

        }

        public TDataModel GetById(TKey id)
        {
            return base.GetById(id);
        }

        public Task<TDataModel> GetByIdAsync(TKey id)
        {
            return base.GetByIdAsync(id);
        }

        public SingleQueryable<TDataModel> QueryById(TKey id)
        {
            return base.QueryById(id);
        }
    }
}
