using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Dalia
{
    internal static class LinqToDBQueryableAsyncMethodHelper
    {
        static Lazy<IEnumerable<MethodInfo>> AsyncExtensionsTypeMethods = new Lazy<IEnumerable<MethodInfo>>(() =>
        {
            return AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.GetName().Name == "linq2db")
            .Select(x => x.GetType("LinqToDB.AsyncExtensions")).FirstOrDefault().GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        });

        static IEnumerable<Type> GetNonGenericSignature(IEnumerable<Type> parameterTypes, IEnumerable<Type> genericTypes)
        {
            foreach (var p in parameterTypes)
            {
                if (!p.IsGenericType)
                {
                    if (!genericTypes.Contains(p) && !p.IsGenericParameter)
                        yield return p;
                }
                else
                {
                    yield return p.GetGenericTypeDefinition();
                    foreach (var ip in p.GetGenericArguments())
                    {
                        if (ip.IsGenericType)
                        {
                            yield return ip.GetGenericTypeDefinition();
                            foreach (var iip in GetNonGenericSignature(ip.GenericTypeArguments, genericTypes))
                                if (!genericTypes.Contains(iip) && !iip.IsGenericParameter)
                                    yield return iip;
                        }
                        else
                            if (!genericTypes.Contains(ip) && !ip.IsGenericParameter)
                            yield return ip;
                    }
                }
            }
        }

        static IEnumerable<Type> GetNonGenericSignature(MethodInfo method, IEnumerable<Type> genericTypes)
        {
            return GetNonGenericSignature(method.GetParameters().Select(x => x.ParameterType), genericTypes);
        }

        public static MethodInfo GetMethod(string methodName, params Type[] parameterTypes)
        {
            return GetMethodInternal(methodName, new Type[] { }, parameterTypes);
        }

        public static MethodInfo GetMethod<TGeneric1>(string methodName, params Type[] parameterTypes)
        {
            return GetMethodInternal(methodName, new[] { typeof(TGeneric1) }, parameterTypes);
        }

        public static MethodInfo GetMethod<TGeneric1, TGeneric2>(string methodName, params Type[] parameterTypes)
        {
            return GetMethodInternal(methodName, new[] { typeof(TGeneric1), typeof(TGeneric2) }, parameterTypes);
        }

        public static MethodInfo GetMethod<TGeneric1, TGeneric2, TGeneric3>(string methodName, params Type[] parameterTypes)
        {
            return GetMethodInternal(methodName, new[] { typeof(TGeneric1), typeof(TGeneric2), typeof(TGeneric3) }, parameterTypes);
        }

        private static MethodInfo GetMethodInternal(string methodName, Type[] genericTypes, Type[] parameterTypes)
        {
            var signature = GetNonGenericSignature(parameterTypes, genericTypes).ToList();

            var mm = AsyncExtensionsTypeMethods.Value
                .Where(x => x.Name == methodName)
                .Select(x => new
                {
                    Method = x,
                    Signature = GetNonGenericSignature(x, genericTypes)
                })
                .ToList();



            var m = AsyncExtensionsTypeMethods.Value
                .Where(x => x.Name == methodName
                    && signature.SequenceEqual(
                            GetNonGenericSignature(x, genericTypes)
                            ))
                .FirstOrDefault()
                ;

            return m;
        }
    }



    class LinqToDBQueryableAsyncFunctions<TSource>
    {
        private static ConcurrentDictionary<Type, object> functionsHolders = new ConcurrentDictionary<Type, object>();

        public static LinqToDBQueryableAsyncFunctions<TSource> GetFunctions()
        {
            if (!functionsHolders.TryGetValue(typeof(TSource), out var functions))
            {
                functions = new LinqToDBQueryableAsyncFunctions<TSource>();
                functionsHolders.TryAdd(typeof(TSource), functions);
            }

            return (LinqToDBQueryableAsyncFunctions<TSource>)functions;
        }


        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<bool>>> AllAsync { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<bool>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AllAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, bool>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<bool>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<bool>>> AnyAsyncPredicate { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<bool>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AnyAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, bool>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<bool>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<bool>>> AnyAsync { get; } =
            new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<bool>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AnyAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<bool>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<int?>, CancellationToken, Task<double?>>> AverageAsyncIntToDoubleNullable { get; } =
            new Lazy<Func<IQueryable<int?>, CancellationToken, Task<double?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<int?>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<int?>, CancellationToken, Task<double?>>(method);
            });

        public Lazy<Func<IQueryable<long>, CancellationToken, Task<double>>> AverageAsyncLongToDouble { get; } =
            new Lazy<Func<IQueryable<long>, CancellationToken, Task<double>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<long>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<long>, CancellationToken, Task<double>>(method);
            });

        public Lazy<Func<IQueryable<long?>, CancellationToken, Task<double?>>> AverageAsyncLongToDoubleNullable { get; } =
            new Lazy<Func<IQueryable<long?>, CancellationToken, Task<double?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<long?>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<long?>, CancellationToken, Task<double?>>(method);
            });

        public Lazy<Func<IQueryable<float>, CancellationToken, Task<float>>> AverageAsyncFloat { get; } =
            new Lazy<Func<IQueryable<float>, CancellationToken, Task<float>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<float>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<float>, CancellationToken, Task<float>>(method);
            });

        public Lazy<Func<IQueryable<float?>, CancellationToken, Task<float?>>> AverageAsyncNullableFloat { get; } =
            new Lazy<Func<IQueryable<float?>, CancellationToken, Task<float?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<float?>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<float?>, CancellationToken, Task<float?>>(method);
            });

        public Lazy<Func<IQueryable<double>, CancellationToken, Task<double>>> AverageAsyncDouble { get; } =
            new Lazy<Func<IQueryable<double>, CancellationToken, Task<double>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<double>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<double>, CancellationToken, Task<double>>(method);
            });

        public Lazy<Func<IQueryable<double?>, CancellationToken, Task<double?>>> AverageAsyncNullableDouble { get; } =
            new Lazy<Func<IQueryable<double?>, CancellationToken, Task<double?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<double?>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<double?>, CancellationToken, Task<double?>>(method);
            });

        public Lazy<Func<IQueryable<int>, CancellationToken, Task<double>>> AverageAsyncIntToDouble { get; } =
            new Lazy<Func<IQueryable<int>, CancellationToken, Task<double>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<int>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<int>, CancellationToken, Task<double>>(method);
            });

        public Lazy<Func<IQueryable<decimal?>, CancellationToken, Task<decimal?>>> AverageAsyncNullableDecimal { get; } =
            new Lazy<Func<IQueryable<decimal?>, CancellationToken, Task<decimal?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<decimal?>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<decimal?>, CancellationToken, Task<decimal?>>(method);
            });

        public Lazy<Func<IQueryable<decimal>, CancellationToken, Task<decimal>>> AverageAsyncDecimal { get; } =
            new Lazy<Func<IQueryable<decimal>, CancellationToken, Task<decimal>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("AverageAsync", typeof(IQueryable<decimal>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<decimal>, CancellationToken, Task<decimal>>(method);
            });


        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, int?>>, CancellationToken, Task<double?>>> AverageAsyncSelectorIntToDoubleNullable { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, int?>>, CancellationToken, Task<double?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, int?>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                    .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, int?>>, CancellationToken, Task<double?>>(method.MakeGenericMethod(typeof(TSource)));
            });


        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, long>>, CancellationToken, Task<double>>> AverageAsyncSelectorLongToDouble { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, long>>, CancellationToken, Task<double>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, long>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, long>>, CancellationToken, Task<double>>(method.MakeGenericMethod(typeof(TSource)));
            });


        // TODO: Rename
        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, long?>>, CancellationToken, Task<double?>>> AverageAsyncSelectorLongToDoubleNullable { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, long?>>, CancellationToken, Task<double?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, long?>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, long?>>, CancellationToken, Task<double?>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, float>>, CancellationToken, Task<float>>> AverageAsyncSelectorFloat { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, float>>, CancellationToken, Task<float>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, float>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, float>>, CancellationToken, Task<float>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, float?>>, CancellationToken, Task<float?>>> AverageAsyncSelectorNullableFloat { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, float?>>, CancellationToken, Task<float?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, float?>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, float?>>, CancellationToken, Task<float?>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, double>>, CancellationToken, Task<double>>> AverageAsyncSelectorDouble { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, double>>, CancellationToken, Task<double>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, double>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, double>>, CancellationToken, Task<double>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, double?>>, CancellationToken, Task<double?>>> AverageAsyncSelectorNullableDouble { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, double?>>, CancellationToken, Task<double?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, double?>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, double?>>, CancellationToken, Task<double?>>(method.MakeGenericMethod(typeof(TSource)));
            });


        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, int>>, CancellationToken, Task<double>>> AverageAsyncSelectorIntToDouble { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, int>>, CancellationToken, Task<double>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, int>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, int>>, CancellationToken, Task<double>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, decimal?>>, CancellationToken, Task<decimal?>>> AverageAsyncSelectorDecimalNullable { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, decimal?>>, CancellationToken, Task<decimal?>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, decimal?>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, decimal?>>, CancellationToken, Task<decimal?>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, decimal>>, CancellationToken, Task<decimal>>> AverageAsyncSelectorDecimal { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, decimal>>, CancellationToken, Task<decimal>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("AverageAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, decimal>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, decimal>>, CancellationToken, Task<decimal>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, TSource, CancellationToken, Task<bool>>> ContainsAsync { get; } =
            new Lazy<Func<IQueryable<TSource>, TSource, CancellationToken, Task<bool>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("ContainsAsync", typeof(IQueryable<TSource>), typeof(TSource), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, TSource, CancellationToken, Task<bool>>(method.MakeGenericMethod(typeof(TSource)));
            });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<int>>> CountAsync { get; } =
            new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<int>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("CountAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<int>>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<int>>> CountAsyncPredicate { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<int>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("CountAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, bool>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<int>>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>> FirstAsyncPredicate { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("FirstAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, bool>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>> FirstAsync { get; } =
            new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("FirstAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>> FirstOrDefaultAsyncPredicate { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("FirstOrDefaultAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, bool>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>> FirstOrDefaultAsync { get; } =
            new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("FirstOrDefaultAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, Action<TSource>, CancellationToken, Task>> ForEachAsync { get; } =
            new Lazy<Func<IQueryable<TSource>, Action<TSource>, CancellationToken, Task>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("ForEachAsync", typeof(IQueryable<TSource>), typeof(Action<TSource>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Action<TSource>, CancellationToken, Task>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, Func<TSource, bool>, CancellationToken, Task>> ForEachUntilAsync { get; } =
            new Lazy<Func<IQueryable<TSource>, Func<TSource, bool>, CancellationToken, Task>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("ForEachUntilAsync", typeof(IQueryable<TSource>), typeof(Func<TSource, bool>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Func<TSource, bool>, CancellationToken, Task>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<long>>> LongCountAsync { get; } =
            new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<long>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("LongCountAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<long>>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<long>>> LongCountAsyncPredicate { get; } =
            new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<long>>>(() =>
            {
                var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("LongCountAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, bool>>), typeof(CancellationToken));
                return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                            .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<long>>(method.MakeGenericMethod(typeof(TSource)));

            });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>> MaxAsync { get; } =
        new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("MaxAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));
        });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>> MinAsync { get; } =
        new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("MinAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));
        });




        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>> SingleAsync { get; } =
        new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SingleAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));
        });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>> SingleAsyncWithPredicate { get; } =
           new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>>(() =>
           {
               var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SingleAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, bool>>), typeof(CancellationToken));
               return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                   .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));
           });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>> SingleOrDefaultAsync { get; } =
        new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SingleOrDefaultAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));
        });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>> SingleOrDefaultAsyncWithPredicate { get; } =
          new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SingleOrDefaultAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, bool>>), typeof(CancellationToken));
              return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                  .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, bool>>, CancellationToken, Task<TSource>>(method.MakeGenericMethod(typeof(TSource)));
          });

        public Lazy<Func<IQueryable<decimal>, CancellationToken, Task<decimal>>> SumAsyncDecimal { get; } =
        new Lazy<Func<IQueryable<decimal>, CancellationToken, Task<decimal>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<decimal>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<decimal>, CancellationToken, Task<decimal>>(method);
        });

        public Lazy<Func<IQueryable<decimal?>, CancellationToken, Task<decimal?>>> SumAsyncNullableDecimal { get; } =
        new Lazy<Func<IQueryable<decimal?>, CancellationToken, Task<decimal?>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<decimal?>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<decimal?>, CancellationToken, Task<decimal?>>(method);
        });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, decimal>>, CancellationToken, Task<decimal>>> SumAsyncDecimalWithPredicate { get; } =
          new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, decimal>>, CancellationToken, Task<decimal>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, decimal>>), typeof(CancellationToken));
              return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                  .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, decimal>>, CancellationToken, Task<decimal>>(method.MakeGenericMethod(typeof(TSource)));
          });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, decimal?>>, CancellationToken, Task<decimal?>>> SumAsyncNullableDecimalWithPredicate { get; } =
         new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, decimal?>>, CancellationToken, Task<decimal?>>>(() =>
         {
             var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, decimal?>>), typeof(CancellationToken));
             return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                 .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, decimal?>>, CancellationToken, Task<decimal?>>(method.MakeGenericMethod(typeof(TSource)));
         });

        public Lazy<Func<IQueryable<double>, CancellationToken, Task<double>>> SumAsyncDouble { get; } =
        new Lazy<Func<IQueryable<double>, CancellationToken, Task<double>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<double>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<double>, CancellationToken, Task<double>>(method);
        });

        public Lazy<Func<IQueryable<double?>, CancellationToken, Task<double?>>> SumAsyncNullableDouble { get; } =
        new Lazy<Func<IQueryable<double?>, CancellationToken, Task<double?>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<double?>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<double?>, CancellationToken, Task<double?>>(method);
        });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, double>>, CancellationToken, Task<double>>> SumAsyncDoubleWithPredicate { get; } =
          new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, double>>, CancellationToken, Task<double>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, double>>), typeof(CancellationToken));
              return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                  .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, double>>, CancellationToken, Task<double>>(method.MakeGenericMethod(typeof(TSource)));
          });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, double?>>, CancellationToken, Task<double?>>> SumAsyncNullableDoubleWithPredicate { get; } =
         new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, double?>>, CancellationToken, Task<double?>>>(() =>
         {
             var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, double?>>), typeof(CancellationToken));
             return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                 .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, double?>>, CancellationToken, Task<double?>>(method.MakeGenericMethod(typeof(TSource)));
         });

        public Lazy<Func<IQueryable<int>, CancellationToken, Task<int>>> SumAsyncInt { get; } =
        new Lazy<Func<IQueryable<int>, CancellationToken, Task<int>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<int>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<int>, CancellationToken, Task<int>>(method);
        });

        public Lazy<Func<IQueryable<int?>, CancellationToken, Task<int?>>> SumAsyncNullableInt { get; } =
        new Lazy<Func<IQueryable<int?>, CancellationToken, Task<int?>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<int?>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<int?>, CancellationToken, Task<int?>>(method);
        });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, int>>, CancellationToken, Task<int>>> SumAsyncIntWithPredicate { get; } =
          new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, int>>, CancellationToken, Task<int>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, int>>), typeof(CancellationToken));
              return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                  .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, int>>, CancellationToken, Task<int>>(method.MakeGenericMethod(typeof(TSource)));
          });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, int?>>, CancellationToken, Task<int?>>> SumAsyncNullableIntWithPredicate { get; } =
         new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, int?>>, CancellationToken, Task<int?>>>(() =>
         {
             var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, int?>>), typeof(CancellationToken));
             return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                 .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, int?>>, CancellationToken, Task<int?>>(method.MakeGenericMethod(typeof(TSource)));
         });

        public Lazy<Func<IQueryable<float>, CancellationToken, Task<float>>> SumAsyncFloat { get; } =
        new Lazy<Func<IQueryable<float>, CancellationToken, Task<float>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<float>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<float>, CancellationToken, Task<float>>(method);
        });

        public Lazy<Func<IQueryable<float?>, CancellationToken, Task<float?>>> SumAsyncNullableFloat { get; } =
        new Lazy<Func<IQueryable<float?>, CancellationToken, Task<float?>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<float?>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<float?>, CancellationToken, Task<float?>>(method);
        });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, float>>, CancellationToken, Task<float>>> SumAsyncFloatWithPredicate { get; } =
          new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, float>>, CancellationToken, Task<float>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, float>>), typeof(CancellationToken));
              return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                  .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, float>>, CancellationToken, Task<float>>(method.MakeGenericMethod(typeof(TSource)));
          });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, float?>>, CancellationToken, Task<float?>>> SumAsyncNullableFloatWithPredicate { get; } =
         new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, float?>>, CancellationToken, Task<float?>>>(() =>
         {
             var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, float?>>), typeof(CancellationToken));
             return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                 .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, float?>>, CancellationToken, Task<float?>>(method.MakeGenericMethod(typeof(TSource)));
         });

        public Lazy<Func<IQueryable<long>, CancellationToken, Task<long>>> SumAsyncLong { get; } =
        new Lazy<Func<IQueryable<long>, CancellationToken, Task<long>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<long>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<long>, CancellationToken, Task<long>>(method);
        });

        public Lazy<Func<IQueryable<long?>, CancellationToken, Task<long?>>> SumAsyncNullableLong { get; } =
        new Lazy<Func<IQueryable<long?>, CancellationToken, Task<long?>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod("SumAsync", typeof(IQueryable<long?>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
               .BuildFuncStatic<IQueryable<long?>, CancellationToken, Task<long?>>(method);
        });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, long>>, CancellationToken, Task<long>>> SumAsyncLongWithPredicate { get; } =
          new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, long>>, CancellationToken, Task<long>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, long>>), typeof(CancellationToken));
              return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                  .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, long>>, CancellationToken, Task<long>>(method.MakeGenericMethod(typeof(TSource)));
          });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, long?>>, CancellationToken, Task<long?>>> SumAsyncNullableLongWithPredicate { get; } =
         new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, long?>>, CancellationToken, Task<long?>>>(() =>
         {
             var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("SumAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, long?>>), typeof(CancellationToken));
             return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                 .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, long?>>, CancellationToken, Task<long?>>(method.MakeGenericMethod(typeof(TSource)));
         });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource[]>>> ToArrayAsync { get; } =
        new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<TSource[]>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("ToArrayAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<TSource[]>>(method.MakeGenericMethod(typeof(TSource)));
        });

        public Lazy<Func<IQueryable<TSource>, CancellationToken, Task<List<TSource>>>> ToListAsync { get; } =
        new Lazy<Func<IQueryable<TSource>, CancellationToken, Task<List<TSource>>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource>("ToListAsync", typeof(IQueryable<TSource>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                .BuildFuncStatic<IQueryable<TSource>, CancellationToken, Task<List<TSource>>>(method.MakeGenericMethod(typeof(TSource)));
        });
    }

    class LinqToDBQueryableAsyncFunctions<TSource, TResult>
    {
        private static ConcurrentDictionary<Type, object> functionsHolders = new ConcurrentDictionary<Type, object>();

        public static LinqToDBQueryableAsyncFunctions<TSource, TResult> GetFunctions()
        {
            if (!functionsHolders.TryGetValue(typeof(TSource), out var functions))
            {
                functions = new LinqToDBQueryableAsyncFunctions<TSource, TResult>();
                functionsHolders.TryAdd(typeof(TSource), functions);
            }

            return (LinqToDBQueryableAsyncFunctions<TSource, TResult>)functions;
        }


        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, TResult>>, CancellationToken, Task<TResult>>> MaxAsync { get; } =
          new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, TResult>>, CancellationToken, Task<TResult>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource, TResult>("MaxAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, TResult>>), typeof(CancellationToken));
              return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                      .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, TResult>>, CancellationToken, Task<TResult>>(method.MakeGenericMethod(typeof(TSource), typeof(TResult)));
          });

        public Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, TResult>>, CancellationToken, Task<TResult>>> MinAsync { get; } =
        new Lazy<Func<IQueryable<TSource>, Expression<Func<TSource, TResult>>, CancellationToken, Task<TResult>>>(() =>
        {
            var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource, TResult>("MinAsync", typeof(IQueryable<TSource>), typeof(Expression<Func<TSource, TResult>>), typeof(CancellationToken));
            return Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                .BuildFuncStatic<IQueryable<TSource>, Expression<Func<TSource, TResult>>, CancellationToken, Task<TResult>>(method.MakeGenericMethod(typeof(TSource), typeof(TResult)));
        });
    }

    class LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey>
    {
        private static ConcurrentDictionary<Type, object> functionsHolders = new ConcurrentDictionary<Type, object>();

        public static LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey> GetFunctions()
        {
            if (!functionsHolders.TryGetValue(typeof(TSource), out var functions))
            {
                functions = new LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey>();
                functionsHolders.TryAdd(typeof(TSource), functions);
            }

            return (LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey>)functions;
        }

        public Lazy<Func<IQueryable<TSource>, Func<TSource, TKey>, IEqualityComparer<TKey>, CancellationToken, Task<Dictionary<TKey, TSource>>>> ToDictionaryAsyncWithComparer { get; } =
          new Lazy<Func<IQueryable<TSource>, Func<TSource, TKey>, IEqualityComparer<TKey>, CancellationToken, Task<Dictionary<TKey, TSource>>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource, TKey>("ToDictionaryAsync", typeof(IQueryable<TSource>), typeof(Func<TSource, TKey>), typeof(IEqualityComparer<TKey>), typeof(CancellationToken));
              return (Func<IQueryable<TSource>, Func<TSource, TKey>, IEqualityComparer<TKey>, CancellationToken, Task<Dictionary<TKey, TSource>>>)
                    Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                        .BuildFromTypes(method.MakeGenericMethod(typeof(TSource), typeof(TKey)), null,
                        new[] { typeof(IQueryable<TSource>), typeof(Func<TSource, TKey>), typeof(IEqualityComparer<TKey>), typeof(CancellationToken) }, typeof(Task<Dictionary<TKey, TSource>>));
          });

        public Lazy<Func<IQueryable<TSource>, Func<TSource, TKey>, CancellationToken, Task<Dictionary<TKey, TSource>>>> ToDictionaryAsync { get; } =
          new Lazy<Func<IQueryable<TSource>, Func<TSource, TKey>, CancellationToken, Task<Dictionary<TKey, TSource>>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource, TKey>("ToDictionaryAsync", typeof(IQueryable<TSource>), typeof(Func<TSource, TKey>), typeof(CancellationToken));
              return (Func<IQueryable<TSource>, Func<TSource, TKey>, CancellationToken, Task<Dictionary<TKey, TSource>>>)
                    Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                        .BuildFromTypes(method.MakeGenericMethod(typeof(TSource), typeof(TKey)), null,
                        new[] { typeof(IQueryable<TSource>), typeof(Func<TSource, TKey>), typeof(CancellationToken) }, typeof(Task<Dictionary<TKey, TSource>>));
          });
    }

    class LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey, TElement>
    {
        private static ConcurrentDictionary<Type, object> functionsHolders = new ConcurrentDictionary<Type, object>();

        public static LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey, TElement> GetFunctions()
        {
            if (!functionsHolders.TryGetValue(typeof(TSource), out var functions))
            {
                functions = new LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey, TElement>();
                functionsHolders.TryAdd(typeof(TSource), functions);
            }

            return (LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey, TElement>)functions;
        }

        public Lazy<Func<IQueryable<TSource>, Func<TSource, TKey>, Func<TSource, TElement>, IEqualityComparer<TKey>, CancellationToken, Task<Dictionary<TKey, TElement>>>> ToDictionaryAsyncWithComparer { get; } =
          new Lazy<Func<IQueryable<TSource>, Func<TSource, TKey>, Func<TSource, TElement>, IEqualityComparer<TKey>, CancellationToken, Task<Dictionary<TKey, TElement>>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource, TKey, TElement>("ToDictionaryAsync", typeof(IQueryable<TSource>), typeof(Func<TSource, TKey>), typeof(Func<TSource, TElement>), typeof(IEqualityComparer<TKey>), typeof(CancellationToken));
              return (Func<IQueryable<TSource>, Func<TSource, TKey>, Func<TSource, TElement>, IEqualityComparer<TKey>, CancellationToken, Task<Dictionary<TKey, TElement>>>)
                    Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                        .BuildFromTypes(method.MakeGenericMethod(typeof(TSource), typeof(TKey), typeof(TElement)), null,
                        new[] { typeof(IQueryable<TSource>), typeof(Func<TSource, TKey>), typeof(Func<TSource, TElement>), typeof(IEqualityComparer<TKey>), typeof(CancellationToken) }, typeof(Task<Dictionary<TKey, TElement>>));
          });

        public Lazy<Func<IQueryable<TSource>, Func<TSource, TKey>, Func<TSource, TElement>, CancellationToken, Task<Dictionary<TKey, TElement>>>> ToDictionaryAsync { get; } =
          new Lazy<Func<IQueryable<TSource>, Func<TSource, TKey>, Func<TSource, TElement>, CancellationToken, Task<Dictionary<TKey, TElement>>>>(() =>
          {
              var method = LinqToDBQueryableAsyncMethodHelper.GetMethod<TSource, TKey, TElement>("ToDictionaryAsync", typeof(IQueryable<TSource>), typeof(Func<TSource, TKey>), typeof(Func<TSource, TElement>), typeof(CancellationToken));
              return (Func<IQueryable<TSource>, Func<TSource, TKey>, Func<TSource, TElement>, CancellationToken, Task<Dictionary<TKey, TElement>>>)
                    Dynamix.Reflection.MemberAccessorDelegateBuilder.MethodBuilder
                        .BuildFromTypes(method.MakeGenericMethod(typeof(TSource), typeof(TKey), typeof(TElement)), null,
                        new[] { typeof(IQueryable<TSource>), typeof(Func<TSource, TKey>), typeof(Func<TSource, TElement>), typeof(CancellationToken) }, typeof(Task<Dictionary<TKey, TElement>>));
          });
    }


    class LinqToDBQueryableAsyncExecutor : IQueryableAsyncExecutor
    {
        public Task<bool> AllAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AllAsync.Value(source, predicate, token);
        }

        public Task<bool> AnyAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AnyAsyncPredicate.Value(source, predicate, token);
        }

        public Task<bool> AnyAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AnyAsync.Value(source, token);
        }

        public Task<decimal?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorDecimalNullable.Value(source, selector, token);
        }

        public Task<double?> AverageAsync(IQueryable<int?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<int?>.GetFunctions().AverageAsyncIntToDoubleNullable.Value(source, token);
        }

        public Task<double> AverageAsync(IQueryable<long> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<long>.GetFunctions().AverageAsyncLongToDouble.Value(source, token);
        }

        public Task<double?> AverageAsync(IQueryable<long?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<long?>.GetFunctions().AverageAsyncLongToDoubleNullable.Value(source, token);
        }

        public Task<float> AverageAsync(IQueryable<float> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<float>.GetFunctions().AverageAsyncFloat.Value(source, token);
        }

        public Task<float?> AverageAsync(IQueryable<float?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<float?>.GetFunctions().AverageAsyncNullableFloat.Value(source, token);
        }

        public Task<double> AverageAsync(IQueryable<double> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<double>.GetFunctions().AverageAsyncDouble.Value(source, token);
        }

        public Task<double?> AverageAsync(IQueryable<double?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<double?>.GetFunctions().AverageAsyncNullableDouble.Value(source, token);
        }

        public Task<double> AverageAsync(IQueryable<int> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<int>.GetFunctions().AverageAsyncIntToDouble.Value(source, token);
        }

        public Task<decimal?> AverageAsync(IQueryable<decimal?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<decimal?>.GetFunctions().AverageAsyncNullableDecimal.Value(source, token);
        }

        public Task<decimal> AverageAsync(IQueryable<decimal> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<decimal>.GetFunctions().AverageAsyncDecimal.Value(source, token);
        }

        public Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorIntToDoubleNullable.Value(source, selector, token);
        }

        public Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorLongToDouble.Value(source, selector, token);
        }

        public Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorLongToDoubleNullable.Value(source, selector, token);
        }

        public Task<float> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorFloat.Value(source, selector, token);
        }

        public Task<float?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorNullableFloat.Value(source, selector, token);
        }

        public Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorDouble.Value(source, selector, token);
        }

        public Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorNullableDouble.Value(source, selector, token);
        }

        public Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorIntToDouble.Value(source, selector, token);
        }

        public Task<decimal> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().AverageAsyncSelectorDecimal.Value(source, selector, token);
        }

        public Task<bool> ContainsAsync<TSource>(IQueryable<TSource> source, TSource item, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().ContainsAsync.Value(source, item, token);
        }

        public Task<int> CountAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().CountAsync.Value(source, token);
        }

        public Task<int> CountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().CountAsyncPredicate.Value(source, predicate, token);
        }

        public Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().FirstAsyncPredicate.Value(source, predicate, token);
        }

        public Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().FirstAsync.Value(source, token);
        }

        public Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().FirstOrDefaultAsyncPredicate.Value(source, predicate, token);
        }

        public Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().FirstOrDefaultAsync.Value(source, token);
        }

        public Task ForEachAsync<TSource>(IQueryable<TSource> source, Action<TSource> action, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().ForEachAsync.Value(source, action, token);
        }

        public Task ForEachUntilAsync<TSource>(IQueryable<TSource> source, Func<TSource, bool> func, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().ForEachUntilAsync.Value(source, func, token);
        }

        public Task<long> LongCountAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().LongCountAsync.Value(source, token);
        }

        public Task<long> LongCountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().LongCountAsyncPredicate.Value(source, predicate, token);
        }

        public Task<TResult> MaxAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource, TResult>.GetFunctions().MaxAsync.Value(source, selector, token);
        }

        public Task<TResult> MinAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource, TResult>.GetFunctions().MinAsync.Value(source, selector, token);
        }

        public Task<TSource> MaxAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().MaxAsync.Value(source, token);
        }

        public Task<TSource> MinAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().MinAsync.Value(source, token);
        }


        public Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SingleAsync.Value(source, token);
        }

        public Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SingleAsyncWithPredicate.Value(source, predicate, token);
        }

        public Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SingleOrDefaultAsync.Value(source, token);
        }

        public Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SingleOrDefaultAsyncWithPredicate.Value(source, predicate, token);
        }

        public Task<decimal> SumAsync(IQueryable<decimal> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<decimal>.GetFunctions().SumAsyncDecimal.Value(source, token);
        }

        public Task<decimal?> SumAsync(IQueryable<decimal?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<decimal?>.GetFunctions().SumAsyncNullableDecimal.Value(source, token);
        }

        public Task<decimal> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncDecimalWithPredicate.Value(source, selector, token);
        }

        public Task<decimal?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncNullableDecimalWithPredicate.Value(source, selector, token);
        }

        public Task<double> SumAsync(IQueryable<double> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<decimal>.GetFunctions().SumAsyncDouble.Value(source, token);
        }

        public Task<double?> SumAsync(IQueryable<double?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<decimal>.GetFunctions().SumAsyncNullableDouble.Value(source, token);
        }

        public Task<double> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncDoubleWithPredicate.Value(source, selector, token);
        }

        public Task<double?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncNullableDoubleWithPredicate.Value(source, selector, token);
        }

        public Task<int> SumAsync(IQueryable<int> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<int>.GetFunctions().SumAsyncInt.Value(source, token);
        }

        public Task<int?> SumAsync(IQueryable<int?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<int>.GetFunctions().SumAsyncNullableInt.Value(source, token);
        }

        public Task<int> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncIntWithPredicate.Value(source, selector, token);
        }

        public Task<int?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncNullableIntWithPredicate.Value(source, selector, token);
        }

        public Task<float> SumAsync(IQueryable<float> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<float>.GetFunctions().SumAsyncFloat.Value(source, token);
        }

        public Task<float?> SumAsync(IQueryable<float?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<float>.GetFunctions().SumAsyncNullableFloat.Value(source, token);
        }

        public Task<float> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncFloatWithPredicate.Value(source, selector, token);
        }

        public Task<float?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncNullableFloatWithPredicate.Value(source, selector, token);
        }

        public Task<long> SumAsync(IQueryable<long> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<long>.GetFunctions().SumAsyncLong.Value(source, token);
        }

        public Task<long?> SumAsync(IQueryable<long?> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<long>.GetFunctions().SumAsyncNullableLong.Value(source, token);
        }

        public Task<long> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncLongWithPredicate.Value(source, selector, token);
        }

        public Task<long?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().SumAsyncNullableLongWithPredicate.Value(source, selector, token);
        }


        public Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().ToListAsync.Value(source, token);
        }


        public Task<TSource[]> ToArrayAsync<TSource>(IQueryable<TSource> source, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctions<TSource>.GetFunctions().ToArrayAsync.Value(source, token);
        }


        public Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey, TElement>.GetFunctions().ToDictionaryAsyncWithComparer.Value(source, keySelector, elementSelector, comparer, token);
        }

        public Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey, TElement>.GetFunctions().ToDictionaryAsync.Value(source, keySelector, elementSelector, token);
        }

        public Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey>.GetFunctions().ToDictionaryAsyncWithComparer.Value(source, keySelector, comparer, token);
        }

        public Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken token = default(CancellationToken))
        {
            return LinqToDBQueryableAsyncFunctionsDictionary<TSource, TKey>.GetFunctions().ToDictionaryAsync.Value(source, keySelector, token);
        }
    }
}
