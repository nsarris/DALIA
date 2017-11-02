using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia
{
    public class DataSource : IDataSource
    {
        public string Key { get; private set; }
        public string ConnectionString { get; private set; }
        public string ProviderTypeString { get; private set; }
        public ProviderTypes ProviderType { get; private set; }
        public int Version { get; set; }
        public Type DbConnectionType { get; private set; }

        
        public DataSource(ProviderTypes providerType, string connectionString)
        {
            ProviderType = providerType;
            ProviderTypeString = providerType.ToString();
            ConnectionString = connectionString;
            if (ProviderType == ProviderTypes.SqlServer)
            {
                DbConnectionType = typeof(SqlConnection);
            }
        }

        public DataSource(string key, ProviderTypes providerType, string connectionString, int version = 0)
        {
            Key = key;
            ProviderType = providerType;
            ProviderTypeString = providerType.ToString();
            ConnectionString = connectionString;
            if (ProviderType == ProviderTypes.SqlServer)
            {
                DbConnectionType = typeof(SqlConnection);
            }
            else if (ProviderType == ProviderTypes.DB2iSeries)
            {
                DbConnectionType = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x.GetName().Name == "IBM.Data.DB2.iSeries")
                    .SelectMany(x => x.GetTypes().Where(t => t.Name == "iDB2Connection"))
                    .FirstOrDefault();
            }
            Version = version;
        }
    }
}
