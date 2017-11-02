using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalia.AsyncExtensions;
using Dynamix.QueryableExtensions;
using Dalia.Mapping;

namespace Dalia.Repositories
{
    public class DtoQueryProvider<TDataModel, TDTO> : IDtoQueryProvider<TDataModel, TDTO>
            where TDTO : class
            where TDataModel : class
    {
        public IDataContextAsync Context { get; private set; }
        protected IObjectMapper Mapper { get; private set; }

        public bool OwnsContext { get; set; }

        public DtoQueryProvider(IDataContextAsync context, IObjectMapper mapper)
        {
            this.Context = context;
            this.Mapper = mapper;
        }

        public virtual bool SupportsQueryable => Context.SupportsQueryable;
        public virtual bool SupportsAsync => Context.SupportsAsync;

        

        public virtual TDTO GetById(object id)
        {
            if (SupportsQueryable)
                return Context
                        .QueryById<TDataModel>(id)
                        .Select(Mapper.GetMapExpression<TDataModel, TDTO>())
                        .SingleOrDefault();
            else
                return Mapper.GetMapFunction<TDataModel, TDTO>().Invoke(
                    Context.SelectById<TDataModel>(id));
        }

        public virtual async Task<TDTO> GetByIdAsync(object id)
        {
            if (SupportsQueryable)
                return await Context
                        .QueryById<TDataModel>(id)
                        .Select(Mapper.GetMapExpression<TDataModel, TDTO>())
                        .SingleOrDefaultAsync();
            else
                return Mapper.GetMapFunction<TDataModel, TDTO>().Invoke(
                    await Context.SelectByIdAsync<TDataModel>(id));
        }

        public virtual IQueryable<TDTO> ToQueryable()
        {
            if (!SupportsQueryable)
                throw new NotSupportedException();
            else
                return Context
                    .Query<TDataModel>()
                    .Select(Mapper.GetMapExpression<TDataModel, TDTO>());
        }

        public virtual SingleQueryable<TDTO> QueryById(object id)
        {
            if (!SupportsQueryable)
                throw new NotSupportedException();
            else
                return Context
                    .QueryById<TDataModel>(id)
                    .Select(Mapper.GetMapExpression<TDataModel, TDTO>());
        }

        public void Dispose()
        {
            if (OwnsContext)
                Context.Dispose();
        }
    }

    public class DtoQueryProvider<TDataModel, TDTO, TDTOKey> : DtoQueryProvider<TDataModel, TDTO>, IQueryProvider<TDTO, TDTOKey>
        where TDTO : class
        where TDataModel : class
    {

        public DtoQueryProvider(IDataContextAsync context, IObjectMapper mapper)
            : base(context, mapper)
        {

        }

        public virtual TDTO GetById(TDTOKey id)
        {
            return base.GetById(id);
        }

        public virtual Task<TDTO> GetByIdAsync(TDTOKey id)
        {
            return base.GetByIdAsync(id);
        }

        public virtual SingleQueryable<TDTO> QueryById(TDTOKey id)
        {
            return base.QueryById(id);
        }
    }
}
