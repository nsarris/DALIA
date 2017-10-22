using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

namespace DALIA.DebugTest
{
    public class Mapper : Dalia.DomainObjects.IObjectMapper
    {
        AutoMapper.IMapper mapper;
        public Mapper(AutoMapper.IMapper mapper)
        {
            this.mapper = mapper;
        }
        public Expression<Func<TSource, TTarget>> GetMapExpression<TSource, TTarget>()
        {
            var exp = Enumerable.Empty<TSource>().AsQueryable().ProjectTo<TTarget>(mapper.ConfigurationProvider).Expression;

            var lambda = ((exp as MethodCallExpression).Arguments[1] as UnaryExpression).Operand as Expression<Func<TSource, TTarget>>;

            return lambda;
        }

        public Func<TSource, TTarget> GetMapFunction<TSource, TTarget>()
        {
            return GetMapExpression<TSource, TTarget>().Compile();
        }

        public TTarget MapTo<TSource, TTarget>(TSource source)
        {
            return mapper.Map<TSource, TTarget>(source);
        }
    }
}
