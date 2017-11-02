using DomainObjects.Core;
using DomainObjects.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dalia.AsyncExtensions;
using Dynamix.QueryableExtensions;
using Dalia.Repositories;
using Dalia.Mapping;

namespace Dalia.DomainObjects
{
    public class EntityRepository<TDataModel, TEntity> : EntityQueryProvider<TDataModel, TEntity>, IMappedRepository<TDataModel, TEntity>
        where TEntity : DomainEntity
        where TDataModel : class
    {
        public EntityRepository(IDataContextAsync context, IObjectMapper mapper) : base(context, mapper)
        {
        }

        public void Delete(TDataModel obj)
        {
            Context.Delete(obj);
        }

        public void Delete(TEntity obj)
        {
            Context.Delete(obj);
        }

        public Task DeleteAsync(TDataModel obj)
        {
            return Context.DeleteAsync(obj);
        }

        public Task DeleteAsync(TEntity obj)
        {
            return Context.DeleteAsync(obj);
        }

        public void DeleteById(object id)
        {
            Context.Delete(id);
        }

        public Task DeleteByIdAsync(object id)
        {
            return Context.DeleteAsync(id);
        }

        public void Insert(TDataModel obj)
        {
            Context.Insert(obj);
        }

        public void Insert(TEntity obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(TDataModel obj)
        {
            return Context.InsertAsync(obj);
        }

        public Task InsertAsync(TEntity obj)
        {
            return Context.InsertAsync(obj);
        }

        public void Update(TDataModel obj)
        {
            Context.Update(obj);
        }

        public void Update(TEntity obj)
        {
            Context.Update(obj);
        }

        public Task UpdateAsync(TDataModel obj)
        {
            return Context.UpdateAsync(obj);
        }

        public Task UpdateAsync(TEntity obj)
        {
            return Context.UpdateAsync(obj);
        }
    }

    public class EntityRepository<TDataModel, TEntity, TEntityKey> : EntityRepository<TDataModel, TEntity>, IMappedRepository<TDataModel, TEntity, TEntityKey>
        where TEntity : DomainEntity<TEntityKey>
        where TDataModel : class
    {
        public EntityRepository(IDataContextAsync context, IObjectMapper mapper) : base(context, mapper)
        {
        }

        public void DeleteById(TEntityKey id)
        {
            base.DeleteById(id);
        }

        public Task DeleteByIdAsync(TEntityKey id)
        {
            return base.DeleteByIdAsync(id);
        }

        public virtual TEntity GetById(TEntityKey id)
        {
            return base.GetById(id);
        }

        public virtual Task<TEntity> GetByIdAsync(TEntityKey id)
        {
            return base.GetByIdAsync(id);
        }

       
        public virtual SingleQueryable<TEntity> QueryById(TEntityKey id)
        {
            return base.QueryById(id);
        }

      

      
    }
}