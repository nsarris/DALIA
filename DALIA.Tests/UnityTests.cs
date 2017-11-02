using Dalia.DI.Unity;
using Dalia.UI.Unity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Unity;
using System.Data.Common;
using Dalia.DI;
using Dalia.Repositories;
using Dalia.Mapping;

namespace Dalia.Tests
{
    [TestFixture]
    class UnityTests
    {

        [SetUp]
        public void StartUp()
        {
           
        }

        [Test]
        public void UnityResolverTests()
        {
            var container = new UnityContainer();

            var dsSqlServer1 = new DataSource("ICESQLDB1", ProviderTypes.SqlServer, "Data Source=icermauatsql;Initial Catalog=ICE2DB;User ID=ICEUAT;Password=ic3uat!@");
            var dsSqlServer2 = new DataSource("ICESQLDB2", ProviderTypes.SqlServer, "Data Source=icermauatsql;Initial Catalog=ICE2DB;User ID=ICEUAT;Password=ic3uat!@");
            //var dsAs400 = new DataSource("ICEDB", ProviderTypes.DB2iSeries, "DataCompression=True; DataSource=ATHINA; Password=XK40XK40; UserID=XK40;");

            var dataSources = new DataSources() { dsSqlServer1, dsSqlServer2 };

            var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MockEntityModel, MockEntity>();
            });

            var mapper = mapperConfig.CreateMapper();

            var unityBoostraper = new UnityDaliaResolverBoostraper(container, dataSources, new Mapping.AutoMapper.Mapper(mapper));
            unityBoostraper.Extend(new UnityLinqToDBResolverRegistrator());

            var dalResolver = container.Resolve<IDaliaResolver>();

            var childContainer1 = container.CreateChildContainer();
            var dalResolver1 = childContainer1.Resolve<IDaliaResolver>();

            var childContainer2 = container.CreateChildContainer();
            var dalResolver2 = childContainer2.Resolve<IDaliaResolver>();
            


            var context = dalResolver.ResolveSingleContext<MockContext>();
            
            var context1 = dalResolver1.ResolveSingleContext<MockContext>();
            var context1_1 = dalResolver1.ResolveSingleContext<MockContext>();            
            var context1_2 = dalResolver1.ResolveNewContext<MockContext>();            
            var context2 = dalResolver2.ResolveSingleContext<MockContext>();

            var dataRepository = dalResolver.ResolveRepository<MockRepository>();
            var dataRepository1 = dalResolver1.ResolveRepository<MockRepository>();
            var dataRepository1_1 = dalResolver1.ResolveRepository<MockRepository>();
            var dataRepository2 = dalResolver2.ResolveRepository<MockRepository>();


            var dataQueryprovider = dalResolver.ResolveQueryProvider<MockQueryProvider>();
            var dataQueryprovider1 = dalResolver1.ResolveQueryProvider<MockQueryProvider>();
            var dataQueryprovider1_1 = dalResolver1.ResolveQueryProvider<MockQueryProvider>();
            var dataQueryprovider2 = dalResolver2.ResolveQueryProvider<MockQueryProvider>();

            Assert.AreNotSame(context, context1);
            Assert.AreNotSame(context1, context2);
            Assert.AreSame(context1, context1_1);           
            Assert.AreNotSame(context1, context1_2);

            Assert.AreNotSame(dataRepository1.Context, dataRepository.Context);
            Assert.AreSame(dataRepository1.Context, dataRepository1_1.Context);
            Assert.AreNotSame(dataRepository2.Context, dataRepository.Context);
            Assert.AreNotSame(dataRepository2.Context, dataRepository1.Context);

            Assert.AreNotSame(dataQueryprovider1.Context, dataQueryprovider.Context);
            Assert.AreSame(dataQueryprovider1.Context, dataQueryprovider1_1.Context);
            Assert.AreNotSame(dataQueryprovider2.Context, dataQueryprovider.Context);
            Assert.AreNotSame(dataQueryprovider2.Context, dataQueryprovider1.Context);
        }
    }

    public class MockDataConnection : Dalia.Linq2db.LinqToDBDataConnection
    {
        public MockDataConnection(IDataSource dataSource) : base(dataSource)
        {
        }

        public MockDataConnection(IDataSource dataSource, DbConnection connection) : base(dataSource, connection)
        {
        }

        public MockDataConnection(string providerName, string connectionString) : base(providerName, connectionString)
        {
        }
    }

    [DefaultDataSource("ICESQLDB1")]
    public class MockContext : Dalia.Linq2db.LinqToDBDataContext<MockDataConnection>
    {
        public MockContext(IDataSource dataSource) : base(dataSource)
        {
        }

        public MockContext(MockDataConnection dataConnection) : base(dataConnection)
        {
        }

        public MockContext(string provider, string connectionString) : base(provider, connectionString)
        {
        }
    }

    public class MockEntity //: DomainEntity 
    {
       
    }

    public class MockDTO
    {

    }

    public class MockEntityModel
    {

    }

    public class MockRepository : MappedRepository<MockEntityModel, MockEntity>, IRepository
    {
        public MockRepository(MockContext context, IObjectMapper mapper) : base(context, mapper)
        {
        }
    }

    public class MockQueryProvider : DtoQueryProvider<MockEntityModel, MockDTO>
    {
        public MockQueryProvider(MockContext context, IObjectMapper mapper) : base(context, mapper)
        {
        }
    }

   
}
