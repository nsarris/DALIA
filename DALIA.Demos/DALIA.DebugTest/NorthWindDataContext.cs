using Dalia.Linq2db;
using Dalia;
using Dalia.DI;

namespace DALIA.DebugTest
{
    [DefaultDataSource("Northwind")]
    public class NorthWindDataContext : LinqToDBDataContext<DataModel.Northwind.NorthwndDB>
    {
        public NorthWindDataContext(IDataSource dataSource) : base(dataSource)
        {

        }

        public NorthWindDataContext(DataModel.Northwind.NorthwndDB dataConnection) : base(dataConnection)
        {
        }

        public NorthWindDataContext(string provider, string connectionString) : base(provider, connectionString)
        {
            
        }

        //public IQueryable<Customer> Customers => this.Linq2DBDataConnection.GetTable<Customer>();

        //public IQueryable<Customer> FTestCusomters(int? p1)
        //{
        //    return this.Linq2DBDataConnection.FTestCustomers(p1);
        //}
    }

   
}
