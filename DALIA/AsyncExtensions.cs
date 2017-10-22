using Dynamix.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dalia.AsyncExtensions
{
    public static class QueryableAsyncExtensions
    {
        #region ProviderType
        public enum ProviderType
        {
            Unsupported,
            Linq,
            LinqToDB,
            EF
        }

        private static Dictionary<string, ProviderType> Providers = new Dictionary<string, ProviderType>();

        static QueryableAsyncExtensions()
        {
            Providers.Add("LinqToDB.Linq", ProviderType.LinqToDB);
            Providers.Add("System.Linq", ProviderType.Linq);
        }

        private static ProviderType GetProviderType(this System.Linq.IQueryProvider provider)
        {
            if (Providers.TryGetValue(provider.GetType().Namespace, out var providerType))
                return providerType;
            else
                return ProviderType.Unsupported;
        }

        private static IQueryableAsyncExecutor GetProviderExecutor(System.Linq.IQueryProvider provider)
        {
            if (Providers.TryGetValue(provider.GetType().Namespace, out var providerType))
                if (providerType == ProviderType.LinqToDB)
                    return new LinqToDBQueryableAsyncExecutor();
                else
                    return null;
            else return null;
        }

        public static bool GetSupportsAsync<TSource>(this IQueryable<TSource> queryable)
        {
            return GetProviderType(queryable.Provider) != ProviderType.Unsupported;
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Executes provided action using task scheduler.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <param name="token">Asynchronous operation cancellation token.</param>
        /// <returns>Asynchronous operation completion task.</returns>
        internal static Task GetActionTask(Action action, CancellationToken token)
        {
            var task = new Task(action, token);

            task.Start();

            return task;
        }

        /// <summary>
        /// Executes provided function using task scheduler.
        /// </summary>
        /// <typeparam name="T">Function result type.</typeparam>
        /// <param name="func">Function to execute.</param>
        /// <returns>Asynchronous operation completion task.</returns>
        static Task<T> GetTask<T>(Func<T> func)
        {
            var task = new Task<T>(func);

            task.Start();

            return task;
        }

        /// <summary>
        /// Executes provided function using task scheduler.
        /// </summary>
        /// <typeparam name="T">Function result type.</typeparam>
        /// <param name="func">Function to execute.</param>
        /// <param name="token">Asynchronous operation cancellation token.</param>
        /// <returns>Asynchronous operation completion task.</returns>
        static Task<T> GetTask<T>(Func<T> func, CancellationToken token)
        {
            var task = new Task<T>(func, token);

            task.Start();

            return task;
        }

        #endregion

        #region ForEachAsync

        /// <summary>
        /// Asynchronously apply provided action to each element in source sequence.
        /// Sequence elements processed sequentially.
        /// </summary>
        /// <typeparam name="TSource">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="action">Action to apply to each sequence element.</param>
        /// <param name="token">Optional asynchronous operation cancellation token.</param>
        /// <returns>Asynchronous operation completion task.</returns>
        public static Task ForEachAsync<TSource>(
            this IQueryable<TSource> source, Action<TSource> action,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.ForEachAsync(source, action, token);

#endif

            return GetActionTask(() =>
            {
                foreach (var item in source)
                {
                    if (token.IsCancellationRequested)
                        break;
                    action(item);
                }
            },
            token);
        }

        /// <summary>
        /// Asynchronously apply provided function to each element in source sequence sequentially.
        /// Sequence enumeration stops if function returns <c>false</c>.
        /// </summary>
        /// <typeparam name="TSource">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="func">Function to apply to each sequence element. Returning <c>false</c> from function will stop numeration.</param>
        /// <param name="token">Optional asynchronous operation cancellation token.</param>
        /// <returns>Asynchronous operation completion task.</returns>
        public static Task ForEachUntilAsync<TSource>(
            this IQueryable<TSource> source, Func<TSource, bool> func,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.ForEachUntilAsync(source, func, token);

#endif

            return GetActionTask(() =>
            {
                foreach (var item in source)
                    if (token.IsCancellationRequested || !func(item))
                        break;
            },
            token);
        }

        #endregion

#if !NOASYNC

        #region ToListAsync

        /// <summary>
        /// Asynchronously loads data from query to a list.
        /// </summary>
        /// <typeparam name="TSource">Query element type.</typeparam>
        /// <param name="source">Source query.</param>
        /// <param name="token">Optional asynchronous operation cancellation token.</param>
        /// <returns>List with query results.</returns>
        public static Task<List<TSource>> ToListAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.ToListAsync(source, token);

            return GetTask(() => source.AsEnumerable().TakeWhile(_ => !token.IsCancellationRequested).ToList(), token);
        }

        #endregion

        #region ToArrayAsync

        /// <summary>
        /// Asynchronously loads data from query to an array.
        /// </summary>
        /// <typeparam name="TSource">Query element type.</typeparam>
        /// <param name="source">Source query.</param>
        /// <param name="token">Optional asynchronous operation cancellation token.</param>
        /// <returns>Array with query results.</returns>
        public static Task<TSource[]> ToArrayAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.ToArrayAsync(source, token);

            return GetTask(() => source.AsEnumerable().TakeWhile(_ => !token.IsCancellationRequested).ToArray(), token);
        }

        #endregion

        #region ToDictionaryAsync

        /// <summary>
        /// Asynchronously loads data from query to a dictionary.
        /// </summary>
        /// <typeparam name="TSource">Query element type.</typeparam>
        /// <typeparam name="TKey">Dictionary key type.</typeparam>
        /// <param name="source">Source query.</param>
        /// <param name="keySelector">Source element key selector.</param>
        /// <param name="token">Optional asynchronous operation cancellation token.</param>
        /// <returns>Dictionary with query results.</returns>
        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            CancellationToken token = default(CancellationToken))
        {
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.ToDictionaryAsync(source, keySelector, token);

            return GetTask(() => source.AsEnumerable().TakeWhile(_ => !token.IsCancellationRequested).ToDictionary(keySelector), token);
        }

        /// <summary>
        /// Asynchronously loads data from query to a dictionary.
        /// </summary>
        /// <typeparam name="TSource">Query element type.</typeparam>
        /// <typeparam name="TKey">Dictionary key type.</typeparam>
        /// <param name="source">Source query.</param>
        /// <param name="keySelector">Source element key selector.</param>
        /// <param name="comparer">Dictionary key comparer.</param>
        /// <param name="token">Optional asynchronous operation cancellation token.</param>
        /// <returns>Dictionary with query results.</returns>
        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken token = default(CancellationToken))
        {
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.ToDictionaryAsync(source, keySelector, comparer, token);

            return GetTask(() => source.AsEnumerable().TakeWhile(_ => !token.IsCancellationRequested).ToDictionary(keySelector, comparer), token);
        }

        /// <summary>
        /// Asynchronously loads data from query to a dictionary.
        /// </summary>
        /// <typeparam name="TSource">Query element type.</typeparam>
        /// <typeparam name="TKey">Dictionary key type.</typeparam>
        /// <typeparam name="TElement">Dictionary element type.</typeparam>
        /// <param name="source">Source query.</param>
        /// <param name="keySelector">Source element key selector.</param>
        /// <param name="elementSelector">Dictionary element selector.</param>
        /// <param name="token">Optional asynchronous operation cancellation token.</param>
        /// <returns>Dictionary with query results.</returns>
        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            CancellationToken token = default(CancellationToken))
        {
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.ToDictionaryAsync(source, keySelector, elementSelector, token);

            return GetTask(() => source.AsEnumerable().TakeWhile(_ => !token.IsCancellationRequested).ToDictionary(keySelector, elementSelector), token);
        }

        /// <summary>
        /// Asynchronously loads data from query to a dictionary.
        /// </summary>
        /// <typeparam name="TSource">Query element type.</typeparam>
        /// <typeparam name="TKey">Dictionary key type.</typeparam>
        /// <typeparam name="TElement">Dictionary element type.</typeparam>
        /// <param name="source">Source query.</param>
        /// <param name="keySelector">Source element key selector.</param>
        /// <param name="elementSelector">Dictionary element selector.</param>
        /// <param name="comparer">Dictionary key comparer.</param>
        /// <param name="token">Optional asynchronous operation cancellation token.</param>
        /// <returns>Dictionary with query results.</returns>
        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken token = default(CancellationToken))
        {
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.ToDictionaryAsync(source, keySelector, elementSelector, comparer, token);

            return GetTask(() => source.AsEnumerable().TakeWhile(_ => !token.IsCancellationRequested).ToDictionary(keySelector, elementSelector, comparer), token);
        }

        #endregion

