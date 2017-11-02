using Dalia.AsyncExtensions;
using NUnit.Framework;
using System;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using System.Data.Common;
using System.Linq.Expressions;
using LinqToDB.Mapping;

namespace Dalia.Tests
{
    [TestFixture]
    public class AsyncExtensionsTests
    {
        public class Customer
        {
            public int Id { get; set; }
            public int Int { get; set; }
            public int? NullableInt { get; set; }
            public long Long { get; set; }
            public long? NullableLong { get; set; }
            public float Float { get; set; }
            public float? NullableFloat { get; set; }
            public double Double { get; set; }
            public double? NullableDouble { get; set; }
            public decimal Decimal { get; set; }
            public decimal? NullableDecimal { get; set; }

        }

        public class TestAsyncMethodsException : Exception { }

        //public class TestProvider : LinqToDB.DataProvider.IDataProvider
        //{
        //    public string Name => nameof(TestProvider);

        //    public string ConnectionNamespace => throw new NotImplementedException();

        //    public Type DataReaderType => throw new NotImplementedException();

        //    public MappingSchema MappingSchema => new MappingSchema();

        //    public SqlProviderFlags SqlProviderFlags => throw new NotImplementedException();

        //    public BulkCopyRowsCopied BulkCopy<T>(DataConnection dataConnection, BulkCopyOptions options, IEnumerable<T> source)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public Type ConvertParameterType(Type type, DataType dataType)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public IDbConnection CreateConnection(string connectionString)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public ISqlBuilder CreateSqlBuilder()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void DisposeCommand(DataConnection dataConnection)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public IDisposable ExecuteScope()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public CommandBehavior GetCommandBehavior(CommandBehavior commandBehavior)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public object GetConnectionInfo(DataConnection dataConnection, string parameterName)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public Expression GetReaderExpression(MappingSchema mappingSchema, IDataReader reader, int idx, Expression readerExpression, Type toType)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public ISchemaProvider GetSchemaProvider()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public ISqlOptimizer GetSqlOptimizer()
        //    {
        //        throw new TestAsyncMethodsException();
        //    }

        //    public void InitCommand(DataConnection dataConnection, CommandType commandType, string commandText, DataParameter[] parameters)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public bool IsCompatibleConnection(IDbConnection connection)
        //    {
        //        return true;
        //    }

        //    public bool? IsDBNullAllowed(IDataReader reader, int idx)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public int Merge<T>(DataConnection dataConnection, Expression<Func<T, bool>> predicate, bool delete, IEnumerable<T> source, string tableName, string databaseName, string schemaName) where T : class
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public int Merge<TTarget, TSource>(DataConnection dataConnection, IMergeable<TTarget, TSource> merge)
        //        where TTarget : class
        //        where TSource : class
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public Task<int> MergeAsync<T>(DataConnection dataConnection, Expression<Func<T, bool>> predicate, bool delete, IEnumerable<T> source, string tableName, string databaseName, string schemaName, CancellationToken token) where T : class
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public Task<int> MergeAsync<TTarget, TSource>(DataConnection dataConnection, IMergeable<TTarget, TSource> merge, CancellationToken token)
        //        where TTarget : class
        //        where TSource : class
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void SetParameter(IDbDataParameter parameter, string name, DataType dataType, object value)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        #pragma warning disable CS1998
        [Test]
        public async Task Linq2dbAsyncTestAsync()
        {

            var mockConn = new Mock<IDbConnection>();

            var mockProvider = new Mock<LinqToDB.DataProvider.IDataProvider>();
            mockProvider.Setup(x => x.Name).Returns("TestProvider");
            mockProvider.Setup(x => x.IsCompatibleConnection(It.IsAny<IDbConnection>())).Returns(true);
            mockProvider.Setup(x => x.MappingSchema).Returns(new MappingSchema());
            mockProvider.Setup(x => x.GetSqlOptimizer()).Throws(new TestAsyncMethodsException());

            var c = new LinqToDB.Data.DataConnection(mockProvider.Object, mockConn.Object);
            var l = c.GetTable<Customer>();

            Assert.That(async () => { return await QueryableAsyncExtensions.AllAsync(l, x => x.Int > 0); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AnyAsync(l, x => x.Int > 0); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AnyAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.NullableDecimal).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.Decimal).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.NullableDouble).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.Double).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.NullableFloat).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.Float).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.NullableInt).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.Int).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.Int).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.NullableLong).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l.Select(cust => cust.Long).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.NullableDecimal); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.Decimal); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.NullableDouble); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.Double); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.NullableFloat); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.Float); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.NullableInt); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.Int); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.Int); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.NullableLong); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.AverageAsync(l, cust => cust.Long); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.ContainsAsync(l, new Customer()); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.CountAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.CountAsync(l, x => x.Id > 0); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.FirstAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.FirstAsync(l, cust => cust.Int > 0); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.FirstOrDefaultAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.FirstOrDefaultAsync(l, cust => cust.Int > 0); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { await QueryableAsyncExtensions.ForEachAsync<Customer>(l, customer => { }); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { await QueryableAsyncExtensions.ForEachUntilAsync(l, cust => cust.Int > 0); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.LongCountAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.LongCountAsync(l, cust => cust.Int > 0); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.MaxAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.MaxAsync(l, cust => cust.Long > 0); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.MinAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.MinAsync(l, cust => cust.Long > 0); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.SingleAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SingleAsync(l, cust => cust.Long > 0); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.SingleOrDefaultAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SingleOrDefaultAsync(l, cust => cust.Long > 0); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.NullableDecimal).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.Decimal).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.NullableDouble).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.Double).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.NullableFloat).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.Float).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.NullableInt).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.Int).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.Int).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.NullableLong).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l.Select(cust => cust.Long).AsQueryable()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.NullableDecimal); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.Decimal); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.NullableDouble); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.Double); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.NullableFloat); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.Float); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.NullableInt); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.Int); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.Int); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.NullableLong); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.SumAsync(l, cust => cust.Long); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.ToArrayAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.ToDictionaryAsync(l, (x) => x.Id); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.ToDictionaryAsync(l, (x) => x.Id, x => x.Float); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.ToDictionaryAsync(l, (x) => x.Id, x => x.Float, new IntEqualityComparer()); }, Throws.TypeOf<TestAsyncMethodsException>());
            Assert.That(async () => { return await QueryableAsyncExtensions.ToDictionaryAsync(l, (x) => x.Id, new IntEqualityComparer()); }, Throws.TypeOf<TestAsyncMethodsException>());

            Assert.That(async () => { return await QueryableAsyncExtensions.ToListAsync(l); }, Throws.TypeOf<TestAsyncMethodsException>());

        }
        #pragma warning restore CS1998

        class IntEqualityComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x == y;
            }

            public int GetHashCode(int obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}