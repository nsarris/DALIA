using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dalia
{
    public interface IObjectMapper
    {
        TTarget MapTo<TSource, TTarget>(TSource source);
        Expression<Func<TSource, TTarget>> GetMapExpression<TSource, TTarget>();
        Func<TSource, TTarget> GetMapFunction<TSource, TTarget>();
    }
}
