using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Repositories
{
    public class Repository<T> : DataModelQueryProvider<T>, IRepository<T> where T : class
    {
        public Repository(IDataContextAsync context) : base(context)
        {
        }

        public virtual void Delete(T obj)
        {
            Context.Delete(obj);
        }

        public virtual Task DeleteAsync(T obj)
        {
            return Context.DeleteAsync(obj);
        }

        public virtual void DeleteById(object id)
        {
            Context.Delete(QueryParameters.InferFrom(id));
        }

        public virtual Task DeleteByIdAsync(object id)
        {
            return Context.DeleteAsync(QueryParameters.InferFrom(id));
        }

        public virtual void Insert(T obj)
        {
            Context.Insert(obj);
        }

        public virtual Task InsertAsync(T obj)
        {
            return Context.InsertAsync(obj);
        }

        public virtual void Update(T obj)
        {
            Context.Update(obj);
        }

        public virtual Task UpdateAsync(T obj)
        {
            return Context.UpdateAsync(obj);
        }
    }

    public class Repository<T, TKey> :Repository<T> , IRepository<T,TKey>  where T : class
    {
        public Repository(IDataContextAsync context) : base(context)
        {
        }

        public void DeleteById(TKey id)
        {
            base.DeleteById(id);
        }

        public Task DeleteByIdAsync(TKey id)
        {
            return base.DeleteByIdAsync(id);
        }

        public T GetById(TKey id)
        {
            return base.GetById(id);
        }

        public Task<T> GetByIdAsync(TKey id)
        {
            return base.GetByIdAsync(id);
        }
    }
}
