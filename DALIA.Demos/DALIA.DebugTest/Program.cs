using Dalia.Linq2db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalia.AsyncExtensions;
using IBM.Data.DB2.iSeries;
using System.Data.Common;
using Dalia;
using DataModel.Northwind;
using Dalia.Schema;
using Dalia.Linq2db.Schema;
using LinqToDB.Data;
using Dalia.DomainObjects;
//using LinqToDB;

namespace DALIA.DebugTest
{
    public class NorthwindSchemaInitializer : ILinqToDBSchemaInitializer
    {
        public void Init(DataConnection dataConnection)
        {
            dataConnection.MappingSchema.GetEntityDescriptor(typeof(Customer));
            dataConnection.MappingSchema.GetEntityDescriptor(typeof(Order));
        }
    }

    [SchemaDescriptorInitializer(typeof(NorthwindSchemaInitializer))]
    public class C1 : LinqToDBDataContext<NorthwndDB>
    {
        public C1(string provider, string connectionString) : base(provider, connectionString)
        {
            this.Linq2DBDataConnection.MappingSchema.GetEntityDescriptor(typeof(Customer));
        }

        public IQueryable<Customer> Customers => this.Linq2DBDataConnection.GetTable<Customer>();

        public IQueryable<Customer> FTestCusomters(int? p1)
        {
            return this.Linq2DBDataConnection.FTestCustomers(p1);
        }
    }

    public class C2 : LinqToDBDataContext<NorthwndDB>
    {
        public C2(string provider, string connectionString) : base(provider, connectionString)
        {
        }

        public IQueryable<Order> Customers => this.Linq2DBDataConnection.GetTable<Order>();
    }


    class Program
    {
        static string GetConnectionString()
        {
            var connStrBuilder = new iDB2ConnectionStringBuilder()
            {
                DataSource = "ATHINA",
                UserID = "XK40",
                Password = "XK40XK40",
                DataCompression = true
            };
            return connStrBuilder.ConnectionString;
        }

        

        

        static void Main(string[] args)
        {
            var context = new C1("SqlServer", "Server=.\\SQL2016;Database=Northwnd;Integrated Security=true");
            //context.Query<Customer>();
            //var context2 = new C2("SqlServer", "Server=.\\SQL2016;Database=Northwnd;Integrated Security=true");
            //context2.Query<Order>();
            

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
            var c = context.Query<DataModel.Northwind.Customer>().FirstOrDefault();


            //var p = context.SchemaModel.GetKeyPredicate<DataModel.Northwind.Customer>(new DataModel.Northwind.Customer() { CustomerID = "ALFKI" });
            var p = context.SchemaModel.GetKeyPredicate<DataModel.Northwind.Customer>(c);

            var c1 = context.Query<Customer>().FirstOrDefaultAsync(p).Result;


            var config = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<DataModel.Northwind.Customer, DomainModel.Customer>());
            var mapper = config.CreateMapper();

            var repo = new EntityQueryProvider<DataModel.Northwind.Customer, DomainModel.Customer>(context, new Mapper(mapper));
            var c2 = repo.ToQueryable().FirstAsync().Result;
            var c3 = repo.GetById("ALFKI");
            var c4 = repo.GetByIdAsync("ALFKI").Result;

            var q5 = repo.QueryById("ALFKI").Select(x => x.Address);
            var sq5 = q5.ToString();
            var c5 = q5.SingleOrDefault();
        }
    }


}

namespace DomainModel
{
    public class Customer : DomainObjects.Core.DomainEntity
    {
        public string CustomerID { get; set; } // nchar(5)
        public string CompanyName { get; set; } // nvarchar(40)
        public string ContactName { get; set; } // nvarchar(30)
        public string ContactTitle { get; set; } // nvarchar(30)
        public string Address { get; set; } // nvarchar(60)
        public string City { get; set; } // nvarchar(15)
        public string Region { get; set; } // nvarchar(15)
        public string PostalCode { get; set; } // nvarchar(10)
        public string Country { get; set; } // nvarchar(15)
        public string Phone { get; set; } // nvarchar(24)
        public string Fax { get; set; } // nvarchar(24)
    }
}