#endif


        #region FirstAsync<TSource>

        public static Task<TSource> FirstAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.FirstAsync(source, token);
#endif

            return GetTask(source.First, token);
        }

        #endregion

        #region FirstAsync<TSource, predicate>

        public static Task<TSource> FirstAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.FirstAsync(source, predicate, token);

#endif

            return GetTask(() => source.First(predicate), token);
        }

        #endregion

        #region FirstOrDefaultAsync<TSource>

        public static Task<TSource> FirstOrDefaultAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.FirstOrDefaultAsync(source, token);
#endif

            return GetTask(source.FirstOrDefault, token);
        }

        #endregion

        #region FirstOrDefaultAsync<TSource, predicate>

        public static Task<TSource> FirstOrDefaultAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.FirstOrDefaultAsync(source, predicate, token);

#endif

            return GetTask(() => source.FirstOrDefault(predicate), token);
        }

        #endregion

        #region SingleAsync<TSource>

        public static Task<TSource> SingleAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SingleAsync(source, token);


#endif

            return GetTask(source.Single, token);
        }

        #endregion

        #region SingleAsync<TSource, predicate>

        public static Task<TSource> SingleAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SingleAsync(source, predicate, token);


#endif

            return GetTask(() => source.Single(predicate), token);
        }

        #endregion

        #region SingleOrDefaultAsync<TSource>

        public static Task<TSource> SingleOrDefaultAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SingleOrDefaultAsync(source, token);


