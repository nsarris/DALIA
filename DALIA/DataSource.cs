using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia
{
    public class DataSource : IDataSource
    {
        public string ConnectionString { get; private set; }

        public string ProviderTypeString { get; private set; }

        public ProviderTypes ProviderType { get; private set; }

        public DataSource(string providerType, string connectionString)
        {
            if (Enum.TryParse<ProviderTypes>(providerType, out var type))
                ProviderType = type;
            else
                ProviderType = ProviderTypes.Other;
            ProviderTypeString = providerType;
            ConnectionString = connectionString;
        }

        public DataSource(ProviderTypes providerType, string connectionString)
        {
            ProviderType = providerType;
            ProviderTypeString = providerType.ToString();
            ConnectionString = connectionString;
        }
    }
}
