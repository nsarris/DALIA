using System.Text;
using System.Threading.Tasks;
using Dalia.AsyncExtensions;
using Dalia;
using Dalia.DomainObjects;
using AutoMapperExtensions;
using Unity;
using Unity.Resolution;
using Dalia.DI.Unity;
using LinqToDB.Data;
using Dalia.UI.Unity;

namespace DALIA.DebugTest
{
    class Program
    {
        static void Main(string[] args)
        {




            //var ictx = new LinqToDBDataContext("DB2.iSeries", GetConnectionString());


            //par.iDB2DbType = iDB2DbType.
            //var r = ictx.Query<DataModel.ICE.QueryResultItem>().FirstOrDefaultAsync().Result;
            //var r = context.Query<DataModel.Customer>().AllAsync(x => x.CustomerID != null).Result;
            //var r = context.Query<DataModel.Customer>().MaxAsync(x => x.Orders.Count()).Result;
            //context.exe
            //var r1 = context.ExecuteScalar<string>("SELECT TOP 1 CustomerID FROM Customers");
            //var r2 = context.ExecuteScalarList<string>("SELECT CustomerID FROM Customers");
            //var r3 = context.ExecuteDynamic("SELECT TOP 10 * FROM Customers");
            //var r4 = context.ExecuteNonQuery("SELECT * FROM Customers");
            //var r5 = context.SelectFromQuery(typeof(DataModel.Northwind.Customer), "SELECT TOP 10 * FROM Customers WHERE ContactTitle=@ContactTitle", QueryParameters.FromValue("Owner", "ContactTitle"));
            //var r6 = context.SelectFromQuery<DataModel.Northwind.Customer>("SELECT TOP 10 * FROM Customers WHERE ContactTitle=@ContactTitle", QueryParameters.FromValue("Owner", "ContactTitle"));
            //var c = context.Query<DataModel.Northwind.Customer>().FirstOrDefault();

            //var ctx2 = new LinqToDBDataContext("SqlServer", "Server=.;Database=Northwnd;Integrated Security=true");
            //var p = ctx2.SchemaModel.GetKeyPredicate<DataModel.Northwind.Customer>(new DataModel.Northwind.Customer() { CustomerID = "NIKOS" });

            //var config = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<DataModel.Northwind.Customer, DomainModel.Customer>());
            //var config = new AutoMapper.MapperConfiguration(cfg => 
            //{
            //    //cfg.CreateMissingTypeMaps = false;
            //    //cfg.ShouldMapProperty = p => false;
            //    cfg.AddProfile<NoAutoMappingProfile>();
            //    //cfg.CreateMap<DataModel.Northwind.Customer, DomainModel.Customer>()
            //    //cfg.FromExpression((DataModel.Northwind.Customer c) => new DomainModel.Customer
            //    //{
            //    //    CustomerID = c.CustomerID,
            //    //    Address = c.Orders.Count().ToString()
            //    //});

            //});
            //config.CompileMappings();
            var config = AutoMapperAutoConfig.Config();
            var mapper = config.CreateMapper();

            //var context = new NorthWindDataContext("SqlServer", "Server=.;Database=Northwnd;Integrated Security=true");

            //var repo = new EntityQueryProvider<DataModel.Northwind.Customer, DomainModel.Customer>(context, new Mapper(mapper));



            //var c2 = repo.ToQueryable().FirstAsync().Result;
            //var c3 = repo.GetById("ALFKI");
            //var c4 = repo.GetByIdAsync("ALFKI").Result;

            //var q5 = repo.QueryById("ALFKI").Select(x => x.Address);
            //var sq5 = q5.ToString();


            //var c5 = q5.SingleOrDefault();

            var ds = new DataSource("Northwind", ProviderTypes.SqlServer, "Server=.;Database=Northwnd;Integrated Security=true");
            var ds2 = new DataSource("Northwind2", ProviderTypes.SqlServer, "Server=.;Database=Northwnd;Integrated Security=true");
            var dataSources = new DataSources { ds, ds2 };
            var container = new UnityContainer();

            ///container.RegisterInstance<IObjectMapper>(new Mapper(mapper));
            //container.RegisterInstance<IDataSource>(ds.Key,ds);
            //container.RegisterInstance<IDataSources>(dataSources);

            //UnityDataContextFactory.RegisterFactories(container);

            var unityBoostraper = new UnityDaliaResolverBoostraper(container, dataSources, new Mapper(mapper));
            unityBoostraper.Extend(new UnityLinqToDBResolverRegistrator());

            //UnityDaliaResolver.Init(container, dataSources);


            var child = container.CreateChildContainer();
            var child2 = container.CreateChildContainer();

            var f = container.Resolve<UnityDaliaResolver>();
            
            var f1 = child.Resolve<UnityDaliaResolver>();
            var f1a = child.Resolve<IUnityDaliaResolver>();
            var f1b = child.Resolve<IDaliaResolver>();

            var f2 = child2.Resolve<UnityDaliaResolver>();

            var ctx = container.Resolve<NorthWindDataContext>();
            var ctx2 = container.Resolve<NorthWindDataContext>();

            var ctxa = container.Resolve<NorthWindDataContext>("Northwind2");
            var ctxb = container.Resolve<NorthWindDataContext>("Northwind2");

            var dbcon = container.Resolve<System.Data.SqlClient.SqlConnection>("Northwind2");

            var ctx11 = child.Resolve<NorthWindDataContext>();
            var ctx12 = child.Resolve<NorthWindDataContext>();

            //var ctx21 = child2.Resolve<NorthWindDataContext>();
            //var ctx22 = child2.Resolve<NorthWindDataContext>();

            var ctx111 = f.ResolveSingleContext<NorthWindDataContext>();
            var ctx112 = f.ResolveSingleContext<NorthWindDataContext>("Northwind2");
            var ctx113 = f.ResolveNewContext<NorthWindDataContext>();
            var ctx114 = f.ResolveNewContext<NorthWindDataContext>();
            var ctx115 = f.ResolveNewContext<NorthWindDataContext>("Northwind2");

            //container.RegisterType<NorthWindDataContext>(new Unity.Lifetime.HierarchicalLifetimeManager());
            //var c1 = container.Resolve<NorthWindDataContext>();
            //var c2 = container.Resolve<NorthWindDataContext>();

            //var f = container.RegisterType(typeof(NorthWindDataContext), new InjectionFactory(c => c.Resolve(typeof(NorthWindDataContext), new DependencyOverride<IDataSource>(ds))));

            //container.RegisterType(typeof(SqlConnection), new InjectionConstructor(new ResolvedParameter<string>()));
            //container.RegisterType(typeof(NorthwndDB), new InjectionConstructor(new ResolvedParameter<DbConnection>()));


            //var f = child.Resolve<IDataContextFactory>();
            //var context = f.Current<NorthWindDataContext>();

            //using (var repo = container.Resolve<CustomerRepository>())
            //{

            //}
        }
    }





}