#endif

            return GetTask(source.SingleOrDefault, token);
        }

        #endregion

        #region SingleOrDefaultAsync<TSource, predicate>

        public static Task<TSource> SingleOrDefaultAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SingleOrDefaultAsync(source, predicate, token);


#endif

            return GetTask(() => source.SingleOrDefault(predicate), token);
        }

        #endregion

        #region ContainsAsync<TSource, item>

        public static Task<bool> ContainsAsync<TSource>(
            this IQueryable<TSource> source, TSource item,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.ContainsAsync(source, item, token);

#endif

            return GetTask(() => source.Contains(item), token);
        }

        #endregion

        #region AnyAsync<TSource>

        public static Task<bool> AnyAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AnyAsync(source, token);


#endif

            return GetTask(source.Any, token);
        }

        #endregion

        #region AnyAsync<TSource, predicate>

        public static Task<bool> AnyAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AnyAsync(source, predicate, token);


#endif

            return GetTask(() => source.Any(predicate), token);
        }

        #endregion

        #region AllAsync<TSource, predicate>

        public static Task<bool> AllAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AllAsync(source, predicate, token);


#endif

            return GetTask(() => source.All(predicate), token);
        }

        #endregion

        #region CountAsync<TSource>

        public static Task<int> CountAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.CountAsync(source, token);



#endif

            return GetTask(source.Count, token);
        }

        #endregion

        #region CountAsync<TSource, predicate>

        public static Task<int> CountAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.CountAsync(source, predicate, token);


#endif

            return GetTask(() => source.Count(predicate), token);
        }

        #endregion

        #region LongCountAsync<TSource>

        public static Task<long> LongCountAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.LongCountAsync(source, token);


#endif

            return GetTask(source.LongCount, token);
        }

        #endregion

        #region LongCountAsync<TSource, predicate>

        public static Task<long> LongCountAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.LongCountAsync(source, predicate, token);

#endif

            return GetTask(() => source.LongCount(predicate), token);
        }

        #endregion

        #region MinAsync<TSource>

        public static Task<TSource> MinAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.MinAsync(source, token);


#endif

            return GetTask(source.Min, token);
        }

        #endregion

        #region MinAsync<TSource, selector>

        public static Task<TResult> MinAsync<TSource, TResult>(
            this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.MinAsync(source, selector, token);


#endif

            return GetTask(() => source.Min(selector), token);
        }

        #endregion

        #region MaxAsync<TSource>

        public static Task<TSource> MaxAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.MaxAsync(source, token);


#endif

            return GetTask(source.Max, token);
        }

        #endregion

        #region MaxAsync<TSource, selector>

        public static Task<TResult> MaxAsync<TSource, TResult>(
            this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.MaxAsync(source, selector, token);


#endif

            return GetTask(() => source.Max(selector), token);
        }

        #endregion

        #region SumAsync<int>

        public static Task<int> SumAsync(
            this IQueryable<int> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);


#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<int?>

        public static Task<int?> SumAsync(
            this IQueryable<int?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);


#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<long>

        public static Task<long> SumAsync(
            this IQueryable<long> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);


#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<long?>

        public static Task<long?> SumAsync(
            this IQueryable<long?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);

#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<float>

        public static Task<float> SumAsync(
            this IQueryable<float> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);


#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<float?>

        public static Task<float?> SumAsync(
            this IQueryable<float?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);

#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<double>

        public static Task<double> SumAsync(
            this IQueryable<double> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);


#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<double?>

        public static Task<double?> SumAsync(
            this IQueryable<double?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);


#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<decimal>

        public static Task<decimal> SumAsync(
            this IQueryable<decimal> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);


#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<decimal?>

        public static Task<decimal?> SumAsync(
            this IQueryable<decimal?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, token);


#endif

            return GetTask(source.Sum, token);
        }

        #endregion

        #region SumAsync<int, selector>

        public static Task<int> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, int>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);


