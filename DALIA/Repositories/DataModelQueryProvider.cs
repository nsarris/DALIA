using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalia.AsyncExtensions;
using Dynamix.QueryableExtensions;

namespace Dalia.Repositories
{
    public class DataModelQueryProvider<T> : IQueryProvider<T>
           where T : class
    {
        public IDataContextAsync Context { get; private set; }
        public bool OwnsContext { get; set; }
        public DataModelQueryProvider(IDataContextAsync context)
        {
            this.Context = context;
        }

        public virtual bool SupportsAsync => Context.SupportsAsync;

        public virtual bool SupportsQueryable => Context.SupportsQueryable;

        public virtual T GetById(object id)
        {
            if (SupportsQueryable)
                return Context.QueryById<T>(id).SingleOrDefault();
            else
                return Context.SelectById<T>(id);
        }

        public virtual Task<T> GetByIdAsync(object id)
        {
            if (SupportsQueryable)
                return Context.QueryById<T>(id).SingleOrDefaultAsync();
            else
                return Context.SelectByIdAsync<T>(id);
        }

        public virtual IQueryable<T> ToQueryable()
        {
            if (!SupportsQueryable)
                throw new NotImplementedException();
            else
                return Context.Query<T>();
        }

        public virtual SingleQueryable<T> QueryById(object id)
        {
            if (!SupportsQueryable)
                throw new NotSupportedException();
            else
                return Context
                    .QueryById<T>(id);
        }

        public void Dispose()
        {
            if (OwnsContext)
                Context.Dispose();
        }
    }

    public class DataModelQueryProvider<TDataModel, TKey> : DataModelQueryProvider<TDataModel>, IQueryProvider<TDataModel, TKey>
        where TDataModel : class
    {

        public DataModelQueryProvider(IDataContextAsync context)
            : base(context)
        {

        }

        public virtual TDataModel GetById(TKey id)
        {
            return base.GetById(id);
        }

        public virtual Task<TDataModel> GetByIdAsync(TKey id)
        {
            return base.GetByIdAsync(id);
        }

        public virtual SingleQueryable<TDataModel> QueryById(TKey id)
        {
            return base.QueryById(id);
        }
    }
}
