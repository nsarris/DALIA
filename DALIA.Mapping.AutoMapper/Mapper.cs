using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Dalia.Mapping.AutoMapper
{
    public class Mapper : Dalia.Mapping.IObjectMapper
    {
        global::AutoMapper.IMapper mapper;
        public Mapper(global::AutoMapper.IMapper mapper)
        {
            this.mapper = mapper;
        }
        public Expression<Func<TSource, TTarget>> GetMapExpression<TSource, TTarget>()
        {
            return mapper.ConfigurationProvider.ExpressionBuilder.GetMapExpression<TSource, TTarget>();

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
