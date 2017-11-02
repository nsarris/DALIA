using Dalia.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Repositories
{
    public class MappedRepository<TDataModel,TDTO> : DtoQueryProvider<TDataModel, TDTO>, IMappedRepository<TDataModel,TDTO> 
        where TDataModel : class
        where TDTO : class
    {
        public MappedRepository(IDataContextAsync context, IObjectMapper mapper) : base(context, mapper)
        {
        }

        public virtual void Delete(TDTO obj)
        {
            Context.Delete(obj);
        }

        public virtual void Delete(TDataModel obj)
        {
            Context.Delete(obj);
        }

        public virtual Task DeleteAsync(TDTO obj)
        {
            return Context.DeleteAsync(obj);
        }

        public virtual Task DeleteAsync(TDataModel obj)
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

        public virtual void Insert(TDTO obj)
        {
            Context.Insert(obj);
        }

        public virtual void Insert(TDataModel obj)
        {
            Context.Insert(obj);
        }

        public virtual Task InsertAsync(TDTO obj)
        {
            return Context.InsertAsync(obj);
        }

        public virtual Task InsertAsync(TDataModel obj)
        {
            return Context.InsertAsync(obj);
        }

        public virtual void Update(TDTO obj)
        {
            Context.Update(obj);
        }

        public virtual void Update(TDataModel obj)
        {
            Context.Update(obj);
        }

        public virtual Task UpdateAsync(TDTO obj)
        {
            return Context.UpdateAsync(obj);
        }

        public virtual Task UpdateAsync(TDataModel obj)
        {
            return Context.UpdateAsync(obj);
        }
    }

    public class MappedRepository<TDataModel, TDTO, TKey> : MappedRepository<TDataModel, TDTO>, IMappedRepository<TDataModel, TDTO, TKey>
        where TDataModel : class
        where TDTO : class
    {
        public MappedRepository(IDataContextAsync context, IObjectMapper mapper) : base(context, mapper)
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

        public TDTO GetById(TKey id)
        {
            return base.GetById(id);
        }

        public Task<TDTO> GetByIdAsync(TKey id)
        {
            return base.GetByIdAsync(id);
        }
    }
}
