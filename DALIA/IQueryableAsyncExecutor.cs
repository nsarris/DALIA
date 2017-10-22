﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dalia
{
    public interface IQueryableAsyncExecutor
    {
        Task<bool> AllAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken));
        Task<bool> AnyAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken));
        Task<bool> AnyAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task<decimal?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken token = default(CancellationToken));
        Task<double?> AverageAsync(IQueryable<int?> source, CancellationToken token = default(CancellationToken));
        Task<double> AverageAsync(IQueryable<long> source, CancellationToken token = default(CancellationToken));
        Task<double?> AverageAsync(IQueryable<long?> source, CancellationToken token = default(CancellationToken));
        Task<float> AverageAsync(IQueryable<float> source, CancellationToken token = default(CancellationToken));
        Task<float?> AverageAsync(IQueryable<float?> source, CancellationToken token = default(CancellationToken));
        Task<double> AverageAsync(IQueryable<double> source, CancellationToken token = default(CancellationToken));
        Task<double?> AverageAsync(IQueryable<double?> source, CancellationToken token = default(CancellationToken));
        Task<double> AverageAsync(IQueryable<int> source, CancellationToken token = default(CancellationToken));
        Task<decimal?> AverageAsync(IQueryable<decimal?> source, CancellationToken token = default(CancellationToken));
        Task<decimal> AverageAsync(IQueryable<decimal> source, CancellationToken token = default(CancellationToken));
        Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken token = default(CancellationToken));
        Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken token = default(CancellationToken));
        Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken token = default(CancellationToken));
        Task<float> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken token = default(CancellationToken));
        Task<float?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken token = default(CancellationToken));
        Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken token = default(CancellationToken));
        Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken token = default(CancellationToken));
        Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken token = default(CancellationToken));
        Task<decimal> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken token = default(CancellationToken));
        Task<bool> ContainsAsync<TSource>(IQueryable<TSource> source, TSource item, CancellationToken token = default(CancellationToken));
        Task<int> CountAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task<int> CountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken));
        Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken));
        Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken));
        Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task ForEachAsync<TSource>(IQueryable<TSource> source, Action<TSource> action, CancellationToken token = default(CancellationToken));
        Task ForEachUntilAsync<TSource>(IQueryable<TSource> source, Func<TSource, bool> func, CancellationToken token = default(CancellationToken));
        Task<long> LongCountAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task<long> LongCountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken));
        Task<TResult> MaxAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken token = default(CancellationToken));
        Task<TSource> MaxAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task<TSource> MinAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task<TResult> MinAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken token = default(CancellationToken));
        Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken));
        Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken));
        Task<long> SumAsync(IQueryable<long> source, CancellationToken token = default(CancellationToken));
        Task<int> SumAsync(IQueryable<int> source, CancellationToken token = default(CancellationToken));
        Task<decimal?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken token = default(CancellationToken));
        Task<decimal> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken token = default(CancellationToken));
        Task<long?> SumAsync(IQueryable<long?> source, CancellationToken token = default(CancellationToken));
        Task<double> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken token = default(CancellationToken));
        Task<float> SumAsync(IQueryable<float> source, CancellationToken token = default(CancellationToken));
        Task<int?> SumAsync(IQueryable<int?> source, CancellationToken token = default(CancellationToken));
        Task<double?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken token = default(CancellationToken));
        Task<double?> SumAsync(IQueryable<double?> source, CancellationToken token = default(CancellationToken));
        Task<decimal> SumAsync(IQueryable<decimal> source, CancellationToken token = default(CancellationToken));
        Task<decimal?> SumAsync(IQueryable<decimal?> source, CancellationToken token = default(CancellationToken));
        Task<double> SumAsync(IQueryable<double> source, CancellationToken token = default(CancellationToken));
        Task<int?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken token = default(CancellationToken));
        Task<long> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken token = default(CancellationToken));
        Task<long?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken token = default(CancellationToken));
        Task<float> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken token = default(CancellationToken));
        Task<float?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken token = default(CancellationToken));
        Task<int> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken token = default(CancellationToken));
        Task<float?> SumAsync(IQueryable<float?> source, CancellationToken token = default(CancellationToken));
        Task<TSource[]> ToArrayAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
        Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken token = default(CancellationToken));
        Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken token = default(CancellationToken));
        Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken token = default(CancellationToken));
        Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken token = default(CancellationToken));
        Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken));
    }
}