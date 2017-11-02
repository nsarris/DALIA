using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LinqToDB.DataProvider;

namespace Dalia.Linq2db
{
    public abstract class LinqToDBDataConnection : LinqToDB.Data.DataConnection
    {
        public LinqToDBDataConnection(IDataSource dataSource)
            : base(DataSourceToLinqToDBProvider(dataSource), dataSource.ConnectionString)
        {

        }

        public LinqToDBDataConnection(IDataSource dataSource, DbConnection connection)
            : base(DataSourceToLinqToDBProvider(dataSource), connection)
        {

        }

        public LinqToDBDataConnection(string providerName, string connectionString) 
            : base(providerName, connectionString)
        {
        }

        public static LinqToDB.DataProvider.IDataProvider DataSourceToLinqToDBProvider(IDataSource dataProvider)
        {
            if (dataProvider.ProviderType == ProviderTypes.SqlServer)
            {
                if (dataProvider.Version == 2000)
                    return new LinqToDB.DataProvider.SqlServer.SqlServerDataProvider("", LinqToDB.DataProvider.SqlServer.SqlServerVersion.v2000);
                else if (dataProvider.Version == 2005)
                    return new LinqToDB.DataProvider.SqlServer.SqlServerDataProvider("", LinqToDB.DataProvider.SqlServer.SqlServerVersion.v2005);
                else if (dataProvider.Version == 2008)
                    return new LinqToDB.DataProvider.SqlServer.SqlServerDataProvider("", LinqToDB.DataProvider.SqlServer.SqlServerVersion.v2008);
                else //if (dataProvider.Version == 2012)
                    return new LinqToDB.DataProvider.SqlServer.SqlServerDataProvider("", LinqToDB.DataProvider.SqlServer.SqlServerVersion.v2012);

            }
            else if (dataProvider.ProviderType == ProviderTypes.DB2iSeries)
            {
                var type = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x.GetName().Name == "LinqToDB.DataProvider.DB2iSeries")
                    .SelectMany(x => x.GetTypes().Where(t => t.Name == "DB2iSeriesDataProvider"))
                    .FirstOrDefault();

                return (IDataProvider)Activator.CreateInstance(type);
            }
            else
                throw new Exception("Invalid provider type");
        }
    }
}
