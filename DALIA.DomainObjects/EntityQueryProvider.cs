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


    

    public class EntityQueryProvider<TDataModel, TEntity> : IDtoQueryProvider<TDataModel, TEntity>, IQueryProvider<TEntity>
        where TEntity : DomainEntity
        where TDataModel : class
    {
        public IDataContextAsync Context { get; private set; }
        protected IObjectMapper Mapper { get; private set; }
        public bool OwnsContext { get; set; }

        public EntityQueryProvider(IDataContextAsync context, IObjectMapper mapper)
        {
            this.Context = context;
            this.Mapper = mapper;
        }

        public virtual bool SupportsQueryable => Context.SupportsQueryable;
        public virtual bool SupportsAsync => Context.SupportsAsync;

        public virtual TEntity GetById(object id)
        {
            if (SupportsQueryable)
                return Context
                        .QueryById<TDataModel>(id)
                        .Select(Mapper.GetMapExpression<TDataModel, TEntity>())
                        .SingleOrDefault();
            else
                return Mapper.GetMapFunction<TDataModel, TEntity>().Invoke(
                    Context.SelectById<TDataModel>(id));
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            if (SupportsQueryable)
                return await Context
                        .QueryById<TDataModel>(id)
                        .Select(Mapper.GetMapExpression<TDataModel, TEntity>())
                        .SingleOrDefaultAsync();
            else
                return Mapper.GetMapFunction<TDataModel, TEntity>().Invoke(
                    await Context.SelectByIdAsync<TDataModel>(id));
        }

        public virtual IQueryable<TEntity> ToQueryable()
        {
            if (!SupportsQueryable)
                throw new NotSupportedException();
            else
                return Context
                    .Query<TDataModel>()
                    .Select(Mapper.GetMapExpression<TDataModel, TEntity>());
        }

        public virtual SingleQueryable<TEntity> QueryById(object id)
        {
            if (!SupportsQueryable)
                throw new NotSupportedException();
            else
                return Context
                    .QueryById<TDataModel>(id)
                    .Select(Mapper.GetMapExpression<TDataModel, TEntity>());
        }

        public void Dispose()
        {
            if (OwnsContext)
                Context.Dispose();
        }
    }

    public class EntityQueryProvider<TDataModel, TEntity, TEntityKey> : EntityQueryProvider<TDataModel, TEntity>, IEntityQueryProvider<TEntity, TEntityKey>
        where TEntity : DomainEntity<TEntityKey>
        where TDataModel : class
    {

        public EntityQueryProvider(IDataContextAsync context, IObjectMapper mapper)
            : base(context, mapper)
        {

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