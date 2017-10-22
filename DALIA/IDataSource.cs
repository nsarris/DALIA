using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia
{

    public enum ProviderType
    {
        SqlServer,
        DB2iSeries,
        Other
    }
    public interface IDataSource
    {
        string ConnectionString { get; }
        string ProviderTypeString { get; }
        ProviderType ProviderType { get; }
    }
}