#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region SumAsync<int?, selector>

        public static Task<int?> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);


#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region SumAsync<long, selector>

        public static Task<long> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, long>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);


#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region SumAsync<long?, selector>

        public static Task<long?> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);


#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region SumAsync<float, selector>

        public static Task<float> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, float>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);


#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region SumAsync<float?, selector>

        public static Task<float?> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);


#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region SumAsync<double, selector>

        public static Task<double> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, double>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC


            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);

#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region SumAsync<double?, selector>

        public static Task<double?> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);


#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region SumAsync<decimal, selector>

        public static Task<decimal> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC


            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);

#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region SumAsync<decimal?, selector>

        public static Task<decimal?> SumAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.SumAsync(source, selector, token);

#endif

            return GetTask(() => source.Sum(selector), token);
        }

        #endregion

        #region AverageAsync<int>

        public static Task<double> AverageAsync(
            this IQueryable<int> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);


#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<int?>

        public static Task<double?> AverageAsync(
            this IQueryable<int?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);


#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<long>

        public static Task<double> AverageAsync(
            this IQueryable<long> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC


            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);

#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<long?>

        public static Task<double?> AverageAsync(
            this IQueryable<long?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);


#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<float>

        public static Task<float> AverageAsync(
            this IQueryable<float> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);


#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<float?>

        public static Task<float?> AverageAsync(
            this IQueryable<float?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);


#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<double>

        public static Task<double> AverageAsync(
            this IQueryable<double> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);

#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<double?>

        public static Task<double?> AverageAsync(
            this IQueryable<double?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);


#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<decimal>

        public static Task<decimal> AverageAsync(
            this IQueryable<decimal> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);


#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<decimal?>

        public static Task<decimal?> AverageAsync(
            this IQueryable<decimal?> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, token);


#endif

            return GetTask(source.Average, token);
        }

        #endregion

        #region AverageAsync<int, selector>

        public static Task<double> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, int>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);


#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region AverageAsync<int?, selector>

        public static Task<double?> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);


#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region AverageAsync<long, selector>

        public static Task<double> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, long>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);


#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region AverageAsync<long?, selector>

        public static Task<double?> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);


#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region AverageAsync<float, selector>

        public static Task<float> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, float>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);


#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region AverageAsync<float?, selector>

        public static Task<float?> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);



#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region AverageAsync<double, selector>

        public static Task<double> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, double>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);



#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region AverageAsync<double?, selector>

        public static Task<double?> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC

            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);

#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region AverageAsync<decimal, selector>

        public static Task<decimal> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC


            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);

#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region AverageAsync<decimal?, selector>

        public static Task<decimal?> AverageAsync<TSource>(
            this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC


            var executor = GetProviderExecutor(source.Provider);
            if (executor != null)
                return executor.AverageAsync(source, selector, token);

#endif

            return GetTask(() => source.Average(selector), token);
        }

        #endregion

        #region SingleAsync<TSource>

        public static Task<TSource> SingleAsync<TSource>(
            this SingleQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            
            var executor = GetProviderExecutor(source.Queryable.Provider);
            if (executor != null)
                return executor.SingleAsync(source.Queryable, token);


#endif

            return GetTask(source.Single, token);
        }

        #endregion

        #region SingleAsync<TSource, predicate>

        public static Task<TResult> SingleAsync<TSource,TResult>(
            this SingleQueryable<TSource> source, Expression<Func<TSource, TResult>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Queryable.Provider);
            if (executor != null)
                return executor.SingleAsync(source.Queryable.Select(selector), token);


#endif

            return GetTask(() => source.Select(selector).Single(), token);
        }

        #endregion

        #region SingleOrDefaultAsync<TSource>

        public static Task<TSource> SingleOrDefaultAsync<TSource>(
            this SingleQueryable<TSource> source,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Queryable.Provider);
            if (executor != null)
                return executor.SingleOrDefaultAsync(source.Queryable, token);


#endif

            return GetTask(source.SingleOrDefault, token);
        }

        #endregion

        #region SingleOrDefaultAsync<TSource, predicate>

        public static Task<TResult> SingleOrDefaultAsync<TSource, TResult>(
            this SingleQueryable<TSource> source, Expression<Func<TSource, TResult>> selector,
            CancellationToken token = default(CancellationToken))
        {
#if !NOASYNC
            var executor = GetProviderExecutor(source.Queryable.Provider);
            if (executor != null)
                return executor.SingleOrDefaultAsync(source.Queryable.Select(selector), token);


#endif

            return GetTask(() => source.Select(selector).SingleOrDefault(), token);
        }

        #endregion
    }
}
