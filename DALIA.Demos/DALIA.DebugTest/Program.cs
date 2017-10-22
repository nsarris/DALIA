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
            
        }
    }


}

